using Microsoft.IdentityModel.Tokens;

namespace ToDoList.API.Authentication.Interfaces
{
    /// <summary>
    /// Signing public key
    /// </summary>
    public interface IJwtSigningDecodingKey
    {
        SecurityKey GetKey();
    }
}
