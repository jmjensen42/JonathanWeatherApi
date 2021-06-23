using System;
using CsvHelper.Configuration.Attributes;

namespace WeatherApi
{
    public class WeatherForecast
    {
        [Name("Date time")]
        public DateTime DateTime { get; set; }

        [Name("Temperature")]
        public double Temperature { get; set; }
    }
}
