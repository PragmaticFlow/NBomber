using CSharpProd.DB.Redis;
using NBomber.CSharp;

namespace CSharpProd.HTTP.SimpleBookstore
{
    public class ExampleSimpleBookstore
    {
        public void Run()
        {
            NBomberRunner.RegisterScenarios(
                new InitSimpleBookstoreScenario().Create(),
                new TestSimpleBookstoreScenario().Create()
            )
            .LoadConfig("./HTTP/SimpleBookstore/config.json")
            .Run();
        }
    }
}
