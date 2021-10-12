namespace PerformanceTests.Settings
{
    public class ConfigModel
    {
        public Kafka Kafka { get; set; }
        public Influxdb InfluxDB { get; set; }
        public LoggerSerilog LoggerSerilog { get; set; }
        public TestRunSettings TestRunSettings { get; set; }
    }

    public class Kafka
    {
        public string Broker { get; set; }
        public string Awesome_Topic { get; set; }
        public string Metrics_Topic { get; set; }
        public string Metrics_Node_Topic { get; set; }
    }

    public class Influxdb
    {
        public string Url { get; set; }
        public string DataBaseName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class LoggerSerilog
    {
        public string NodeUris { get; set; }
        public string IndexFormat { get; set; }
    }

    public class TestRunSettings
    {
        public int HttpUsers { get; set; }
        public int KafkaUsers { get; set; }
        public int GuidCountsToCreate { get; set; }
        public int TestDurationSeconds { get; set; }
        public int RampUpSeconds { get; set; }
        public int PauseMs { get; set; }
        public string SimpleAppUrl { get; set; }
        public ReportingSettings ReportingSettings { get; set; }
    }

    public class ReportingSettings
    {
        public string Pipeline { get; set; }
        public string Build_SimpleWebApp { get; set; }
        public string Config_SimpleWebApp { get; set; }
        public string Test_Name { get; set; }
        public string Test_Suite { get; set; }
    }
}