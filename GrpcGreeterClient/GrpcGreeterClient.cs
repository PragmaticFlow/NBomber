using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using NBomber.Data;

namespace GrpcGreeterClient
{
    class GrpcGreeterClient
    {
        private readonly Greeter.GreeterClient _client;

        public GrpcGreeterClient(string address)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7117");
            _client = new Greeter.GreeterClient(channel);
        }

        public void SendRandomData(int recordId)
        {
            var randomBytes = Data.GenerateRandomBytes(10);
            var randomData = ByteString.CopyFrom(randomBytes);
            var sendDataReply = _client.SendData(
                new SendDataRequest { RecordId = recordId, Data = randomData }
            );

            Console.WriteLine($"Data sent: {String.Join(", ", randomData)}.");
            Console.WriteLine($"Data sending status: {sendDataReply.SendDataStatus}.");
            Console.WriteLine();
        }

        public void GetData(int recordId)
        {
            var getDataReply = _client.GetData(
                new GetDataRequest { RecordId = recordId }
            );
            
            Console.WriteLine($"Data get: {String.Join(", ", getDataReply.Data)}.");
            Console.WriteLine();
        }

        public async Task SendRandomDataStream()
        {
            try
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                using AsyncClientStreamingCall<SendDataRequest, SendDataReply> clientStreamingCall =
                    _client.SendDataStream(cancellationToken: cancellationToken.Token);

                var i = 0;
                while (i < 10)
                {
                    var randomBytes = Data.GenerateRandomBytes(10);
                    var randomData = ByteString.CopyFrom(randomBytes);

                    await clientStreamingCall.RequestStream.WriteAsync(
                        new SendDataRequest { RecordId = i, Data = randomData });
                    Console.WriteLine($"Data sent: {String.Join(", ", randomData)}.");
                    i++;
                }

                await clientStreamingCall.RequestStream.CompleteAsync();
                Console.WriteLine("Client streaming completed.");
                Console.WriteLine();
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Client stream cancelled.");
                Console.WriteLine();
            }
        }

        public async Task GetDataStream()
        {
            try
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                using var streamingCall = _client.GetDataStream(new Empty(),
                    cancellationToken: cancellationToken.Token);

                await foreach (var data in streamingCall.ResponseStream
                    .ReadAllAsync(cancellationToken: cancellationToken.Token))
                {
                    Console.WriteLine($"Data received: {String.Join(", ", data.Data)}.");
                }

                Console.WriteLine("Server streaming completed.");
                Console.WriteLine();
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Server stream cancelled.");
                Console.WriteLine();
            }
        }

        public async Task SendAndGetDataStream()
        {
            try
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                using AsyncDuplexStreamingCall<SendDataRequest, GetDataReply> duplexStreamingCall =
                    _client.SendAndGetDataStream(cancellationToken: cancellationToken.Token);

                var i = 0;
                var task = Task.WhenAll(
                [
                    Task.Run(async () =>
                    {
                        while (i < 10)
                        {
                            var randomBytes = Data.GenerateRandomBytes(10);
                            var randomData = ByteString.CopyFrom(randomBytes);

                            await duplexStreamingCall.RequestStream.WriteAsync(
                                new SendDataRequest { RecordId = i, Data = randomData });
                            Console.WriteLine($"Data sent: {String.Join(", ", randomData )}.");
                            i++;
                        }

                        await duplexStreamingCall.RequestStream.CompleteAsync();
                        Console.WriteLine("Client streaming completed.");
                    }),
                    Task.Run(async () =>
                    {
                        while(!cancellationToken.IsCancellationRequested &&
                            await duplexStreamingCall.ResponseStream.MoveNext())
                        {
                            Console.WriteLine($"Data received: {String.Join(", ", duplexStreamingCall.ResponseStream.Current.Data)}.");
                        }
                        Console.WriteLine("Server streaming completed.");
                    })
                ]);

                try
                {
                    task.Wait(cancellationToken.Token);
                    Console.WriteLine();
                }
                catch (OperationCanceledException e)
                {
                    await duplexStreamingCall.RequestStream.CompleteAsync();
                    Thread.Sleep(6000);
                    Console.WriteLine();
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Set and get stream cancelled.");
                Console.WriteLine();
            }
        }
    }
}
