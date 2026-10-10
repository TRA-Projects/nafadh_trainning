using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    //[ModelBinder(typeof(JsonWithFilesFormDataModelBinder))]
    public class UserInfo
    {
        public UserInfo()
        {
            ImagesPath = new List<string>();
        }
        public string Name { get; set; }
        public List<IFormFile> Photo { get; set; }

        public List<string> ImagesPath { get; set; }

        public IFileWrapper FilesWrapper { get; set; }
        public Address UserAddress { get; set; }
    }

    public class testAddress {
        public string StreetNo { get; set; }
        public int houseNo { get; set; }
    }

    public class IFileWrapper
    {
        public IFormFile File { get; set; }
    }
}
