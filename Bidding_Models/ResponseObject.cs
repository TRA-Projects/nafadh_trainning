using Bidding_Models.DatabaseModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
  public  class ResponseObject <T>
    {
        public ResponseObject()
        {
         
        }
        public T Data { get; set; }
        public List<IdentityError> Error { get; set; }
        public ErrorDetails ServerError { get; set; }

        public string CustomMessage { get; set; }

        public static implicit operator ResponseObject<T>(PagedResult<UserTraining> v)
        {
            throw new NotImplementedException();
        }
    }
}
