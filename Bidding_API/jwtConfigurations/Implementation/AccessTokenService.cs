using Bidding_API.jwtConfigurations.Interfaces;
using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Bidding_API.jwtConfigurations.Implementation
{
    /// <summary>
    /// For Access Token
    /// </summary>
    public class AccessTokenService : IAccessTokenService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly JwtBearerTokenSettings _jwtSettings;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tokenGenerator"></param>
        /// <param name="jwtSettings"></param>
        public AccessTokenService(ITokenGenerator tokenGenerator, JwtBearerTokenSettings jwtSettings) =>
            (_tokenGenerator, _jwtSettings) = (tokenGenerator, jwtSettings);

        /// <summary>
        /// Access Token Generating with claims
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Access token</returns>
        public string Generate(AppUser user)
        {
            try
            {

                List<Claim> UserClaimsList = new List<Claim>();

                if (user != null)
                {
                    foreach (var item in user.AppUserRoles)
                    {
                        UserClaimsList.Add(new Claim("Role", item?.Role?.Name));

                        if (item?.Role?.RolesClaimsList != null && item?.Role?.RolesClaimsList.Count() > 0)
                        {
                            foreach (var claim in item?.Role?.RolesClaimsList)
                            {

                                UserClaimsList.Add(new Claim(RemoveSpace(claim.ClaimMaster?.ClaimName), true.ToString()));

                            }
                        }

                        //UserClaimsList.Add(new Claim(ClaimTypes.Role, item.Name));
                        //UserClaimsList.AddRange(item.RoleClaims);

                    }
                    UserClaimsList.Add(new Claim(ClaimTypes.Name, user?.Profile?.Name ?? "Test"));
                    UserClaimsList.Add(new Claim(ClaimTypes.GivenName, user?.Profile?.ProfileType.ToString() ?? "0"));
                    UserClaimsList.Add(new Claim("ProfileType", user?.Profile?.ProfileType.ToString() ?? "-1"));
                    UserClaimsList.Add(new Claim("ProfileId", !String.IsNullOrEmpty(user?.ProfileId.ToString()) ? user?.ProfileId.ToString() : "0"));
                    UserClaimsList.Add(new Claim("ParentProfileId", !String.IsNullOrEmpty(user?.Profile?.ParentProfileId.ToString()) ? user?.Profile?.ParentProfileId.ToString() : "0"));
                    UserClaimsList.Add(new Claim("UserId", user?.Id.ToString() ?? String.Empty));
                    UserClaimsList.Add(new Claim("IsPostOpertunity", user?.Profile.IsPostOpertunity.ToString()));
                    UserClaimsList.Add(new Claim("Email", Convert.ToString(user?.Email)));
                }
            

            
                return _tokenGenerator.Generate(JwtBearerTokenSecretKey.AccessKeyTokenSecret, _jwtSettings.Issuer, _jwtSettings.Audience,
                    _jwtSettings.ExpiryTimeInMinute, UserClaimsList);
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        private string RemoveSpace(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                return Regex.Replace(str, " ", "");
            }

            return String.Empty;

        }
    }
}
