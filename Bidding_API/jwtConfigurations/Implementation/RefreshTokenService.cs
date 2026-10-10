using Bidding_API.jwtConfigurations;
using Bidding_API.jwtConfigurations.Interfaces;
using Bidding_Models.DatabaseModel;


namespace Bidding_API.jwtConfigurationss.Implementation
{
    /// <summary>
    /// Refresh Token service
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly JwtBearerTokenSettings _jwtSettings;

        /// <summary>
        /// Refresh Token Constructor
        /// </summary>
        /// <param name="tokenGenerator"></param>
        /// <param name="jwtSettings"></param>
        public RefreshTokenService(ITokenGenerator tokenGenerator, JwtBearerTokenSettings jwtSettings) =>
            (_tokenGenerator, _jwtSettings) = (tokenGenerator, jwtSettings);

        /// <summary>
        /// Generate Refresh token without claims. Claims are not required for refresh token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string Generate(AppUser user) => _tokenGenerator.Generate(JwtBearerTokenSecretKey.RefreshKeyTokenSecret,
            _jwtSettings.Issuer, _jwtSettings.Audience,
            _jwtSettings.RefreshTokenExpiryTimeInMinute);
    }
}
