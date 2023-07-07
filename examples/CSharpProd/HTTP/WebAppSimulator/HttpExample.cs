using NBomber.CSharp;

namespace CSharpProd.HTTP.WebAppSimulator
{
    public class HttpExample
    {
        public void Run()
        {
            NBomberRunner.RegisterScenarios(
                new InitHTTPScenario().Create(),
                new ReadHttpScenario().Create(),
                new WriteHttpScenario().Create()
            )
            .LoadConfig("./HTTP/WebAppSimulator/config.json")
            .Run();
        }
    }
}
