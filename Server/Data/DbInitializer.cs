using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipEase.Shared.Models;
using System.IO;
using System.Collections.Generic;
using LumenWorks.Framework.IO.Csv;


namespace RecipEase.Server.Data
{
    public static class DbInitializer
    {
        public static void Initialize(RecipEaseContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.WeatherForecasts.Any())
            {
                return;   // DB has been seeded
            }

            var Summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            var rng = new Random();
            var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                Humidity = 5.4
            }).ToArray();

            context.WeatherForecasts.AddRange(weatherForecasts);
            context.SaveChanges();
        }
        public static void InitializeUnit(RecipEaseContext context)
        {



            CsvReader csv;

            csv = new CsvReader(new StreamReader("unit_and_ingr/volume.csv"), true);

            UnitType utype;
            String uname, usym;
            Unit temp;

            if (!context.Unit.Any())
            {
                while (csv.ReadNextRecord())
                {
                    utype = UnitType.Volume;
                    uname = csv[0];
                    usym = csv[1];
                    temp = new Unit { Name = uname, UnitType = utype, Symbol = usym };
                    context.Unit.Add(temp);

                }


                csv = new CsvReader(new StreamReader("unit_and_ingr/weight.csv"), true);

                while (csv.ReadNextRecord())
                {
                    utype = UnitType.Weight;
                    uname = csv[0];
                    usym = csv[1];

                    temp = new Unit { Name = uname, UnitType = utype, Symbol = usym };
                    context.Unit.Add(temp);
                }
                context.SaveChanges();
            }


            if (!context.UnitConversion.Any())
            {

                Dictionary<Unit, double> myDict;

                csv = new CsvReader(new StreamReader("unit_and_ingr/frompound.csv"));
                myDict = new Dictionary<Unit, double>();

                while (csv.ReadNextRecord())
                {
                    Unit aUnit = context.Unit.Find(csv[0]);
                    myDict.Add(aUnit, Double.Parse(csv[1]));
                }
                myDict.ToList().ForEach(x => myDict.ToList().ForEach(y =>
                context.UnitConversion.Add(new UnitConversion { ConvertsToUnitName = x.Key.Name, ConvertsFromUnitName = y.Key.Name, Ratio = x.Value / y.Value })));
                context.SaveChanges();

                csv = new CsvReader(new StreamReader("unit_and_ingr/fromfluidounce.csv"));
                myDict = new Dictionary<Unit, double>();

                while (csv.ReadNextRecord())
                {
                    Unit aUnit = context.Unit.Find(csv[0]);
                    myDict.Add(aUnit, Double.Parse(csv[1]));
                }
                myDict.ToList().ForEach(x => myDict.ToList().ForEach(y =>
                context.UnitConversion.Add(new UnitConversion { ConvertsToUnitName = x.Key.Name, ConvertsFromUnitName = y.Key.Name, Ratio = x.Value / y.Value })));
                context.SaveChanges();
            }
        }
    }
}