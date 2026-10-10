using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bidding_Models
{
  public static  class Conversion_Old
    {

        public static string ConvertToString(this string value) {
            if (value==null)
            {
                return String.Empty;
            }
            return value;
        
        }


        public static string HTMLToString(this string htmlstring)
        {
            if (htmlstring == null)
            {
                return String.Empty;
            }

            string str = htmlstring.Replace("&nbsp;", string.Empty);
            return Regex.Replace(str, "<.*?>", String.Empty);


           

        }

    
    }
}
