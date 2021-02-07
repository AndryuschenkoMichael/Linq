using System;

namespace semLinqTask
{
    class WeatherEvent
    {
        public string EventId { get; set; }
        public WeatherEventType WeatherType { get; set; }
        public Severitys Severity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TimeZone { get; set; }
        public string AirportZone { get; set; }
        public double LocationLat { get; set; }
        public double LocationLng { get; set; }
        public string City { get; set; }
        
        public string Country { get; set; }
        
        public string State { get; set; }
        
        public string ZipCode { get; set; }

        public WeatherEvent(string input)
        {
            try
            {
                string[] inputLines = input.Split(',');
                EventId = inputLines[0];
                
                if (!Enum.TryParse(inputLines[1], out WeatherEventType weather))
                {
                    weather = WeatherEventType.Unknown;
                }

                WeatherType = weather;


                if (!Severitys.TryParse(inputLines[2], out Severitys severity))
                {
                    severity = Severitys.Unknown;
                }

                Severity = severity;

                StartTime = DateTime.Parse(inputLines[3]);
                EndTime = DateTime.Parse(inputLines[4]);
                TimeZone = inputLines[5];
                AirportZone = inputLines[6];
                LocationLat = Double.Parse(inputLines[7].Replace('.', ','));
                LocationLng = Double.Parse(inputLines[8].Replace('.', ','));
                City = inputLines[9];
                Country = inputLines[10];
                State = inputLines[11];
                ZipCode = inputLines[12];
            }
            catch
            {
                throw new ArgumentException($"Can't parse string: '{input}', to WeatherEvent");
            }
        }

    }

    //Дополнить перечисления
    enum WeatherEventType
    {
        Unknown,
        Snow,
        Fog,
        Cold,
        Storm,
        Rain,
        Precipitation,
        Hail,
    }

    enum Severitys
    {
        Unknown,
        Light,
        Severe,
        Moderate,
        Heavy,
        UNK,
        Other,
    }
}