using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace User.SoftWare.Service
{
    public class Weathersojson
    {
        static WeatherCurrentInfosojson _current;
        static WeatherHistoryInfosojson _yesterday;
        static WeatherHistoryInfosojson[] _forecast;

        public static WeatherCurrentInfosojson Current => _current;
        public static WeatherHistoryInfosojson Yesterday => _yesterday;
        public static WeatherHistoryInfosojson[] Forecast => _forecast;

        public static void Load()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.sojson.com/open/api/weather/xml.shtml?city=滨江");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    XDocument xDocument = XDocument.Load(sr);

                    XElement element = xDocument.Root;
                    XElement eyesterday = element.Element("yesterday");
                    IEnumerable<XElement> eforecast = element.Element("forecast").Elements("weather");
                    List<WeatherHistoryInfosojson> wforecast = new List<WeatherHistoryInfosojson>();
                    foreach (var item in eforecast)
                    {
                        wforecast.Add(GetWeatherHistoryInfo(item));
                    }

                    _current = new WeatherCurrentInfosojson(
                    element.Element("city").Value,
                    element.Element("updatetime").Value,
                    int.Parse(element.Element("wendu").Value),
                    element.Element("fengxiang").Value,
                    element.Element("fengli").Value,
                    int.Parse(element.Element("shidu").Value.Substring(0, element.Element("shidu").Value.Length - 1)),
                    element.Element("sunrise_1").Value,
                    element.Element("sunset_1").Value);

                    _yesterday = new WeatherHistoryInfosojson(
                        eyesterday.Element("date_1").Value,
                        int.Parse(eyesterday.Element("high_1").Value.Substring(3, eyesterday.Element("high_1").Value.Length - 4)),
                        int.Parse(eyesterday.Element("low_1").Value.Substring(3, eyesterday.Element("low_1").Value.Length - 4)),
                        eyesterday.Element("day_1").Element("type_1").Value,
                        eyesterday.Element("day_1").Element("fx_1").Value,
                        eyesterday.Element("day_1").Element("fl_1").Value,
                        eyesterday.Element("night_1").Element("type_1").Value,
                        eyesterday.Element("night_1").Element("fx_1").Value,
                        eyesterday.Element("night_1").Element("fl_1").Value);

                    _forecast = wforecast.ToArray();
                }
            }
        }

        static WeatherHistoryInfosojson GetWeatherHistoryInfo(XElement element)
        {
            return new WeatherHistoryInfosojson(
                element.Element("date").Value,
                int.Parse(element.Element("high").Value.Substring(3, element.Element("high").Value.Length - 4)),
                int.Parse(element.Element("low").Value.Substring(3, element.Element("low").Value.Length - 4)),
                element.Element("day").Element("type").Value,
                element.Element("day").Element("fengxiang").Value,
                element.Element("day").Element("fengli").Value,
                element.Element("night").Element("type").Value,
                element.Element("night").Element("fengxiang").Value,
                element.Element("night").Element("fengli").Value
                );
        }
    }
    public struct WeatherCurrentInfosojson
    {
        string city;
        string updateTime;
        int temperature;
        string winddestination;
        string windpower;
        int humidity;
        string sunrisetime;
        string sunsettime;

        public WeatherCurrentInfosojson(string city, string updateTime, int temperature, string winddestination, string windpower, int humidity, string sunrisetime, string sunsettime)
        {
            this.city = city;
            this.updateTime = updateTime;
            this.temperature = temperature;
            this.winddestination = winddestination;
            this.windpower = windpower;
            this.humidity = humidity;
            this.sunrisetime = sunrisetime;
            this.sunsettime = sunsettime;
        }

        public string City { get => city; set => city = value; }
        public string UpdateTime { get => updateTime; set => updateTime = value; }
        public int Temperature { get => temperature; set => temperature = value; }
        public string Winddestination { get => winddestination; set => winddestination = value; }
        public string Windpower { get => windpower; set => windpower = value; }
        public int Humidity { get => humidity; set => humidity = value; }
        public string Sunrisetime { get => sunrisetime; set => sunrisetime = value; }
        public string Sunsettime { get => sunsettime; set => sunsettime = value; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} ",
                City, UpdateTime, Temperature, Winddestination, Windpower, Humidity, Sunrisetime, Sunsettime
                );
        }
    }
    public struct WeatherHistoryInfosojson
    {
        string date;
        int uppertemperature;
        int lowertemperature;
        string dayweather;
        string daywinddestination;
        string daywindpower;
        string nightweather;
        string nightwinddestination;
        string nightwindpower;

        public WeatherHistoryInfosojson(string date, int uppertemperature, int lowertemperature, string dayweather, string daywinddestination, string daywindpower, string nightweather, string nightwinddestination, string nightwindpower)
        {
            this.date = date;
            this.uppertemperature = uppertemperature;
            this.lowertemperature = lowertemperature;
            this.dayweather = dayweather;
            this.daywinddestination = daywinddestination;
            this.daywindpower = daywindpower;
            this.nightweather = nightweather;
            this.nightwinddestination = nightwinddestination;
            this.nightwindpower = nightwindpower;
        }

        public string Date { get => date; set => date = value; }
        public int Uppertemperature { get => uppertemperature; set => uppertemperature = value; }
        public int Lowertemperature { get => lowertemperature; set => lowertemperature = value; }
        public string Dayweather { get => dayweather; set => dayweather = value; }
        public string Daywinddestination { get => daywinddestination; set => daywinddestination = value; }
        public string Daywindpower { get => daywindpower; set => daywindpower = value; }
        public string Nightweather { get => nightweather; set => nightweather = value; }
        public string Nightwinddestination { get => nightwinddestination; set => nightwinddestination = value; }
        public string Nightwindpower { get => nightwindpower; set => nightwindpower = value; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} ", Date, Uppertemperature, Lowertemperature, Dayweather, Daywinddestination, Daywindpower, Nightweather, Nightwinddestination, Nightwindpower);
        }
    }

    public class Weatherwebxml
    {
        static DateTime lastTime = new DateTime();
        public static DateTime UpdateTime { get; private set; }
        public static WeatherForcastInfo[] Weather { get; private set; }
        public static string City { get; private set; }
        public static event Action Completed;
        public static void Load(string city)
        {
            if (DateTime.Now - lastTime > TimeSpan.FromHours(1))
            {
                WeatherService.WeatherWebService weatherservice = new WeatherService.WeatherWebService();
                string[] s = weatherservice.getWeatherbyCityName(city);
                DateTime updateTime = DateTime.Parse(s[4]);
                WeatherForcastInfo[] weatherinfo = new WeatherForcastInfo[3];
                weatherinfo[0] = new WeatherForcastInfo(DateTime.Today, GetWeather(s[6]), GetTemp(s[5]));
                weatherinfo[1] = new WeatherForcastInfo(DateTime.Today.AddDays(1), GetWeather(s[13]), GetTemp(s[12]));
                weatherinfo[2] = new WeatherForcastInfo(DateTime.Today.AddDays(2), GetWeather(s[18]), GetTemp(s[17]));

                City = s[1];
                UpdateTime = updateTime;
                Weather = weatherinfo;

                lastTime = DateTime.Now;
            }
            Completed?.Invoke();
        }
        public static async Task LoadAsync(string city)
        {
            await Task.Run(() => Load(city));
        }

        private static int[] GetTemp(string arg)
        {
            string[] ts = new string[2];
            int i = 0;
            foreach (var item in arg)
            {
                if ((item >= '0' && item <= '9') || item =='-')
                {
                    ts[i] += item;
                }
                else if (item == '/')
                {
                    i++;
                }
            }
            return new int[] { int.Parse(ts[0]), int.Parse(ts[1]) };
        }
        private static string GetWeather(string arg)
        {
            return arg.Split(' ')[1];
        }
    }
    public struct WeatherForcastInfo
    {
        public WeatherForcastInfo(DateTime time, string weather, int[] temp) : this()
        {
            Time = time;
            Weather = weather;
            LowerTemp = temp[0];
            UpperTemp = temp[1];
        }

        public DateTime Time { get;private set; }
        public string Weather { get; private set; }
        public int LowerTemp { get; private set; }
        public int UpperTemp { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}-{3}", Time.ToShortDateString(), Weather, LowerTemp, UpperTemp);
        }
    }
}
