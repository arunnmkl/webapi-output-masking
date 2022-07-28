using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WebApi.Models
{
    public class Root
    {
        [JsonProperty("maskSettings")]
        public MaskSettings MaskSettings { get; set; }
    }

    public class MaskSettings
    {
        [JsonProperty("ignoredPaths")]
        public IEnumerable<string> IgnoredPaths { get; set; } = Enumerable.Empty<string>();

        [JsonProperty("masks")]
        public IEnumerable<Mask> Masks { get; set; } = Enumerable.Empty<Mask>();
    }

    public class Mask
    {
        [JsonProperty("paths")]
        public IEnumerable<string> Paths { get; set; } = Enumerable.Empty<string>();

        [JsonProperty("properties")]
        public IEnumerable<string> Properties { get; set; } = Enumerable.Empty<string>();

        [JsonProperty("maskString")]
        public string MaskString { get; set; } = "#########";
    }
}
