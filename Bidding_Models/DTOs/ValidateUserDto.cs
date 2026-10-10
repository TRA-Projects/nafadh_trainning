using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bidding_Models.DTOs
{
    public class ValidateUserDto
    {
        public string refNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
