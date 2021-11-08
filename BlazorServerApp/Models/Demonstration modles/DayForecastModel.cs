namespace BlazorServerApp
{
    public class DayForecastModel { 

        public string weather_state_name { get; set; }
        public string applicable_date { get; set; }
        public double min_temp { get; set; }
        public double max_temp { get; set; }
        public double the_temp { get; set; }
        public double wind_speed { get; set; }
        public double wind_direction { get; set; }
        public double air_pressure { get; set; }
        public int humidity { get; set; }
        public double visibility { get; set; }
        public int predictability { get; set; }
    }


}
