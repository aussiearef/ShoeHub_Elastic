using System;

namespace ShoeHub.DataGenerator
{
    public static class Extensions
    {
        public static int ToEpoch(this DateTime instance)
        {
            TimeSpan t = instance - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}
