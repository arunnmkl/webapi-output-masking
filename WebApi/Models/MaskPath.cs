using System.Collections.Generic;

namespace WebApi.Models
{
    public class MaskPath
    {
        public MaskPath(IEnumerable<string> properties, string path, string maskString)
        {
            this.Properties = properties;
            this.Path = path;
            this.MaskString = maskString;
        }

        public string Path { get; set; }

        public IEnumerable<string> Properties { get; set; }

        public string MaskString { get; set; }
    }
}
