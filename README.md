# Weather HVAC DATA
Parse Weather data to determine which days the Airconditioner and Heater turns on.

This Program is written in .Net Core 3.1. Please read below on how to run the program.

## Prerequisite
Install .Net Core SDK 3.1 https://dotnet.microsoft.com/download

## Running the Program
To run the API project, run the following commands 


```
dotnet restore
dotnet build
dotnet run --project .\WeatherApi\WeatherApi.csproj
```

This should start the program on http://localhost:5000. You can then hit the api with Postman using the path http://localhost:5000/WeatherForecast/getData by POSTing the following body
```
{
    "startDate": "6/1/2020",
    "endDate": "8/3/2020"
}
```
Ctrl + C to exit the program

## Running the unit tests
```
dotnet restore
dotnet build
dotnet test  .\WeatherApiTests\WeatherApiTests.csproj
```