using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace location
{
    public static class Program
    {
        private static PlacesConfig _placesConfig;
        
        private static void Main(string[] args)
        {
            var rootConfig = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
            _placesConfig = rootConfig.GetSection("Places").Get<PlacesConfig>();
            
            HandleArguments(args);

            var time = Stopwatch.StartNew();

            var home = _placesConfig.Home.Position;
            var work = _placesConfig.Work.Position;

            var locations = GetLocations().Take(100);

            foreach (var location in locations)
            {
                Console.WriteLine($"{location.Position.DistanceTo(work)}, {location.Position.DistanceTo(home)}");
                foreach (var activity in location.Activities ?? Enumerable.Empty<Activity>())
                {
                    Console.WriteLine($"--{activity}");
                }
            }

//             Console.WriteLine($"total {locations.ToJson()}");

            Console.WriteLine($"took {time.Elapsed:g}");
            Console.WriteLine("^");
        }

        private static void HandleArguments(IEnumerable<string> args)
        {
            var test = "hey";

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("t|test", ref test, "A test option");
                syntax.DefineOption("a|another", ref test, "A different option");
            });

            Console.WriteLine($"command line: {test}");
        }

        private static IEnumerable<Location> GetLocations()
        {
            var serializer = new JsonSerializer();

            using (var reader = new JsonTextReader(File.OpenText(_placesConfig.LocationsFile)))
            {
                // get to location array
                foreach (var _ in Enumerable.Range(0, 3))
                    reader.Read();

                while (reader.Read())
                    if (reader.TokenType == JsonToken.StartObject)
                        yield return serializer.Deserialize<Location>(reader);
            }
        }
    }
}