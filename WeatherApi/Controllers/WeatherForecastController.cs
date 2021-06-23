using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherApi.Helpers;
using WeatherApi.Models;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IGetWeatherDataHelper _getWeatherDataHelper;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IGetWeatherDataHelper getWeatherDataHelper)
        {
            _logger = logger;
            _getWeatherDataHelper = getWeatherDataHelper;
        }

        [HttpPost]
        [Route("getData")]
        public IActionResult GetData([FromBody] HVACRequest request)
        {
            if(request == null)
            {
                return BadRequest("Request is null");
            }
            if(request.StartDate == null || request.EndDate == null)
            {
                return BadRequest("Request is invalid. Missing StartDate or EndDate");
            }
            var startTime = DateTime.Parse(request.StartDate).Date;
            var endTime = DateTime.Parse(request.EndDate).Date;
            if(startTime > endTime)
            {
                return BadRequest("StartDate must be before EndDate");
            }
            var records = _getWeatherDataHelper.GetForecastData();
            var data = records.Where(w => (w.Temperature < 62 || w.Temperature > 75) && (w.DateTime.Date >= startTime && w.DateTime.Date <= endTime))
                .GroupBy(g => g.DateTime.Date,
                    g => g.Temperature,
                    (date, temp) =>
                        new HVACData { Day = date, AirConditionerOn = temp.Max() > 75, HeatingOn = temp.Min() < 62 }).ToList();
            return new JsonResult(data) { StatusCode = 200};
        }


    }
}
