

using Bidding_Models.DatabaseModel;

namespace Bidding_API.jwtConfigurations.Interfaces
{
    /// <summary>
    /// Interface for generating token.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates token based on user information.
        /// </summary>
        /// <param name="user"><see cref="User"/> instance.</param>
        /// <returns>Generated token.</returns>
        string Generate(AppUser user);
    }


    /// <inheritdoc cref="ITokenService"/>
    public interface IRefreshTokenService : ITokenService { }
    /// <inheritdoc cref="ITokenService"/>
    public interface IAccessTokenService : ITokenService { }
}
