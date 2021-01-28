using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using ToDoList.API.Authentication;
using ToDoList.API.Authentication.Interfaces;
using ToDoList.API.Helpers.Interfaces;
using ToDoList.DAL.Models;

namespace ToDoList.API.Helpers
{
    public class AccountHelper : IAccountHelper
    {
        public string GenerateJwtToken(UserModel user, IJwtSigningEncodingKey signingEncodingKey, IJwtEncryptingEncodingKey encryptingEncodingKey)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user?.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: JwtSettings.ValidIssuer,
                audience: JwtSettings.ValidAudience,
                subject: new ClaimsIdentity(claims),
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(5),
                issuedAt: DateTime.Now,
                signingCredentials: new SigningCredentials(
                        signingEncodingKey.GetKey(),
                        signingEncodingKey.SigningAlgorithm),
                encryptingCredentials: new EncryptingCredentials(
                        encryptingEncodingKey.GetKey(),
                        encryptingEncodingKey.SigningAlgorithm,
                        encryptingEncodingKey.EncryptingAlgorithm)
            );

            var jwteString = tokenHandler.WriteToken(token);
            return jwteString;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);
            return refreshToken;
        }

        public ClaimsPrincipal GetPrincipal(string token, bool isValidateLifetime = true)
        {
            var tokenValidationParameters = JwtSettings.GetValidationParameters(isValidateLifetime);

            var tokenHandler = new JwtSecurityTokenHandler();
            var clearToken = token.Replace("Bearer ", string.Empty);
            var principal = tokenHandler.ValidateToken(clearToken, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(JwtSettings.EncodingSigningAlgorithm, StringComparison.InvariantCultureIgnoreCase) ||
                !jwtSecurityToken.Header.Enc.Equals(JwtSettings.EncodingEncryptingAlgorithm, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
    }
}
