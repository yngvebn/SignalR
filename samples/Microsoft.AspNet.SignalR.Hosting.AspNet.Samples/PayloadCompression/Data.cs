using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Compression;

namespace Microsoft.AspNet.SignalR.Samples
{
    [Payload]
    public class Data
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public long Value { get; set; }
        public BigData Sibling { get; set; }

        public double Foo;
        public Double Bar;

        public decimal FooD;
        public Decimal BarD;

        public Data Child { get; set; }
    }
}