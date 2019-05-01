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

            Console.Write("Enter the URL of your Elasticsearch instance (including the port number) e.g. http://localhost:9200 (Enter to use default): ");
            var ipAddress = Console.ReadLine();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = "http://localhost:9200";
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

            var connSettings = new Nest.ConnectionSettings(new Uri(ipAddress)).DefaultIndex(IndexName);
            var esClient = new Nest.ElasticClient(connSettings);

            if (!esClient.IndexExists(IndexName).Exists)
            {
                //esClient.CreateIndex(IndexName,m=>m.Map<Model>(map=>map.Properties(p=>p.Date(d=>d.Name(n=>n.TimeStamp).Format("epoch_second")).
                //Text(t=>t.Name(n=>n.Metric)).Number(n=>n.Name(nn=>nn.Value).Type(Nest.NumberType.Double)

                //))));

                esClient.CreateIndex(IndexName, m =>m.Mappings(mm=>mm.Map<Model>(model=>model.AutoMap())));
            }

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
                var salesModel = new Model {Metric=salesBucketName,Value=1};

                var res= esClient.IndexDocument<Model>(salesModel);
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
                    esClient.IndexDocument(refundModel);
                }
                else
                {
                    var paymentMethod = paymentMethods[randomGenerator.Next(paymentMethods.Length)];
                    var paymentMethodBucketName = $"shoehub.{countryCode}.payments.{paymentMethod}";
                    var paymentValue = randomGenerator.Next(1000);
                    var paymentModel = new Model {Metric= paymentMethodBucketName , Value=paymentValue };
                    esClient.IndexDocument(paymentModel);
                }
                Thread.Sleep(new TimeSpan(0,0,0, randomGenerator.Next(10)));
            }

            Console.WriteLine("All datapoints were sent to Elasticsearch. Press any keys...");
            Console.ReadKey();
        }
    }
}
