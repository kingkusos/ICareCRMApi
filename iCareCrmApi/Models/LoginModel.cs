using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCareCrmApi.Models
{
    public class LoginModel
    {
        public string token { get; set; }
        public string userId { get; set; }
    }
    public class LoginTokenModel
    {
        public string token { get; set; }
        public string expried { get; set; }
        public string id { get; set; }
    }
}