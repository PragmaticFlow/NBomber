using System;
using System.Collections.Generic;
using System.Timers;

using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Example.SimplePush
{
    class Program
    {
        class PushNotification
        {
            public string ClientId { get; set; }
            public string Message { get; set; }
        }

        class FakePushServer
        {
            readonly Dictionary<string, Timer> _subscribers = new Dictionary<string, Timer>();

            public event EventHandler<PushNotification> Notify;

            public void Publish(string clientId, string message)
            {
                var timer = new Timer(500);
                timer.Elapsed += (s, e) =>
                {
                    var msg = new PushNotification
                    {
                        ClientId = clientId,
                        Message = $"Hi Client {clientId} from server"
                    };
                    Notify(this, msg);
                    timer.Stop();
                };
                timer.Start();

                _subscribers[clientId] = timer;
            }
        }

        static Scenario BuildScenario()
        {
            var server = new FakePushServer();

            var step1 = Step.CreateRequest("publish", async req =>
            {
                var clientId = req.CorrelationId;
                var message = $"Hi Server from client: {clientId}";

                server.Publish(clientId, message);
                return Response.Ok();
            });

            var listenerChannel = Step.CreateListenerChannel();
            server.Notify += (s, pushNotification) =>
            {
                listenerChannel.Notify(correlationId: pushNotification.ClientId,
                                       response: Response.Ok(pushNotification.Message));
            };

            var step2 = Step.CreateListener("listen", listenerChannel);

            return new ScenarioBuilder(scenarioName: "PushScenario")
                .AddTestFlow("test push ", steps: new[] { step1, step2 }, concurrentCopies: 1)
                .Build(duration: TimeSpan.FromSeconds(10));
        }

        static void Main(string[] args)
        {
            var scenario = BuildScenario();
            scenario.RunInConsole();
        }
    }
}
