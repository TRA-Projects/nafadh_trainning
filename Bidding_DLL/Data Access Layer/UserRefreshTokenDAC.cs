using Bidding_Models.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_DLL.Data_Access_Layer
{
  public  class UserRefreshTokenDAC
    {

        private readonly CHDBContext context;

        public UserRefreshTokenDAC(CHDBContext Context)
        {
            context = Context;
        }

        public UserRefreshToken Add(UserRefreshToken refreshtoken)
        {
            try
            {
                using (context)
                {
                    context.UserRefreshToken.Add(refreshtoken);
                    context.SaveChanges();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return refreshtoken;
        }

        public async Task<int> Count(string UserId)
        {
            try
            {
                using (context)
                {
                    return await context.UserRefreshToken.AsNoTracking().CountAsync(t => t.UserId == UserId);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool Delete(string UserId)
        {
            try
            {
                using (context)
                {
                    List<UserRefreshToken> tokens = context.UserRefreshToken.Where(t => t.UserId == UserId).ToList();
                    context.RemoveRange(tokens);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public async Task<bool> IsValid(UserRefreshToken refreshtoken)
        {
            try
            {
                using (context)
                {
                    int count = await context.UserRefreshToken.Where(t => t.UserId == refreshtoken.UserId && t.RefreshToken == refreshtoken.RefreshToken).CountAsync();
                    if (count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public UserRefreshToken Update(int RefreshtokenId, UserRefreshToken refreshtoken)
        {
            try
            {
                using (context)
                {
                    UserRefreshToken model = context.UserRefreshToken.FirstOrDefault(t => t.Id == RefreshtokenId);
                    if (model != null)
                    {
                        model.RefreshToken = refreshtoken.RefreshToken;
                        model.RefreshTokenExpirey = refreshtoken.RefreshTokenExpirey;
                        model.Token = model.Token;
                        model.TokenRenewDate = DateTime.UtcNow;
                        context.Entry(model).State = EntityState.Modified;
                        context.SaveChanges();
                        return refreshtoken;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserRefreshToken Get(UserRefreshToken refreshtoken)
        {
            try
            {
                using (context)
                {
                    return context.UserRefreshToken.AsNoTracking().Where(t => t.UserId == refreshtoken.UserId && t.RefreshToken == refreshtoken.RefreshToken && Convert.ToDateTime(t.RefreshTokenExpirey) > Convert.ToDateTime(DateTime.UtcNow)).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
