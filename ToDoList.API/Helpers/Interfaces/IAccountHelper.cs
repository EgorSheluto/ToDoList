using System.Security.Claims;
using ToDoList.API.Authentication.Interfaces;
using ToDoList.DAL.Models;

namespace ToDoList.API.Helpers.Interfaces
{
    public interface IAccountHelper
    {
        string GenerateJwtToken(UserModel user, IJwtSigningEncodingKey signingEncodingKey, IJwtEncryptingEncodingKey encryptingEncodingKey);

        string GenerateRefreshToken();

        public ClaimsPrincipal GetPrincipal(string token, bool isExpired = false);
    }
}
