namespace GrpcGreeterClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new GrpcGreeterClient("https://localhost:7117");
            var randomId = 1;

            client.SendRandomData(randomId);
            client.GetData(randomId);
            await client.SendRandomDataStream();
            await client.GetDataStream();
            await client.SendAndGetDataStream();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
