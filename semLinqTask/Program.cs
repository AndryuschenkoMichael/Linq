using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;

namespace semLinqTask
{
    
    class Program
    {
        private const string PathToFile = @"D:\Projects C#\WeatherEvents_Jan2016-Dec2020.csv";
        
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

            // var countEventsOfCitys =
            //     from weatherEvent in weatherEvents
            //     group weatherEvent by weatherEvent.City
            //     into cityGroup
            //     select (City: cityGroup.Key, Count: cityGroup.Count());
            //
            // foreach (var (city, count) in countEventsOfCitys)
            // {
            //     Console.WriteLine($"{city} {count}");
            // }

            // Количество ивентов в 2018 году.
            int countAmerica = weatherEvents.Count(x => x.StartTime.Year == 2018);
            
            Console.WriteLine($"-1: Count events on 2018 = {countAmerica}");

            int countStates = 
                (from weatherEvent in weatherEvents 
                    group weatherEvent by weatherEvent.State
                    into statesGroup
                    select statesGroup.Key).Count();

            // Количество различных городов.
            int countCity =
                (from weatherEvent in weatherEvents
                    group weatherEvent by weatherEvent.City
                    into cityGroup
                    select cityGroup.Key).Count();

            Console.WriteLine($"0: Count of States = {countStates}; Count of City = {countCity}");
            
            // Топ 3 самых дождливых города в 2019 году. У меня в 2019 - значит начались в 2019.
            // Если нужно, чтобы они полностью были в 2019, то просто в Where нужно дописать: && x.EndTime.Year = 2019).
            // P.s. Кортежи в данном случае круче анонимных переменных, т.к. они также могут иметь нормальное имя полей
            // + к ним подцеплен интерфейс IComparable, значит к ним можно применить функцию Max и она работает корректно
            Console.WriteLine("1:");

            var top =
                from weatherEvent in weatherEvents
                where weatherEvent.StartTime.Year == 2019
                group weatherEvent by weatherEvent.City
                into cityGroup
                let rainDays = cityGroup.Count(x => x.WeatherType == WeatherEventType.Rain)
                orderby rainDays descending 
                select (City: cityGroup.Key,
                    RainDays: rainDays);


            foreach (var (city, rainDays) in top.Take(3))
            {
                Console.WriteLine($"{city} rain {rainDays}");
            }
            
            // Топ снегопадов в Америке по годам.
            Console.WriteLine("2:");

            var topYears =
                from weatherEvent in weatherEvents
                group weatherEvent by weatherEvent.StartTime.Year
                into yearGroup
                select (Year: yearGroup.Key,
                    CityInformation:
                    (from yearElement in yearGroup
                        where yearElement.WeatherType == WeatherEventType.Snow
                        select (Delta: yearElement.EndTime.Ticks - yearElement.StartTime.Ticks,
                            StartTime: yearElement.StartTime,
                            EndTime: yearElement.EndTime,
                            City: yearElement.City)).Max());

            foreach (var (year, cityInformation) in topYears)
            {
                Console.WriteLine($"{year}: | City: {cityInformation.City}; Start time: " +
                                  $"{cityInformation.StartTime}; End time: {cityInformation.EndTime}");
            }
        }
    }
}