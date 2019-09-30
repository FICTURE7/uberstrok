using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UberStrok.WebServices.AspNetCore.Configurations;

namespace UberStrok.WebServices.AspNetCore.Authentication.Jwt
{
    public class JwtAuthService : IAuthService
    {
        private readonly byte[] _secret;
        private readonly JwtEncoder _jwtEncoder;
        private readonly JwtDecoder _jwtDecoder;
        private readonly ILogger<JwtAuthService> _logger;

        public JwtAuthService(ILogger<JwtAuthService> logger, IOptions<AuthConfiguration> config, IHostingEnvironment env)
        {
            if (env == null)
                throw new ArgumentNullException(nameof(env));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (!string.IsNullOrEmpty(config.Value.Secret))
            {
                _secret = Encoding.UTF8.GetBytes(config.Value.Secret);
            }
            else
            {
                _secret = new byte[256];
                
                if (env.IsDevelopment())
                {
                    // NOTE:
                    // When in development mode, generate the secret from a
                    // known seeded random generator so we can restart the
                    // web services without invalidating older auth tokens.

                    var now = DateTime.UtcNow;
                    var seed = now.Day + now.Year + now.Month + env.GetHashCode();
                    var random = new Random(seed);
                    random.NextBytes(_secret);

                    _logger.LogCritical("\"Token.Secret\" was not configured in configuration, using DEVELOPMENT one.");
                }
                else
                {
                    using (var random = new RNGCryptoServiceProvider())
                        random.GetBytes(_secret);

                    _logger.LogWarning("\"Token.Secret\" was not configured in configuration, using randomly new generated one.");
                }
            }

            _jwtEncoder = new JwtEncoder(_secret);
            _jwtDecoder = new JwtDecoder(_secret);
        }

        public string Create(int memberId, DateTime createdAt, DateTime expiresAt)
        {
            var tokenPayload = new Dictionary<string, object> { { "mid", memberId } };
            var token = _jwtEncoder.Encode(tokenPayload, createdAt, expiresAt);
            return token;
        }

        public AuthError TryGet(string authToken, out int memberId)
        {
            memberId = 0;

            try
            {
                var jwtPayload = _jwtDecoder.Decode(authToken);
                memberId = (int)(long)jwtPayload["mid"];
            }
            catch (SecurityTokenExpiredException)
            {
                return AuthError.Expired;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return AuthError.InvalidSignature;
            }

            return AuthError.Ok;
        }
    }
}
