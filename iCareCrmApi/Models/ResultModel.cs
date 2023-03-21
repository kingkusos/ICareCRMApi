using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCareCrmApi.Models
{
    public class ResultModel
    {
        public bool status { get; set; }
        public string error { get; set; }
        public int code { get; set; }
        public object data { get; set; }
    }
}