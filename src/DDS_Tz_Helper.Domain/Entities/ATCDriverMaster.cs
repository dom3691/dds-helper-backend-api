using System;
using System.Collections.Generic;

namespace DDS_Tz_Helper.Domain.Entities
{
   
    public partial class ATCDriverMaster
    {
        public int id { get; set; }
        public string status { get; set; }
        public string cab_no { get; set; }
        public string truck_no { get; set; }
        public string transit_div { get; set; }
        public string fleet { get; set; }
        public string driver_name { get; set; }
        public string driver_phone { get; set; }
        public string driver_sap_id { get; set; }
    }
}

