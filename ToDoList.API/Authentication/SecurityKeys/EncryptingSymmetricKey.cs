﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ToDoList.API.Authentication.Interfaces;

namespace ToDoList.API.Authentication.SecurityKeys
{
    public class EncryptingSymmetricKey : IJwtEncryptingEncodingKey, IJwtEncryptingDecodingKey
    {
        private readonly SymmetricSecurityKey _secretKey;

        public string SigningAlgorithm { get; } = JwtConstants.DirectKeyUseAlg;

        public string EncryptingAlgorithm { get; } = SecurityAlgorithms.Aes256CbcHmacSha512;

        public EncryptingSymmetricKey(string key)
        {
            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public SecurityKey GetKey() => _secretKey;
    }
}
