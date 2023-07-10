using NBomber.CSharp;

namespace CSharpProd.HTTP.WebAppSimulator
{
    public class WebAppSimulatorExample
    {
        public void RunHttpUserExample()
        {
            NBomberRunner.RegisterScenarios(
                new InitHttpScenario().Create(),
                new ReadHttpScenario().Create(),
                new WriteHttpScenario().Create()
            )
            .LoadConfig("./HTTP/WebAppSimulator/config.json")
            .Run();
        }
    }
}
