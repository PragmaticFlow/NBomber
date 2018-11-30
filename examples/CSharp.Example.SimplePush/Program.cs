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
            var updatesChannel = GlobalUpdatesChannel.Instance;

            var step1 = Step.CreatePull("publish", async req =>
            {
                var clientId = req.CorrelationId;
                var message = $"Hi Server from client: {clientId}";

                server.Publish(clientId, message);
                return Response.Ok();
            });

            var step2 = Step.CreatePush("update from server");

            server.Notify += (s, pushNotification) =>
            {
                updatesChannel.ReceivedUpdate(
                    correlationId: pushNotification.ClientId,
                    pushStepName: step2.Name,
                    update: Response.Ok(pushNotification.Message)
                );
            };

            return ScenarioBuilder.CreateScenario("PushScenario", step1, step2)
                                  .WithConcurrentCopies(1)
                                  .WithDuration(TimeSpan.FromSeconds(3));
        }

        static void Main(string[] args)
        {
            var scenario = BuildScenario();
            NBomberRunner.RegisterScenarios(scenario)
                         .RunInConsole();

        }
    }
}
