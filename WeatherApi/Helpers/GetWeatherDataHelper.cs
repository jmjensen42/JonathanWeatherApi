using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace WeatherApi.Helpers
{
    public class GetWeatherDataHelper : IGetWeatherDataHelper
    {
        public IEnumerable<WeatherForecast> GetForecastData()
        {
            var source = File.ReadLines(@"history_data_hourly.csv").Select(line => line.Split(','));
            using var reader = new StreamReader("history_data_hourly.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<WeatherForecast>().ToList();
        }
    }
}
