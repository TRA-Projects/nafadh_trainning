using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
  public  class CHRequest <T> where T : class
    {
        public IFormFile  File { get; set; }
        public List<IFormFile> Files { get; set; }

        public T Data { get; set; }

       
    }
}
