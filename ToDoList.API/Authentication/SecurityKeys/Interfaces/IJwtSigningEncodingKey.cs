using Microsoft.IdentityModel.Tokens;

namespace ToDoList.API.Authentication.Interfaces
{
    /// <summary>
    /// Signing private key
    /// </summary>
    public interface IJwtSigningEncodingKey
    {
        string SigningAlgorithm { get; }
        SecurityKey GetKey();
    }
}
