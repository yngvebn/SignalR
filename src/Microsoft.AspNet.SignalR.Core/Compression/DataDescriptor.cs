using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNet.SignalR.Compression
{
    public class DataDescriptor
    {
        public virtual string Name { get; set; }

        [JsonIgnore]
        public Type Type { get; set; }

        [JsonIgnore]
        public Action<object, object> SetValue { get; set; }

        [JsonIgnore]
        public Func<object, object> GetValue { get; set; }
    }
}
