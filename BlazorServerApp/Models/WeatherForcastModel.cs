using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp
{


    public class WeatherForecastModel
    {
        public DayForecastModel[] consolidated_weather { get; set; }
        public DateTime sun_rise { get; set; }
        public DateTime sun_set { get; set; }
        
        public string Title { get; set; }

        public string timezone { get; set; }
    }


}
