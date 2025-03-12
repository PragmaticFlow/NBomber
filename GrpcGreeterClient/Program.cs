using Google.Protobuf;
using Grpc.Net.Client;
using NBomber.Data;

namespace GrpcGreeterClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:7117");
            var client = new Greeter.GreeterClient(channel);

            var random = new Random();
            var randomId = random.Next(1, Int32.MaxValue);
            var randomBytes = Data.GenerateRandomBytes(10);
            var randomData = ByteString.CopyFrom(randomBytes);
            var sendDataReply = client.SendData(
                new SendDataRequest { RecordId = randomId, Data = randomData }
            );
            Console.WriteLine($"Data sending status: {sendDataReply.SendDataStatus}");

            var readDataReply = client.ReadData(
                new ReadDataRequest { RecordId = randomId }
            );
            Console.WriteLine($"Sent data: { String.Join(", ", randomData) }");
            Console.WriteLine($"Read data: { String.Join(", ", readDataReply.Data) }");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
