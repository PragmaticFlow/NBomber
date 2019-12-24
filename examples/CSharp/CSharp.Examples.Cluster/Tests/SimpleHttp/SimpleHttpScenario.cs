// using NBomber.Contracts;
// using NBomber.CSharp;
// using NBomber.Http.CSharp;
//
// namespace CSharp.Examples.Cluster.Tests.SimpleHttp
// {
//     public class SimpleHttpScenario
//     {
//         public static Scenario Create()
//         {
//             var step = HttpStep.Create("pull home page", (context) =>
//                 Http.CreateRequest("GET", "https://nbomber.com")
//                     .WithHeader("Accept", "text/html")
//             );
//
//             return ScenarioBuilder.CreateScenario("simple http scenario", new[] { step });
//         }
//     }
// }