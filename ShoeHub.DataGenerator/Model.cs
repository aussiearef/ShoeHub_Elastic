using Nest;
using System;

namespace ShoeHub.DataGenerator
{
    [ElasticsearchType(Name="shoehub")]
   public class Model
    {
        public string Metric { get; set; }
        public double  Value { get; set; }

        [Nest.Date(Format = "epoch_second")]
        public string TimeStamp => DateTime.Now.ToEpoch().ToString();
    }
}
