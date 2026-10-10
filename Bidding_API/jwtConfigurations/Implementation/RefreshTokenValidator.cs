using Bidding_API.jwtConfigurations.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Bidding_API.jwtConfigurations.Implementation
{
    /// <summary>
    /// Refresh token Validator
    /// </summary>
    public class RefreshTokenValidator : IRefreshTokenValidator
    {
        private readonly JwtBearerTokenSettings _jwtSettings;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jwtSettings"></param>
        public RefreshTokenValidator(JwtBearerTokenSettings jwtSettings) => _jwtSettings = jwtSettings;
        /// <summary>
        /// Validate refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public bool Validate(string refreshToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtBearerTokenSecretKey.RefreshKeyTokenSecret)),
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                jwtSecurityTokenHandler.ValidateToken(refreshToken, validationParameters,
                    out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
