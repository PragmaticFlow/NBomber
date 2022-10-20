using System;
using CSharpDev.DataFeed;
using CSharpDev.Features;
using CSharpDev.HelloWorld;
using CSharpDev.HttpTests;
using CSharpDev.Mqtt;
using NBomber.CSharp;

namespace CSharpDev
{
    class Program
    {
        static void Main(string[] args)
        {
            // doNotTrack = true
            // statusCode = string
            // stepTimeout - https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/cancel-async-tasks-after-a-period-of-time
            // scenarioTimeOut
            // disableHintAnalizer on data transfer

            SimpleHttpTest.Run();
            //new MqttScenario().Run();

            //HelloWorldExample.Run();
            // ClientDistributionExample.Run();
            // CustomStepExecControlExample.Run();
            // DataFeedTest.Run();
            // CustomReportingExample.Run();
            // CustomSettingsExample.Run();
        }
    }
}
