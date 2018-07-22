using System;
using System.Collections.Generic;
using System.Timers;
using NBomber.CSharp;

namespace NBomber.Examples.CSharp.Scenarios.PubSub
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

    class SimplePushScenario
    {
        public static Scenario BuildScenario()
        {
            var server = new FakePushServer();            

            var step1 = StepFactory.CreateRequest("publish", async req =>
            {
                var clientId = req.CorrelationId;
                var message = $"Hi Server from client: {clientId}";

                server.Publish(clientId, message);
                return Response.Ok();
            });

            var listeners = new StepListeners();
            server.Notify += (s, pushNotification) =>
            {
                listeners.Notify(correlationId: pushNotification.ClientId,
                                 response: Response.Ok(pushNotification.Message));
            };            

            var step2 = StepFactory.CreateListener("listen", listeners);

            return new ScenarioBuilder(scenarioName: nameof(SimplePushScenario))
                .AddTestFlow("test push ", steps: new[] { step1, step2 }, concurrentCopies: 1)
                .Build(duration: TimeSpan.FromSeconds(10));
        }
    }
}
