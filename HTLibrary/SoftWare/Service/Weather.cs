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
    public class Weather
    {
        static WeatherCurrentInfo _current;
        static WeatherHistoryInfo _yesterday;
        static WeatherHistoryInfo[] _forecast;

        public static WeatherCurrentInfo Current => _current;
        public static WeatherHistoryInfo Yesterday => _yesterday;
        public static WeatherHistoryInfo[] Forecast => _forecast;

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
                    List<WeatherHistoryInfo> wforecast = new List<WeatherHistoryInfo>();
                    foreach (var item in eforecast)
                    {
                        wforecast.Add(GetWeatherHistoryInfo(item));
                    }

                    _current = new WeatherCurrentInfo(
                    element.Element("city").Value,
                    element.Element("updatetime").Value,
                    int.Parse(element.Element("wendu").Value),
                    element.Element("fengxiang").Value,
                    element.Element("fengli").Value,
                    int.Parse(element.Element("shidu").Value.Substring(0, element.Element("shidu").Value.Length - 1)),
                    element.Element("sunrise_1").Value,
                    element.Element("sunset_1").Value);

                    _yesterday = new WeatherHistoryInfo(
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

        static WeatherHistoryInfo GetWeatherHistoryInfo(XElement element)
        {
            return new WeatherHistoryInfo(
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
    public struct WeatherCurrentInfo
    {
        string city;
        string updateTime;
        int temperature;
        string winddestination;
        string windpower;
        int humidity;
        string sunrisetime;
        string sunsettime;

        public WeatherCurrentInfo(string city, string updateTime, int temperature, string winddestination, string windpower, int humidity, string sunrisetime, string sunsettime)
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
    public struct WeatherHistoryInfo
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

        public WeatherHistoryInfo(string date, int uppertemperature, int lowertemperature, string dayweather, string daywinddestination, string daywindpower, string nightweather, string nightwinddestination, string nightwindpower)
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
}
