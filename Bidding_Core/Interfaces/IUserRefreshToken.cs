
using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding_Core.Interfaces
{
    public interface IUserRefreshToken
    {
        UserRefreshToken Add(UserRefreshToken refreshtoken);

        UserRefreshToken Update(int RefreshtokenId , UserRefreshToken refreshtoken);

        Task<int> Count(string UserId);

        bool Delete(string UserId);

        Task<bool> IsValid(UserRefreshToken refreshtoken);
        UserRefreshToken Get(UserRefreshToken refreshtoken);
    }
}
