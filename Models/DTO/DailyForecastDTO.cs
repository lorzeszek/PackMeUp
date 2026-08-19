namespace PackMeUp.Models.DTO
{
    public class DailyForecastDTO
    {
        public DateTime Date { get; set; }
        public decimal MaxTemp { get; set; }
        public decimal MinTemp { get; set; }
        public int WeatherCode { get; set; }
        public string WeatherIcon { get; set; }
    }
}
