using Microsoft.IdentityModel.Tokens;

namespace ToDoList.API.Authentication.Interfaces
{
    /// <summary>
    /// Encoding public key
    /// </summary>
    public interface IJwtEncryptingEncodingKey
    {
        string SigningAlgorithm { get; }
        string EncryptingAlgorithm { get; }
        SecurityKey GetKey();
    }
}
