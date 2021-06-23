using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApi.Models
{
    public class HVACData
    {
        public DateTime Day { get; set; }
        public bool AirConditionerOn { get; set; }
        public bool HeatingOn { get; set; }
    }
}
