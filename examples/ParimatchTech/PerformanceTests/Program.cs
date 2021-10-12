using PerformanceTests.Examples;
using System.Threading.Tasks;

namespace PerformanceTests
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await RealTimeReportingExample.Run();
        }
    }
}