using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public class ChunkFile
    {
        public string ClientID { get; set; }
        public byte[] Chunk { get; set; }
        public string FileName { get; set; }

        public IFormFile  File { get; set; }
    }
}
