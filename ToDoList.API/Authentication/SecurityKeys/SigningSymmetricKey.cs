using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDoList.API.Authentication.Interfaces;

namespace ToDoList.API.Authentication.SecurityKeys
{
    public class SigningSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
    {
        private readonly SymmetricSecurityKey _secretkey;

        public string SigningAlgorithm { get; } = SecurityAlgorithms.HmacSha256;

        public SigningSymmetricKey(string key)
        {
            _secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public SecurityKey GetKey() => _secretkey;
    }
}
