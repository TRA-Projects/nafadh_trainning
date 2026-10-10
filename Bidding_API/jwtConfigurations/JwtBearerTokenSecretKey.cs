using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding_API.jwtConfigurations
{
    public static class JwtBearerTokenSecretKey
    {
        public static string AccessKeyTokenSecret = "TrainingModule-Local-AccessToken-SigningKey-ChangeMe";
        public static string RefreshKeyTokenSecret = "TrainingModule-Local-RefreshToken-SigningKey-ChangeMe";
    }
}
