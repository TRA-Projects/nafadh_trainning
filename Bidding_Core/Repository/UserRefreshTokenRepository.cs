

using Bidding_Core.Interfaces;
using Bidding_DLL.Data_Access_Layer;
using Bidding_Models.DatabaseModel;
using Bidding_DLL;


using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding_Core.Repository
{
    public class UserRefreshTokenRepository : IUserRefreshToken
    {
      
        private readonly UserRefreshTokenDAC userRefreshTokenDAC;

        public UserRefreshTokenRepository(UserRefreshTokenDAC UserRefreshTokenDAC)
        {
           
            userRefreshTokenDAC = UserRefreshTokenDAC;
        }

        public UserRefreshToken Add(UserRefreshToken refreshtoken)
        {
          return  userRefreshTokenDAC.Add(refreshtoken);
            
        }

        public async Task<int> Count(string UserId)
        {

            return await userRefreshTokenDAC.Count(UserId);
          

        }

        public bool Delete(string UserId)
        {
            return userRefreshTokenDAC.Delete(UserId);
        }

        public async Task<bool>  IsValid(UserRefreshToken refreshtoken)
        {
            return await userRefreshTokenDAC.IsValid(refreshtoken);

        }

        public UserRefreshToken Update(int RefreshtokenId ,UserRefreshToken refreshtoken)
        {
            return userRefreshTokenDAC.Update(RefreshtokenId,refreshtoken);
           
        }

        public  UserRefreshToken Get(UserRefreshToken refreshtoken)
        {
            return userRefreshTokenDAC.Get(refreshtoken);

        }
    }
}
