using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCareCrmApi.Models
{
    public class ClinicModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string road { get; set; }
        public string his { get; set; }
        public bool isUse_video { get; set; }
        public bool isDecided { get; set; }
        public int people { get; set; }
        public string call_number_way { get; set; }
        public string isVisit_datetime { get; set; }
        public string care_group { get; set; }
        public string experience { get; set; }
        public string care_network { get; set; }
        public string clinic_status { get; set; }
    }

    public class ClinicPageModel
    {
        public int totalpage { get; set; }
        public int total { get; set; }
        public List<ClinicListModel> list = new List<ClinicListModel>();
    }
    public class ClinicListModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string road { get; set; }
        public string visitor_id { get; set; }
        public string visitor_name { get; set; }
        public string visit_datetime { get; set; }
        public string clinic_status { get; set; }
    }
}