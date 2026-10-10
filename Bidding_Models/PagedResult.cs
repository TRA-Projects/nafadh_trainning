using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models
{
  public  class PagedResult<T> : PagedResultBase<T>
    {
  
        public List<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
            
        }

      // public T SearchParametersObject { get; set; }

    }
}
