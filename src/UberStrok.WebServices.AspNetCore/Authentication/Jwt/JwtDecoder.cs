using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace UberStrok.WebServices.AspNetCore.Authentication.Jwt
{
    public class JwtDecoder
    {
        private readonly TokenValidationParameters _validationParams;
        private readonly SymmetricSecurityKey _signKey;

        public JwtDecoder(byte[] signKey)
        {
            if (signKey == null)
                throw new ArgumentNullException(nameof(signKey));

            _signKey = new SymmetricSecurityKey(signKey);
            _validationParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateActor = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signKey,
            };
        }

        public Dictionary<string, object> Decode(string encodedToken)
        {
            new JwtSecurityTokenHandler().ValidateToken(encodedToken, _validationParams, out SecurityToken token);
            return ((JwtSecurityToken)token).Payload;
        }

        public T Decode<T>(string encodedToken)
            => JObject.FromObject(Decode(encodedToken)).ToObject<T>();
    }
}
