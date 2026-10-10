using Bidding_API.jwtConfigurations.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bidding_API.jwtConfigurations.Implementation
{
    /// <inheritdoc cref="ITokenGenerator"/>
    public class TokenGenerator : ITokenGenerator
    {
        /// <summary>
        /// Generates jwt token.
        /// </summary>
        public string Generate(string secretKey, string issuer, string audience, double expires, IEnumerable<Claim> claims = null)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                JwtSecurityToken securityToken = new JwtSecurityToken(issuer, audience,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(expires),
                    credentials);
                return new JwtSecurityTokenHandler().WriteToken(securityToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
