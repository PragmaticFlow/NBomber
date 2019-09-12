using CSharp.Examples.Cluster.Scenarios;

namespace CSharp.Examples.Cluster
{
    class Program
    {
        static void Main(string[] args)
        {
            var configPath = args[0]; // agent_config.json or coordinator_config.json
            HttpScenario.Run(configPath);
        }
    }
}