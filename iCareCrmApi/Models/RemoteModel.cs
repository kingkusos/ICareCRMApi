using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCareCrmApi.Models
{
    public class RemoteModel
    {
    }
    public class RemoteTypeModel
    {
        public string id { get; set; }
        public string type { get; set; }
    }
    public class RemoteInfoModel
    {
        public string id { get; set; }
        public string device { get; set; }
        public string type { get; set; }
        public string number { get; set; }
        public string pwd { get; set; }
    }
}