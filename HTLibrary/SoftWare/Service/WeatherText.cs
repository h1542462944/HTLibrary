using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.SoftWare.Service
{
    public sealed class WeatherText : AutoText
    {
        public WeatherText(string city):this()
        {
            City = city;

        }
        public WeatherText()
        {
            Weatherwebxml.Completed += WeatherLoadCompleted;
        }

        public event Action WeatherLoadCompleted;
        public override string Key => "weather";
        public string City { get; set; }
        protected override string GetText()
        {
            return string.Format("更新于{0}\n{1}\n{2}\n{3}",Weatherwebxml.UpdateTime,Weatherwebxml.Weather[0]
                    ,Weatherwebxml.Weather[1],Weatherwebxml.Weather[2]);
        }
        public async Task LoadWeather()
        {
            await Weatherwebxml.LoadAsync(City);
        }
    }
}
