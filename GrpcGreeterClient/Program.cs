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

            var randomBytes = Data.GenerateRandomBytes(10);
            var data = ByteString.CopyFrom(randomBytes);
            var sendDataReply = client.SendData(
                new SendDataRequest { Data = data }
            );
            Console.WriteLine($"Data sending status: {sendDataReply.SendDataStatus}");

            var readDataReply = client.ReadData(
                new ReadDataRequest()
            );
            Console.WriteLine($"Sent data: {String.Join(", ", data)}");
            Console.WriteLine($"Read data: {String.Join(", ", readDataReply.Data)}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
