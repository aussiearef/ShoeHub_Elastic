using Nest;
using System;

namespace ShoeHub.DataGenerator
{
    public class Model
    {
        public string Metric { get; set; }
        public double  Value { get; set; }

        [Nest.Date(Format = "epoch_millis")]
        public int TimeStamp => DateTime.Now.ToEpoch();
    }
}
