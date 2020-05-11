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
            HelloWorldScenario.Run(args);
            //new AdvancedHelloWorldScenario().Run(args);
            //DataFeedScenario.Run(args);
            //HttpScenario.Run(args);
            //MongoDbScenario.Run(args);
            //WebSocketsScenario.Run(args);
            //RealtimeStatistics.Run(args);
        }
    }
}
