using ToDoList.API.Authentication.Interfaces;
using ToDoList.API.Authentication.SecurityKeys;
using ToDoList.API.Helpers;
using ToDoList.DAL.Models;
using Xunit;

namespace ToDoList.API.Tests.Helpers
{
    public class AccountHelperTests
    {
        private readonly AccountHelper _accountHelper;
        private readonly IJwtSigningEncodingKey _signingEncodingKey;
        private readonly IJwtEncryptingEncodingKey _encryptingEncodingKey;

        public AccountHelperTests()
        {
            _accountHelper = new AccountHelper();

            // Setting up a jwe keys
            const string signingSecurityKey = "745hvv43uhvfnvu2v";
            var signingKey = new SigningSymmetricKey(signingSecurityKey);

            const string encodingSecurityKey = "dfkng20jfsdjfvsdmvw";
            var encryptionEncodingKey = new EncryptingSymmetricKey(encodingSecurityKey);

            _signingEncodingKey = signingKey;
            _encryptingEncodingKey = encryptionEncodingKey;
        }

        // Won't passed
        [Fact]
        public void GenerateTokenTest()
        {
            var user = new UserModel { Login = "Kari" };
            var jweToken = _accountHelper.GenerateJwtToken(user, _signingEncodingKey, _encryptingEncodingKey);
            var result = string.IsNullOrEmpty(jweToken) || string.IsNullOrWhiteSpace(jweToken);

            Assert.False(result);
        }

        [Fact]
        public void GenerateRefreshTokenTest()
        {
            var refreshToken = _accountHelper.GenerateRefreshToken();
            var result = string.IsNullOrEmpty(refreshToken) || string.IsNullOrWhiteSpace(refreshToken);

            Assert.False(result);
        }

        // Won't passed
        [Fact]
        public void GetPrincipalBearerTest()
        {
            var user = new UserModel { Login = "Kari" };
            var jweToken = _accountHelper.GenerateJwtToken(user, _signingEncodingKey, _encryptingEncodingKey);
            var jwtBearer = $"Bearer ${jweToken}";
            var principals = _accountHelper.GetPrincipal(jwtBearer);

            Assert.Equal("Kari", principals.Identity.Name);
        }

        // Won't passed
        [Fact]
        public void GetPrincipalNoneTest()
        {
            var user = new UserModel { Login = "Kari" };
            var jweToken = _accountHelper.GenerateJwtToken(user, _signingEncodingKey, _encryptingEncodingKey);
            var principals = _accountHelper.GetPrincipal(jweToken) ?? null;

            Assert.Null(principals);
        }
    }
}
