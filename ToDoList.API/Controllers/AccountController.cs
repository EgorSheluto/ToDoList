using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ToDoList.API.Authentication.Interfaces;
using ToDoList.API.Controllers.Base;
using ToDoList.API.Helpers.Interfaces;
using ToDoList.DAL.Models;
using ToDoList.DAL.Repository.Interfaces;

namespace ToDoList.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IRepository<UserModel> _userRepository;
        private readonly IRepository<UserLoginModel> _userLoginRepository;
        private readonly IAccountHelper _accountHelper;

        public AccountController(IRepository<UserModel> userRepository,
                                 IRepository<UserLoginModel> userLoginRepository,
                                 IAccountHelper accountHelper)
        {
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _accountHelper = accountHelper;
        }

        /// <summary>
        /// Existing user authorization
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="authorization">Authorization Basic in Base64</param>
        /// <returns>Object with JWE and Refresh tokens</returns>
        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromHeader(Name = "User-Agent")] string userAgent,
                                               [FromHeader(Name = "Authorization")] string authorization,
                                               [FromServices] IJwtSigningEncodingKey signingEncodingKey,
                                               [FromServices] IJwtEncryptingEncodingKey encryptingEncodingKey)
        {
            if (string.IsNullOrEmpty(userAgent) ||
                string.IsNullOrWhiteSpace(userAgent))
            {
                return Json("Incorrect user-agent info");
            }
            if (string.IsNullOrEmpty(authorization) ||
                string.IsNullOrWhiteSpace(authorization))
            {
                return Json("Incorrect user data");
            }

            // Decoding from Base64 Authorization Basic <username:password>
            var clearAuth = authorization.Replace("Basic ", string.Empty);
            var base64EncodedBytes = Convert.FromBase64String(clearAuth);
            var base64EncodedString = Encoding.UTF8.GetString(base64EncodedBytes);
            var credentials = base64EncodedString.Split(":");
            if (credentials is null ||
                credentials?.Length != 2)
            {
                return Json("Incorrect data format for authorization");
            }

            // User verification
            var existUser = await _userRepository.Query()
                                                 .Where(s => s.Login == credentials[0])
                                                 .SingleOrDefaultAsync();

            if (existUser is null)
                return Json("Authorization error");
            if (!credentials[1].Equals(existUser?.Password))
                return Json("Incorrect password");

            // Generate Jwe token
            var jwtToken = _accountHelper.GenerateJwtToken(existUser, signingEncodingKey, encryptingEncodingKey);
            var refreshToken = _accountHelper.GenerateRefreshToken();

            if (refreshToken is null)
                return Json("Refresh token isn't formed");

            var dtUtcNow = DateTime.UtcNow;
            var userLogin = new UserLoginModel
            {
                RefreshToken = refreshToken,
                CreateDateUTC = dtUtcNow,
                ExpireDateUTC = dtUtcNow.AddDays(2),
                UserAgent = userAgent
            };
            var existUserLogin = await _userLoginRepository.Query()
                                                           .Where(s => s.UserId == existUser.Id && 
                                                                       s.UserAgent == userLogin.UserAgent)
                                                           .SingleOrDefaultAsync();
            // Delete existing login
            if (existUserLogin != null)
            {
                await _userLoginRepository.DeleteAsync(existUserLogin);
            }

            existUser.UsersLogins.Add(userLogin);
            await _userRepository.UpdateAsync(existUser);

            var response = new { jwtToken, refreshToken };

            return Json(response);
        }

        /// <summary>
        /// User logout
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [Route("logout")]
        public async Task<IActionResult> LogOut([FromHeader(Name = "User-Agent")] string userAgent,
                                                [FromHeader(Name = "Authorization")] string token)
        {
            if (userAgent is null)
                return Json("Incorrect user-agent info");

            var principal = _accountHelper.GetPrincipal(token);
            if (principal is null)
                return Json("Incorrect token");
            var login = principal?.Identity?.Name;
            var userLogin = await _userLoginRepository.Query()
                                                      .Where(s => s.User.Login.Equals(login) &&
                                                                  s.UserAgent.Equals(userAgent))
                                                      .SingleOrDefaultAsync();

            if (userLogin is null)
                return Json("Incorrect authorization data", HttpStatusCode.Unauthorized);

            await _userLoginRepository.DeleteAsync(userLogin);

            return Json(true);
        }

        /// <summary>
        /// Refresh tokens
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns>Object with JWE and Refresh tokens</returns>
        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromHeader(Name = "User-Agent")] string userAgent,
                                                      [FromHeader(Name = "Authorization")] string token,
                                                      [FromHeader(Name = "Authorization-Refresh")] string refreshToken,
                                                      [FromServices] IJwtSigningEncodingKey signingEncodingKey,
                                                      [FromServices] IJwtEncryptingEncodingKey encryptingEncodingKey)
        {
            if (userAgent is null)
                return Json("Incorrect user-agent info");
            if (refreshToken is null)
                return Json("Incorrect refresh token");

            // Token validation, decoding
            var principal = _accountHelper.GetPrincipal(token, false);
            if (principal is null)
                return Json("Incorrect token");
            var login = principal?.Identity?.Name;
            var oldUserLogin = await _userLoginRepository.Query()
                                                         .Where(s => s.User.Login.Equals(login) &&
                                                                     s.UserAgent.Equals(userAgent) &&
                                                                     s.RefreshToken.Equals(refreshToken))
                                                         .SingleOrDefaultAsync();
            if (oldUserLogin is null)
                return Json("Incorrect authorization data", HttpStatusCode.Unauthorized);

            // Expire time of refresh tokens are 2 days
            var dtUtcNow = DateTime.UtcNow;
            if (oldUserLogin?.ExpireDateUTC <= dtUtcNow)
            {
                await _userLoginRepository.DeleteAsync(oldUserLogin);
                return Json("Refresh token has been expired", HttpStatusCode.Unauthorized);
            }

            var user = await _userRepository.Query()
                                            .Where(s => s.Login.Equals(login))
                                            .SingleOrDefaultAsync();
            if (user is null)
                return Json("There is no user with this data");

            // Forming tokens 
            var newJwtToken = _accountHelper.GenerateJwtToken(user, signingEncodingKey, encryptingEncodingKey);
            var newRefreshToken = _accountHelper.GenerateRefreshToken();

            oldUserLogin.RefreshToken = newRefreshToken;
            oldUserLogin.CreateDateUTC = dtUtcNow;
            oldUserLogin.ExpireDateUTC = dtUtcNow.AddDays(2);

            await _userLoginRepository.UpdateAsync(oldUserLogin);

            var response = new { newJwtToken, newRefreshToken };

            return Json(response);
        }
    }
}
