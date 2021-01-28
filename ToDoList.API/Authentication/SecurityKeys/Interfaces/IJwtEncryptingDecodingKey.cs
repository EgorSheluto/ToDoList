using Microsoft.IdentityModel.Tokens;

namespace ToDoList.API.Authentication.Interfaces
{
    /// <summary>
    /// Decoding private key
    /// </summary>
    public interface IJwtEncryptingDecodingKey
    {
        SecurityKey GetKey();
    }
}
