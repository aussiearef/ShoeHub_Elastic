using System;

namespace ShoeHub.DataGenerator
{
    public static class Extensions
    {
        public static long ToEpoch(this DateTime instance)
        {
            var offset = new DateTimeOffset(DateTime.Now);
            return offset.ToUnixTimeSeconds();
        }
    }
}
