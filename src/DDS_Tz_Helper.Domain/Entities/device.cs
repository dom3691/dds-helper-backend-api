using System;
using System.Collections.Generic;

namespace DDS_Tz_Helper.Domain.Entities
{
  
    public partial class device
    {
        public int id { get; set; }
        public Nullable<int> weighbridge_id { get; set; }
        public string device_type { get; set; }
        public string ip_address { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string channel { get; set; }
        public string port { get; set; }
        public string baud { get; set; }
        public string parity { get; set; }
        public string data_bits { get; set; }
        public string sensor_index { get; set; }
        public string sensor_green { get; set; }
        public string traffic_index { get; set; }
        public string traffic_green { get; set; }
        public string status { get; set; }
    
        public virtual weighbridge weighbridge { get; set; }
    }
}

