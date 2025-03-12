using Google.Protobuf;
using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:7117");
            var client = new Greeter.GreeterClient(channel);

            var data = ByteString.CopyFrom([0xAA, 0xBB, 0xCC]);
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
