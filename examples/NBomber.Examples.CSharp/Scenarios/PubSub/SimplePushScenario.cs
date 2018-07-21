using System;
using System.Timers;

namespace NBomber.Examples.CSharp.Scenarios.PubSub
{
    class FakeService
    {
        readonly Timer _timer = new Timer(500);        
        public event EventHandler<string> PushMessageEvent;

        public FakeService()
        {
            _timer.Elapsed += (s, e) =>
            {
                PushMessageEvent(this, "Hi Client");
            };
        }

        public void Send(string msg) => _timer.Start();
        public void Stop() => _timer.Stop();
    }

    class SimplePushScenario
    {
        public static Scenario BuildScenario()
        {
            var service = new FakeService();

            var step1 = Step.CreateRequest("publish", async _ =>
            {
                service.Send("Hi Server");
                return Response.Ok();
            });

            var listeners = new StepListeners();
            service.PushMessageEvent += (s, msg) =>
            {
                service.Stop();
                listeners.Notify(flowId: 0, response: Response.Ok());
            };

            var step2 = Step.CreateListener("listen", listeners);

            return new ScenarioBuilder(scenarioName: nameof(SimplePushScenario))
                .AddTestFlow("test push ", steps: new[] { step1, step2 }, concurrentCopies: 1)
                .Build(duration: TimeSpan.FromSeconds(10));
        }
    }
}
