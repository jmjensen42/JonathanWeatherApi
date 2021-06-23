using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherApi;
using WeatherApi.Controllers;
using WeatherApi.Helpers;
using WeatherApi.Models;

namespace WeatherApiTests
{
    [TestClass]
    public class WeatherForecastControllerTests
    {
        [TestMethod]
        public void NullRequestReturnsFailure()
        {
            new WeatherForecastControllerTestState()
                .GivenANullRequest()
                .WhenGetDataIsCalled()
                .ThenBadRequestIsReturned("Request is null");
        }

        [TestMethod]
        public void NullEndDateReturnsFailure()
        {
            new WeatherForecastControllerTestState()
                .GivenNullEndDate()
                .WhenGetDataIsCalled()
                .ThenBadRequestIsReturned("Request is invalid. Missing StartDate or EndDate");
        }

        [TestMethod]
        public void StartDateAfterEndDateReturnsFailure()
        {
            new WeatherForecastControllerTestState()
                .GivenStartDateAfterEndDate()
                .WhenGetDataIsCalled()
                .ThenBadRequestIsReturned("StartDate must be before EndDate");
        }

        [TestMethod]
        public void ValidDataGiveSuccessResponse()
        {
            new WeatherForecastControllerTestState()
                .GivenValidRequest()
                .WhenGetWeatherDataReturnsHVACData()
                .WhenGetDataIsCalled()
                .ThenDataIsWithinTimeFrame()
                .ThenDataWhereHVACDidNotTurnOnIsExcluded()
                .ThenCorrectHVACDataIsReturned()
                .ThenSuccessResponseIsReturned();

        }
        public class WeatherForecastControllerTestState
        {
            private HVACRequest hVACRequest;
            private WeatherForecastController controller;
            private ILogger<WeatherForecastController> logger;
            private IActionResult result;
            private IGetWeatherDataHelper getWeatherDataHelper;
            public WeatherForecastControllerTestState()
            {
                logger = A.Fake<ILogger<WeatherForecastController>>();
                getWeatherDataHelper = A.Fake<IGetWeatherDataHelper>();
                controller = new WeatherForecastController(logger, getWeatherDataHelper);
            }

            //Given
            public WeatherForecastControllerTestState GivenANullRequest()
            {
                hVACRequest = null;
                return this;
            }

            public WeatherForecastControllerTestState GivenNullEndDate()
            {
                hVACRequest = new HVACRequest()
                {
                    EndDate = null,
                    StartDate = "6/3/2021"
                };
                return this;
            }

            public WeatherForecastControllerTestState GivenStartDateAfterEndDate()
            {
                hVACRequest = new HVACRequest()
                {
                    EndDate = "6/1/2021",
                    StartDate = "6/3/2021"
                };
                return this;
            }

            public WeatherForecastControllerTestState GivenValidRequest()
            {
                hVACRequest = new HVACRequest()
                {
                    StartDate = "6/1/2021",
                    EndDate= "6/10/2021"
                };
                return this;
            }
            //When

            public WeatherForecastControllerTestState WhenGetDataIsCalled()
            {
                result = controller.GetData(hVACRequest);
                return this;
            }

            public WeatherForecastControllerTestState WhenGetWeatherDataReturnsHVACData()
            {
                IEnumerable<WeatherForecast> data = new List<WeatherForecast>(){
                    new WeatherForecast { DateTime = DateTime.Parse("5/1/2021  12:00:00 AM"), Temperature = 30},
                    new WeatherForecast { DateTime = DateTime.Parse("6/1/2021  12:00:00 AM"), Temperature = 30},
                    new WeatherForecast { DateTime = DateTime.Parse("6/2/2021  6:00:00 AM"), Temperature = 80},
                    new WeatherForecast { DateTime = DateTime.Parse("6/3/2021  2:00:00 AM"), Temperature = 80},
                    new WeatherForecast { DateTime = DateTime.Parse("6/3/2021  12:00:00 AM"), Temperature = 30},
                    new WeatherForecast { DateTime = DateTime.Parse("6/5/2021  7:00:00 PM"), Temperature = 66},
                    new WeatherForecast { DateTime = DateTime.Parse("6/5/2021  12:00:00 AM"), Temperature = 66},
                    new WeatherForecast { DateTime = DateTime.Parse("6/14/2021  12:00:00 AM"), Temperature = 30},
                };
                A.CallTo(() => getWeatherDataHelper.GetForecastData()).Returns(data);
                return this;
            }
            //Then

            public WeatherForecastControllerTestState ThenBadRequestIsReturned(string message)
            {
                var badResult = result as BadRequestObjectResult;
                Assert.AreEqual(badResult.StatusCode, 400);
                Assert.AreEqual(badResult.Value, message);
                return this;
            }

            public WeatherForecastControllerTestState ThenSuccessResponseIsReturned()
            {
                var jsonresult = result as JsonResult;
                Assert.AreEqual(jsonresult.StatusCode, 200);
                return this;
            }

            public WeatherForecastControllerTestState ThenDataIsWithinTimeFrame()
            {
                var jsonresult = result as JsonResult;
                var data = jsonresult.Value as IEnumerable<HVACData>;
                Assert.IsFalse(data.Any(d => d.Day.Equals(DateTime.Parse("6/14/2021"))));
                Assert.IsFalse(data.Any(d => d.Day.Equals(DateTime.Parse("5/1/2021"))));
                Assert.IsTrue(data.Any(d => d.Day.Equals(DateTime.Parse("6/3/2021"))));
                return this;
            }

            public WeatherForecastControllerTestState ThenDataWhereHVACDidNotTurnOnIsExcluded()
            {
                var jsonresult = result as JsonResult;
                var data = jsonresult.Value as IEnumerable<HVACData>;
                Assert.IsFalse(data.Any(d => d.Day.Equals(DateTime.Parse("6/5/2021"))));
                return this;
            }

            public WeatherForecastControllerTestState ThenCorrectHVACDataIsReturned()
            {
                var jsonresult = result as JsonResult;
                var data = jsonresult.Value as IEnumerable<HVACData>;
                Assert.IsTrue(data.Where(d => d.AirConditionerOn).Any(d => d.Day.Equals(DateTime.Parse("6/2/2021"))));
                Assert.IsTrue(data.Where(d => d.AirConditionerOn).Any(d => d.Day.Equals(DateTime.Parse("6/3/2021"))));
                Assert.IsTrue(data.Where(d => d.HeatingOn).Any(d => d.Day.Equals(DateTime.Parse("6/1/2021"))));
                Assert.IsTrue(data.Where(d => d.HeatingOn).Any(d => d.Day.Equals(DateTime.Parse("6/3/2021"))));
                Assert.IsFalse(data.Any(d => d.Day.Equals(DateTime.Parse("6/5/2021"))));
                return this;
            }
        }
    }
}
