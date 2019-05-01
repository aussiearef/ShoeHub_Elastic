using System;
using System.Threading;

namespace ShoeHub.DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            const string IndexName = "shoehub";

            Console.Clear();
            Console.WriteLine("Grafana, Graphite and StatsD: From Beginner To Advance! \n");
            Console.WriteLine("This utility program will populate randomly generated data to your Elasticsearch instance!");
            Console.WriteLine("The generated data belong sot an imaginary company called Shoe Hub\n");
            Console.WriteLine($"The Index will be created automatically. It is called '{IndexName}'");
            getIpAddress:

            Console.Write("Enter the URL of your Elasticsearch instance (including the port number) e.g. http://localhost:9200: ");
            var ipAddress = Console.ReadLine();

            if (string.IsNullOrEmpty(ipAddress))
            {
                Console.WriteLine("Invalid ES address. Try again, or press Ctrl-C to stop!");
                goto getIpAddress;
            }


            getNumberOfDataPoints:

            Console.Write($"Please enter the number of data points from 1 to ({(int.MaxValue - 1).ToString("###,###")}): ");
            var dataPointsCount = 0;
            var dataPointsCountStr = Console.ReadLine();
            if (!int.TryParse(dataPointsCountStr, out dataPointsCount))
            {
                Console.WriteLine("The value you entered is not valid. Please try again");
                goto getNumberOfDataPoints;
            }

            var esClient = new Nest.ElasticClient(new Uri(ipAddress));

            const short Refund = 0;
            var countryCodes = new[]{"AU","US","IN"};
            var paymentMethods = new[] {"Card","Cash","Paypal"};
            var shoeTypes = new[] {"Loafers","Boots","HighHeels"};

            var randomGenerator = new Random(DateTime.Now.Millisecond);

            for (int i = 1; i <= dataPointsCount; i++)
            {
                Console.WriteLine("Sending metrics to server...");

                

                var shoeType = shoeTypes[randomGenerator.Next(shoeTypes.Length)];
                var salesBucketName = $"shoehub.sales.{shoeType}";
                var salesBucket = new Model {Metric=salesBucketName,Value=1};

                var res= esClient.Index(salesBucket, x => x.Index(IndexName));
                if (res.ServerError != null)
                {
                    throw new Exception(res.ServerError.Error.Reason);
                }

                var countryCode = countryCodes[randomGenerator.Next(countryCodes.Length)];
                var paymentOrRefund = randomGenerator.Next(1);

                if (paymentOrRefund == Refund)
                {
                    var refundBucketName = $"shoehub.{countryCode}.refunds";
                    var refundValue = randomGenerator.Next(1000);

                    var refundModel = new Model { Metric = refundBucketName, Value = refundValue };
                    esClient.Index(refundModel, x => x.Index(IndexName));
                }
                else
                {
                    var paymentMethod = paymentMethods[randomGenerator.Next(paymentMethods.Length)];
                    var paymentMethodBucketName = $"shoehub.{countryCode}.payments.{paymentMethod}";
                    var paymentValue = randomGenerator.Next(1000);
                    var paymentModel = new Model {Metric= paymentMethodBucketName , Value=paymentValue };
                }
                Thread.Sleep(new TimeSpan(0,0,0, randomGenerator.Next(60)));
            }

            Console.WriteLine("All datapoints were sent to Elasticsearch. Press any keys...");
            Console.ReadKey();
        }
    }
}
