using NBomber.CSharp;

namespace Demo.DB.MongoDB
{
    public class MongoDBExample
    {
        public void Run()
        {
            var initScenario = new MongoInitScenario().Create();
            var readScenario = new MongoReadScenario().Create();
            var writeScenario = new MongoWriteScenario().Create();

            NBomberRunner.RegisterScenarios(initScenario, readScenario, writeScenario)
                .WithoutReports()
                .LoadConfig("./DB/MongoDB/config.json")
                .Run();
        }
    }
}
