using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
namespace WeatherChecker
{
    class WeatherStack
    {
        #region -- Properties --

            private string _AccessKey = "";
            private string _URL = "";

        #endregion

        public WeatherResults PerformWeatherCheck(string sZipCode)
        {
            WeatherResults WRData = new();
            
            try
            {
                                
                string destinationUrl = _URL + "current?access_key=" + _AccessKey + "&query=" + sZipCode;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(destinationUrl);  // Access WeatherStack API to fetch the weather data by Zip Code
                httpWebRequest.ProtocolVersion = HttpVersion.Version11; 
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

                    string json = JsonConvert.SerializeObject(new{});
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse response;
                response = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream newStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(newStream);
                var result = sr.ReadToEnd();
                
                var splashInfo = JsonConvert.DeserializeObject<TransactionResult>(result);  // Parse WeatherStack Weather Data and Place it to Weather Data Class
                if(splashInfo.Location == null)
                {
                    throw new InvalidOperationException("API Error or ZIPCODE not Found");
                }
                else
                { 
                    WRData.Location = splashInfo.Location.Name + ", " + splashInfo.Location.Country;
                    WRData.WeatherCode = splashInfo.Current.Weather_Code;
                    WRData.WeatherDescription = splashInfo.Current.Weather_Descriptions;
                    WRData.WindSpeed = Convert.ToInt32(splashInfo.Current.Wind_Speed);
                    WRData.UV_Index = Convert.ToInt32(splashInfo.Current.UV_Index);
                }



            }
            catch (Exception x)
            {
                WRData.Status = "fail";
                WRData.ErrorMsg = x.Message;

            }


            return WRData;



        }

        public string AccessKey
        {
            get { return _AccessKey; }
            set { _AccessKey = value; }
        }

        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
    }


}
    public class TransactionResult
    {

        public string Sucess { get; set; }
        public Location Location{ get; set; }
        public Current Current { get; set; }

     }

    public class Location
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Localtime { get; set; }
        
  
    }

    public class Current
    {
            public string Weather_Code { get; set; }
            public IList<string> Weather_Descriptions { get; set; }        
            public string Temperature { get; set; }

            public int Wind_Speed { get; set; }

            public int UV_Index { get; set; }
            
}



    public class WeatherResults
    {

            private string _APISuccess = "true";
            private string _Status = "success";
            private string _ErrorMsg = "";
            private string _Location = "";
            private string _WeatherCode = "";
            private int _WindSpeed = 0;
            private int _UV_Index = 0;
            private IList<string> _WeatherDescription;

            public string APISuccess
            {
                get { return _APISuccess; }
                set { _APISuccess = value; }
            }
            public string Status
            {
                get { return _Status; }
                set { _Status = value; }
            }

            public string ErrorMsg
            {
                get { return _ErrorMsg; }
                set { _ErrorMsg = value; }
            }
            public string Location
            {
                get { return _Location; }
                set { _Location = value; }
            }

            public string WeatherCode
            {
                get { return _WeatherCode; }
                set { _WeatherCode = value; }
            }

            public IList<string> WeatherDescription
            {
                get { return _WeatherDescription; }
                set { _WeatherDescription = value; }
            }

            public int WindSpeed
            {
                get { return _WindSpeed; }
                set { _WindSpeed = value; }
            }

            public int UV_Index
            {
                get { return _UV_Index; }
                set { _UV_Index = value; }
            }








}

