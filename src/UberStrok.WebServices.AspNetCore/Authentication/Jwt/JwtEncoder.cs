using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace UberStrok.WebServices.AspNetCore.Authentication.Jwt
{
    public class JwtEncoder
    {
        private readonly SigningCredentials _signCred;
        private readonly SymmetricSecurityKey _signKey;

        public JwtEncoder(byte[] signKey)
        {
            if (signKey == null)
                throw new ArgumentNullException(nameof(signKey));

            _signKey = new SymmetricSecurityKey(signKey);
            _signCred = new SigningCredentials(_signKey, SecurityAlgorithms.HmacSha256);
        }

        public string Encode(Dictionary<string, object> payload, DateTime? issuedAt = null, DateTime? expiresAt = null)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtHeader = new JwtHeader(_signCred);
            var jwtPayload = new JwtPayload();

            foreach (var kv in payload)
                jwtPayload.Add(kv.Key, kv.Value);

            if (issuedAt.HasValue)
                jwtPayload.Add("iat", EpochTime.GetIntDate(issuedAt.Value));
            if (expiresAt.HasValue)
                jwtPayload.Add("exp", EpochTime.GetIntDate(expiresAt.Value));

            var token = new JwtSecurityToken(jwtHeader, jwtPayload);
            return jwtHandler.WriteToken(token);
        }

        public string Encode<T>(T payload)
            => Encode(JObject.FromObject(payload).ToObject<Dictionary<string, object>>());
    }
}
