using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ToDoList.API.Authentication
{
    /// <summary>
    /// Token generation settings
    /// </summary>
    public static class JwtSettings
    {
        public const bool ValidateIssuer = true;

        public const string ValidIssuer = "ToDoList.API";

        public const bool ValidateAudience = false;

        public const string ValidAudience = null;

        public const bool ValidateLifetime = true;

        public const bool RequireExpirationTime = true;

        // In prod this should be in a separate file
        public const string SigningKey = "e1c3d3ee339c4f6890a3e987a48ddd96ad28a14ccc034f12b6683d739f4d477f093436a61aae489091abf47e19265d42b61901809d78458bbbcb2d6e89581e42";

        public const string SigningAlgorithm = SecurityAlgorithms.HmacSha256;

        public static SymmetricSecurityKey GetSymmetricSigningKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));

        // In prod this should be in a separate file
        public const string EncodingKey = "ee339c4f6890a3e987a48ddd96ad28a14ccc034f12b6683d739f4d477f093436a61aae4890919b075ed06a986456ba9dbaa66b5f31488faabadaa81875e4faa";

        public const string EncodingSigningAlgorithm = JwtConstants.DirectKeyUseAlg;

        public const string EncodingEncryptingAlgorithm = SecurityAlgorithms.Aes256CbcHmacSha512;

        public static SymmetricSecurityKey GetSymmetricEncodingKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EncodingKey));

        public const bool ValidateIssuerSigningKey = true;

        public static TimeSpan ClockSew => TimeSpan.Zero;

        public static TokenValidationParameters GetValidationParameters(bool isValidateLifetime = ValidateLifetime) =>
            new TokenValidationParameters
            {
                ValidateIssuer = ValidateIssuer,
                ValidIssuer = ValidIssuer,
                ValidateAudience = ValidateAudience,
                ValidAudience = ValidAudience,
                ValidateLifetime = isValidateLifetime,
                IssuerSigningKey = GetSymmetricSigningKey(),
                TokenDecryptionKey = GetSymmetricEncodingKey(),
                ValidateIssuerSigningKey = ValidateIssuerSigningKey,
                RequireExpirationTime = RequireExpirationTime,
                ClockSkew = ClockSew
            };
    }
}
