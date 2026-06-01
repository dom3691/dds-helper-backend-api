using System;
using System.Collections.Generic;
using System.Linq;
namespace DDS_Tz_Helper.Application.DTOs.BusinessObjects
{
    public class trip_registry_dto
    {
        public string msg { get; set; }
        public string trip { get; set; }
        public string tms_status { get; set; }
        public string tms_online_status { get; set; }
        public Boolean status { get; set; }
    }
}
