using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DotSpatial.Positioning;

namespace location
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var time = Stopwatch.StartNew();

            var locations = GetLocations().Take(100);

            foreach (var location in locations)
            {
                Console.WriteLine($"{location}");
                foreach (var activity in location.Activities ?? Enumerable.Empty<object>())
                {
                    Console.WriteLine($"--{activity}");
                }
            }

//            Console.WriteLine($"total {JsonConvert.SerializeObject(locations, Formatting.Indented)}");

            Console.WriteLine($"took {time.Elapsed:g}");
            Console.WriteLine("^");
        }

        private static IEnumerable<Location> GetLocations(string filePath = "locations.json") 
        {
            var serializer = new JsonSerializer();
            
            using (var reader = new JsonTextReader(File.OpenText(filePath))) {
                
                // get to location array
                foreach (var _ in Enumerable.Range(0, 3))
                    reader.Read();

                while (reader.Read())
                    if (reader.TokenType == JsonToken.StartObject)
                        yield return serializer.Deserialize<Location>(reader);
            }
        }
    }

    class Location 
    {
        [JsonProperty("timestampMs")]
        public long TimestampMs { get; set; }

        [JsonProperty("latitudeE7")]
        public long LatitudeE7 { get; set; }

        [JsonProperty("longitudeE7")]
        public long LongitudeE7 { get; set; }

        [JsonProperty("accuracy")]
        public int Accuracy { get; set; }

        [JsonProperty("activity")]
        public List<Activity> Activities { get; set; }
        
        public DateTime DateTime
            => DateTimeOffset.FromUnixTimeMilliseconds(TimestampMs).DateTime;
        
        public Position Position 
            => new Position(new Latitude(LatitudeE7 / 1e7), new Longitude(LongitudeE7 / 1e7));

        public override string ToString() 
            => $"{DateTime} {Position} {Accuracy}";
    }

    class Activity
    {
        [JsonProperty("timestampMs")]
        public long TimestampMs { get; set; }

        [JsonProperty("activity")]
        public List<ActivityPossibility> Activities { get; set; }
        
        public DateTime DateTime
            => DateTimeOffset.FromUnixTimeMilliseconds(TimestampMs).DateTime;

        public override string ToString() 
            => $"{DateTime} {(Activities != null ? string.Join(", ", Activities) : "")}";
    }

    enum ActivityType {
        [EnumMember(Value = "STILL")]
        Still,
        
        [EnumMember(Value = "TILTING")]
        Tilting,
        
        [EnumMember(Value = "ON_FOOT")]
        OnFoot,
        
        [EnumMember(Value = "WALKING")]
        Walking,
        
        [EnumMember(Value = "RUNNING")]
        Running,
        
        [EnumMember(Value = "IN_VEHICLE")]
        InVehicle,
        
        [EnumMember(Value = "ON_BICYCLE")]
        OnBicycle,
        
        [EnumMember(Value = "IN_ROAD_VEHICLE")]
        InRoadVehicle,
        
        [EnumMember(Value = "IN_RAIL_VEHICLE")]
        InRailVehicle,
        
        [EnumMember(Value = "EXITING_VEHICLE")]
        ExitingVehicle,
        
        [EnumMember(Value = "UNKNOWN")]
        Unknown
    }

    class ActivityPossibility 
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityType Type { get; set; }

        public int Confidence { get; set; }

        public override string ToString() 
            => $"{Type} - {Confidence}";
    }
}
