using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace WeatherChecker
{
    class Program
    {
        static void Main(string[] args)
        {

            
            IConfiguration Config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string sAccess_Key = Config.GetSection("Rules").GetSection("Access_Key").Value;
            string sURL = Config.GetSection("URL").Value;
            int iWind = Convert.ToInt32(Config.GetSection("Wind_Speed_Min").Value);
            int iUVMax = Convert.ToInt32(Config.GetSection("UV_Max_Threshold").Value);
            bool bIsRaining = false;
            string[] sRainCodes = Config.GetSection("Rules").GetSection("Rain_Codes").Get<string[]>();

            Console.Write("Enter Your ZIP Code and Press Any Key:");
            var zip = Console.ReadLine();
            int X;
            WeatherStack WS = new WeatherStack();
            WS.AccessKey = sAccess_Key;
            WS.URL = sURL;

            while (!Int32.TryParse(zip, out X)) // Only Accept Number for Zip Code
            {
                Console.WriteLine("Not a valid ZIP CODE, Please Try Again.");

                zip = Console.ReadLine();
            }

            Console.WriteLine();
            WeatherResults WSResults = new WeatherResults();
            WSResults = WS.PerformWeatherCheck(zip);

            if (WSResults.Status == "success")
            {
                Console.WriteLine("YOUR LOCATION IS: " + WSResults.Location);
                Console.WriteLine();


                //Check if it's raining
                string sWeatherCode = WSResults.WeatherCode;
                int stringIndex = Array.IndexOf(sRainCodes, sWeatherCode); // Loop through rain weather codes to check all rain codes
                if (stringIndex >= 0)
                {
                    Console.WriteLine("Should I go outside?: No");  // If string is found then it's raining don't go out
                    bIsRaining = true;  // It's raining set to true
                }
                else
                {
                    Console.WriteLine("Should I go outside?: Yes! | The weather is: " + WSResults.WeatherDescription[0]); ;
                    bIsRaining = false; // It's NOT raining set to flase
                }

                //Check if it's hot
                if (WSResults.UV_Index > iUVMax) // UV Allowable rate is 3, more than the wear sunscreen
                {
                    Console.WriteLine("Should I wear sunscreen?: Yes | UV INDEX IS: " + WSResults.UV_Index);
                }
                else
                {
                    Console.WriteLine("Should I wear sunscreen?: No | UV INDEX IS: " + WSResults.UV_Index);
                }

                if (WSResults.WindSpeed > iWind) //Windspeed should be more than 15 value to fly a kite
                {
                    if (bIsRaining) // don't fly if its raining
                    {
                        Console.WriteLine("Can I fly my kite?: No | Wind Speed is: " + WSResults.WindSpeed);
                        Console.WriteLine("It's windy but it's also raining");
                    }
                    else
                        Console.WriteLine("Can I fly my kite?: Yes! | Wind Speed is: " + WSResults.WindSpeed);

                }
                else
                {

                    Console.WriteLine("Can I fly my kite?: No | Wind Speed is: " + WSResults.WindSpeed);
                    Console.WriteLine("There's not enough wind speed to fly your kite.");

                }



                // var currentDate = DateTime.Now;
                //  Console.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");
                Console.Write($"{Environment.NewLine}Press any key to exit...");
                Console.ReadKey(true);
            }
            else
            {
                Console.WriteLine("The application is having difficulty accessing WeatherStack API");
                Console.WriteLine("Please contact the author or try again later.");
                Console.WriteLine("Error Message: " + WSResults.ErrorMsg); 
                Console.Write($"{Environment.NewLine}Press any key to exit...");
            }
        }
    }
}
