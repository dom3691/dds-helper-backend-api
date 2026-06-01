using System;
using System.Collections.Generic;
using System.Linq;
namespace DDS_Tz_Helper.Application.DTOs.BusinessObjects
{
    public class transaction_dto
    {
        public string msg { get; set; }
        public string driver { get; set; }
        public string vehicle { get; set; }
        public Boolean status { get; set; }
    }
}
