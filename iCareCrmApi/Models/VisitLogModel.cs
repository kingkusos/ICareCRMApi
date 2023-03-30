using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCareCrmApi.Models
{
    public class VisitLogModel
    {
        public string id { get; set; }
        public string visitor_id { get; set; }
        public string visitor_name { get; set; }
        public string content { get; set; }
        public string visit_datetime { get; set; }
        public string now_datetime { get; set; }
        public string visit_category { get; set; }
        public bool isApproval { get; set; }
        public string clinic_status { get; set; }
        public bool isEdit { get; set; }
    }
    public class VisitLogPageModel
    {
        public int totalpage { get; set; }
        public int total { get; set; }
        public List<VisitLogModel> list = new List<VisitLogModel>();
    }
}