using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace semLinqTask
{
    
    class Program
    {
        private const string PathToFile = @"WeatherEvents_Jan2016-Dec2020.csv";
        
        //[SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        static void Main(string[] args)
        {
            //Нужно дополнить модель WeatherEvent, создать список этого типа List<>
            //И заполнить его, читая файл с данными построчно через StreamReader
            //Ссылка на файл https://www.kaggle.com/sobhanmoosavi/us-weather-events

            //Написать Linq-запросы, используя синтаксис методов расширений
            //и продублировать его, используя синтаксис запросов
            //(возможно с вкраплениями методов расширений, ибо иногда первого может быть недостаточно)
            
            //0. Linq - сколько различных городов есть в датасете.
            //1. Сколько записей за каждый из годов имеется в датасете.
            //Потом будут еще запросы
            
            List<WeatherEvent> weatherEvents = new List<WeatherEvent>();
            
            using (StreamReader sr = new StreamReader(PathToFile))
            {
                string line = sr.ReadLine();

                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        weatherEvents.Add(new WeatherEvent(line));
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    
                }
            }
            
            
            // Сколько записей за каждый из годов имеется в датасете. 
            // weatherEvents
            //     .GroupBy((x => x.City))
            //     .Select(x => (x.Key, x.Count()))
            //     .ToList()
            //     .ForEach(x => Console.WriteLine($"{x.Key} {x.Item2}"));

            // Количество ивентов в 2018 году.
            int countAmerica = weatherEvents
                .Count(x
                    => x.StartTime.Year == 2018);
            
            Console.WriteLine($"-1: Count events on 2018 = {countAmerica}");
            
            // Количество различных штатов.
            int countStates = weatherEvents
                .GroupBy(x 
                    => x.State)
                .Select(x 
                    => x.Key)
                .ToList()
                .Count();
            
            // Количество различных городов.
            int countCity = weatherEvents
                .GroupBy(x 
                    => x.City)
                .Select(x 
                    => x.Key)
                .ToList()
                .Count();
            
            Console.WriteLine($"0: Count of States = {countStates}; Count of City = {countCity}");
            
            // Топ 3 самых дождливых города в 2019 году. У меня в 2019 - значит начались в 2019.
            // Если нужно, чтобы они полностью были в 2019, то просто в Where нужно дописать: && x.EndTime.Year = 2018).
            // P.s. Кортежи в данном случае круче анонимных переменных, т.к. они также могут иметь нормальное имя полей
            // + к ним подцеплен интерфейс IComparable, значит к ним можно применить функцию Max и она работает корректно
            Console.WriteLine("1:");
            
            var top = weatherEvents
                .Where(x
                    => x.StartTime.Year == 2019)
                .GroupBy(x 
                    => x.City)
                .Select(x 
                    => (City: x.Key, RainDays: x
                    .Count(x 
                        => x.WeatherType == WeatherEventType.Rain)))
                .ToList();
                
            top.Sort((x, y) 
                => y.RainDays - x.RainDays);
            top
                .Take(3)
                .ToList()
                .ForEach(x 
                    => Console.WriteLine($"{x.City} rain {x.RainDays}"));

            
            // Топ снегопадов в Америке по годам.
            Console.WriteLine("2:");

            weatherEvents
                .GroupBy(x 
                    => x.StartTime.Year)
                .Select(x 
                    => (Year: x.Key, CityInformation: x
                    .Where(x 
                        => x.WeatherType == WeatherEventType.Snow)
                    .Select(x 
                        => (Delta: x.EndTime.Ticks - x.StartTime.Ticks, x.StartTime, x.EndTime, x.City))
                    .Max()))
                .ToList()
                .ForEach(x 
                    => Console.WriteLine($"{x.Year}: | City: {x.Item2.City}; Start time: " +
                                         $"{x.CityInformation.StartTime}; End time: {x.CityInformation.EndTime}"));

            // string city = weatherEvents
            //     .GroupBy(x => x.City)
            //     .Select(x =>
            //         (x.ToList().Sum(x =>
            //             (x.WeatherType == WeatherEventType.Rain) ? (x.EndTime - x.StartTime).Ticks : 0), 
            //         x.Key))
            //     .Max().Key;
            // Console.WriteLine(city);
            
            
        }
    }
}