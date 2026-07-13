using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NWC.DAL.NWCEntities;

namespace NWC.DTO.Common
{
    public class DirectionsResponse
    {
        [JsonProperty("routes")]
        public List<Route> routes { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Route
    {
        [JsonProperty("legs")]
        public List<Leg> Legs { get; set; }
    }

    public class Leg
    {
        [JsonProperty("distance")]
        public DirectionItem Distance { get; set; }

        [JsonProperty("duration")]
        public DirectionItem Duration { get; set; }

        [JsonProperty("steps")]
        public List<Step> Steps { get; set; }

        [JsonProperty("via_waypoint")]
        public List<Waypoint> Waypoints { get; set; }
    }

    public class Step
    {
        [JsonProperty("distance")]
        public DirectionItem Distance { get; set; }

        [JsonProperty("duration")]
        public DirectionItem Duration { get; set; }

        [JsonProperty("polyline")]
        public Polyline Polyline { get; set; }

    }

    public class Waypoint
    {
        [JsonProperty("step_index")]
        public string StepIndex { get; set; }
    }
    public class Polyline
    {
        [JsonProperty("points")]
        public string Points { get; set; }
    }
    public class DirectionItem
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}
