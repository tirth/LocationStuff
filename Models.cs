using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using DotSpatial.Positioning;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace location
{
    internal class Location
    {
        [JsonProperty("timestampMs")]
        private long TimestampMs { get; set; }

        [JsonProperty("latitudeE7")]
        private long LatitudeE7 { get; set; }

        [JsonProperty("longitudeE7")]
        private long LongitudeE7 { get; set; }

        [JsonProperty("accuracy")]
        public int Accuracy { get; private set; }

        [JsonProperty("activity")]
        public List<Activity> Activities { get; private set; }

        public DateTime DateTime
            => DateTimeOffset.FromUnixTimeMilliseconds(TimestampMs).DateTime;

        public Position Position
            => new Position(new Latitude(LatitudeE7 / 1e7), new Longitude(LongitudeE7 / 1e7));

        public override string ToString()
            => $"{DateTime} {Position} {Accuracy}";
    }
    
    internal class Activity
    {
        [JsonProperty("timestampMs")]
        private long TimestampMs { get; set; }

        [JsonProperty("activity")]
        public List<ActivityPossibility> Activities { get; set; }

        public DateTime DateTime
            => DateTimeOffset.FromUnixTimeMilliseconds(TimestampMs).DateTime;

        public override string ToString()
            => $"{DateTime} {(Activities != null ? string.Join(", ", Activities) : "")}";
    }
    
    internal class ActivityPossibility
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityType Type { get; set; }

        public int Confidence { get; set; }

        public override string ToString()
            => $"{Type} - {Confidence}";
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum ActivityType
    {
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
}