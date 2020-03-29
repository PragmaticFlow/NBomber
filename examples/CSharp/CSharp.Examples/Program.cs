using System;
using NBomber.Contracts;
using NBomber.CSharp;
using CSharp.Examples.Scenarios;

namespace CSharp.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            HelloWorldScenario.Run();
            //new AdvancedHelloWorldScenario().Run();
            //DataFeedScenario.Run();
            //HttpScenario.Run();
            //MongoDbScenario.Run();
            //WebSocketsScenario.Run();
            //RealtimeStatistics.Run();
        }
    }
}
