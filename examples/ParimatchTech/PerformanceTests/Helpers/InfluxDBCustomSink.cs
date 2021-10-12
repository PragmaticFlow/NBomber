using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using Serilog;

namespace PerformanceTests.Helpers
{
    [Serializable]
    public class InfluxDBCustomSink : IReportingSink
    {
        private ILogger _logger;
        private InfluxDBClient _influxdbClient;
        private IBaseContext _context;
        private Dictionary<string, string> _globalTags = new();

        public InfluxDBCustomSink(string url, string userName, string password, string databaseName)
        {
            _logger = null;
            _context = null;
            _influxdbClient =
                InfluxDBClientFactory.CreateV1(url, userName, password.ToCharArray(), databaseName, "autogen");
        }

        public Dictionary<string, string> GlobalMetricTags
        {
            get => _globalTags;
            set => _globalTags = value;
        }

        [SpecialName] public string SinkName => "NBomber.ParimatchTech.Sinks.InfluxDB";

        public Task Init(IBaseContext context, IConfiguration infraConfig)
        {
            _logger = context.Logger.ForContext<InfluxDBCustomSink>();
            _context = context;

            return Task.CompletedTask;
        }

        public Task Start() => Task.CompletedTask;

        public async Task SaveAnnotationToInfluxDBAsync(
            string annotationTitle,
            Dictionary<string, string> annotationTags,
            string text = "")
        {
            string tags = "";

            foreach (var tag in annotationTags)
                tags += $"{tag.Key}: {tag.Value},";

            tags = tags.Substring(0, tags.Length - 1);

            var point = PointData.Measurement("nbomber__annotations")
                .Tag("title", annotationTitle)
                .Tag("tags", tags)
                .Field("text", text);

            point = AddTags(point, new Dictionary<string, string>());

            await _influxdbClient.GetWriteApiAsync().WritePointAsync(point);
        }

        public async Task SaveValueToInfluxDBAsync(
            string measurement,
            Dictionary<string, string> customTags,
            string fieldKey,
            double fieldValue)
        {
            var point = PointData.Measurement(measurement)
                .Field(fieldKey, fieldValue);
            point = AddTags(point, customTags);

            await _influxdbClient.GetWriteApiAsync().WritePointAsync(point);
        }

        public async Task SaveValueToInfluxDBAsync(
            string measurement,
            Dictionary<string, string> customTags,
            string fieldKey,
            double fieldValue,
            DateTime timestamp)
        {
            var point = PointData.Measurement(measurement)
                .Field(fieldKey, fieldValue);
            point = AddTags(point, customTags);
            point = point.Timestamp(timestamp, WritePrecision.Ms);

            await _influxdbClient.GetWriteApiAsync().WritePointAsync(point);
        }

        internal string GetOperationName(OperationType operation)
        {
            switch (operation)
            {
                case OperationType.Bombing:
                    return "bombing";
                case OperationType.Complete:
                    return "complete";
                default:
                    return "bombing";
            }
        }

        public Task SaveRealtimeStats(ScenarioStats[] stats)
        {
            if (stats == null)
                throw new ArgumentNullException("Scenario stats is null");

            List<Task> scenarioTasks = new List<Task>();

            foreach (var s in stats)
                scenarioTasks.Add(SaveScenarioStats(s));

            return Task.WhenAll(scenarioTasks);
        }

        private Task SaveScenarioStats(ScenarioStats stats)
        {
            string operationName = GetOperationName(stats.CurrentOperation);
            string nodeType = _context.NodeInfo.NodeType.ToString();
            string scenarioName = stats.ScenarioName;
            TestInfo testInfo = _context.TestInfo;
            LoadSimulationStats loadSimulationStats = stats.LoadSimulationStats;
            StepStats[] stepStats = stats.StepStats; 

            if (stepStats == null)
                throw new ArgumentNullException("Step stats is null");

            List<Task> stepTasks = new List<Task>();
            foreach (var s in stepStats)
                stepTasks.Add(SaveStepStats(operationName, nodeType, testInfo, scenarioName, loadSimulationStats, s));

            return Task.WhenAll(stepTasks);
        }

        private Task SaveStepStats(
            string operationName,
            string nodeType,
            TestInfo testInfo,
            string scenarioName,
            LoadSimulationStats loadSimulationStats,
            StepStats stats)
        {
            string context = "nbomber";

            string measurementSimulationValue = $"{context}__simulation.value";

            string measurementOkRequestCount = $"{context}__ok.request.count";
            string measurementOkRequestRps = $"{context}__ok.request.rps";
            string measurementOkLatencyMax = $"{context}__ok.latency.max";
            string measurementOkLatencyMin = $"{context}__ok.latency.min";
            string measurementOkLatencyMean = $"{context}__ok.latency.mean";
            string measurementOkLatencyPercent50 = $"{context}__ok.latency.percent50";
            string measurementOkLatencyPercent75 = $"{context}__ok.latency.percent75";
            string measurementOkLatencyPercent95 = $"{context}__ok.latency.percent95";
            string measurementOkLatencyPercent99 = $"{context}__ok.latency.percent99";
            string measurementOkLatencyStddev = $"{context}__ok.latency.stddev";
            string measurementOkDataTransferAllbytes = $"{context}__ok.datatransfer.allbytes";
            string measurementOkDataTransferMax = $"{context}__ok.datatransfer.max";
            string measurementOkDataTransferMin = $"{context}__ok.datatransfer.min";
            string measurementOkDataTransferMean = $"{context}__ok.datatransfer.mean";

            string measurementFailRequestCount = $"{context}__fail.request.count";
            string measurementFailRequestRps = $"{context}__fail.request.rps";
            string measurementFailLatencyMax = $"{context}__fail.latency.max";
            string measurementFailLatencyMin = $"{context}__fail.latency.min";
            string measurementFailLatencyMean = $"{context}__fail.latency.mean";
            string measurementFailLatencyPercent50 = $"{context}__fail.latency.percent50";
            string measurementFailLatencyPercent75 = $"{context}__fail.latency.percent75";
            string measurementFailLatencyPercent95 = $"{context}__fail.latency.percent95";
            string measurementFailLatencyPercent99 = $"{context}__fail.latency.percent99";
            string measurementFailLatencyStddev = $"{context}__fail.latency.stddev";
            string measurementFailDataTransferAllbytes = $"{context}__fail.datatransfer.allbytes";
            string measurementFailDataTransferMax = $"{context}__fail.datatransfer.max";
            string measurementFailDataTransferMin = $"{context}__fail.datatransfer.min";
            string measurementFailDataTransferMean = $"{context}__fail.datatransfer.mean";

            string measurementAllRequestCount = $"{context}__all.request.count";
            string measurementAllDataTransferAllbytes = $"{context}__all.datatransfer.allbytes";

            var commonTags = new Dictionary<string, string>()
            {
                { "node_type", nodeType },
                { "operation", operationName },
                { "scenario", scenarioName },
                { "simulation.name", loadSimulationStats.SimulationName },
                { "step", stats.StepName }
            };

            var writeApiAsync = _influxdbClient.GetWriteApiAsync();
            List<Task> stepTasks = new List<Task>();

            stepTasks.Add(SaveOnePoint(measurementSimulationValue, writeApiAsync, commonTags, loadSimulationStats.Value));

            if (stats.Ok.Request.Count > 0)
            {
                stepTasks.Add(SaveOnePoint(measurementOkRequestCount, writeApiAsync, commonTags, stats.Ok.Request.Count));
                stepTasks.Add(SaveOnePoint(measurementOkRequestRps, writeApiAsync, commonTags, stats.Ok.Request.RPS));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyMax, writeApiAsync, commonTags, stats.Ok.Latency.MaxMs));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyMin, writeApiAsync, commonTags, stats.Ok.Latency.MinMs));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyMean, writeApiAsync, commonTags, stats.Ok.Latency.MeanMs));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyPercent50, writeApiAsync, commonTags, stats.Ok.Latency.Percent50));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyPercent75, writeApiAsync, commonTags, stats.Ok.Latency.Percent75));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyPercent95, writeApiAsync, commonTags, stats.Ok.Latency.Percent95));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyPercent99, writeApiAsync, commonTags, stats.Ok.Latency.Percent99));
                stepTasks.Add(SaveOnePoint(measurementOkLatencyStddev, writeApiAsync, commonTags, stats.Ok.Latency.StdDev));
                stepTasks.Add(SaveOnePoint(measurementOkDataTransferAllbytes, writeApiAsync, commonTags, stats.Ok.DataTransfer.AllBytes));
                stepTasks.Add(SaveOnePoint(measurementOkDataTransferMax, writeApiAsync, commonTags, stats.Ok.DataTransfer.MaxBytes));
                stepTasks.Add(SaveOnePoint(measurementOkDataTransferMin, writeApiAsync, commonTags, stats.Ok.DataTransfer.MinBytes));
                stepTasks.Add(SaveOnePoint(measurementOkDataTransferMean, writeApiAsync, commonTags, stats.Ok.DataTransfer.MeanBytes));
            }

            if (stats.Fail.Request.Count > 0)
            {
                stepTasks.Add(SaveOnePoint(measurementFailRequestCount, writeApiAsync, commonTags, stats.Fail.Request.Count));
                stepTasks.Add(SaveOnePoint(measurementFailRequestRps, writeApiAsync, commonTags, stats.Fail.Request.RPS));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyMax, writeApiAsync, commonTags, stats.Fail.Latency.MaxMs));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyMin, writeApiAsync, commonTags, stats.Fail.Latency.MinMs));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyMean, writeApiAsync, commonTags, stats.Fail.Latency.MeanMs));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyPercent50, writeApiAsync, commonTags, stats.Fail.Latency.Percent50));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyPercent75, writeApiAsync, commonTags, stats.Fail.Latency.Percent75));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyPercent95, writeApiAsync, commonTags, stats.Fail.Latency.Percent95));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyPercent99, writeApiAsync, commonTags, stats.Fail.Latency.Percent99));
                stepTasks.Add(SaveOnePoint(measurementFailLatencyStddev, writeApiAsync, commonTags, stats.Fail.Latency.StdDev));
                stepTasks.Add(SaveOnePoint(measurementFailDataTransferAllbytes, writeApiAsync, commonTags, stats.Fail.DataTransfer.AllBytes));
                stepTasks.Add(SaveOnePoint(measurementFailDataTransferMax, writeApiAsync, commonTags, stats.Fail.DataTransfer.MaxBytes));
                stepTasks.Add(SaveOnePoint(measurementFailDataTransferMin, writeApiAsync, commonTags, stats.Fail.DataTransfer.MinBytes));
                stepTasks.Add(SaveOnePoint(measurementFailDataTransferMean, writeApiAsync, commonTags, stats.Fail.DataTransfer.MeanBytes));
            }

            stepTasks.Add(SaveOnePoint(measurementAllRequestCount, writeApiAsync, commonTags, stats.Ok.Request.Count + stats.Fail.Request.Count));
            stepTasks.Add(SaveOnePoint(measurementAllDataTransferAllbytes, writeApiAsync, commonTags, stats.Ok.DataTransfer.AllBytes + stats.Fail.DataTransfer.AllBytes));

            return Task.WhenAll(stepTasks);
        }

        private Task SaveOnePoint(
            string measurement,
            WriteApiAsync writeApiAsync,
            Dictionary<string, string> commonTags,
            double fieldValue,
            string fieldKey = "value")
        {
            var point = PointData.Measurement(measurement);
            point = AddTags(point, commonTags);
            point = point.Field(fieldKey, fieldValue);

            return writeApiAsync.WritePointAsync(point);
        }

        private PointData AddTags(PointData point, Dictionary<string, string> commonTags)
        {
            foreach (var tag in _globalTags)
                point = point.Tag(tag.Key, tag.Value);

            foreach (var tag in commonTags)
                point = point.Tag(tag.Key, tag.Value);

            return point;
        }

        public Task SaveFinalStats(NodeStats[] stats)
        {
            List<Task> nodeTasks = new List<Task>();

            foreach (var s in stats)
            {
                nodeTasks.Add(SaveFinalNodeStats(s));
            }

            return Task.WhenAll(nodeTasks);
        }

        private Task SaveFinalNodeStats(NodeStats stats)
        {
            List<Task> scenarioTasks = new List<Task>();

            foreach (var s in stats.ScenarioStats)
            {
                scenarioTasks.Add(SaveFinalScenarioStats(s, stats.NodeInfo.CurrentOperation));
            }

            return Task.WhenAll(scenarioTasks);
        }

        private Task SaveFinalScenarioStats(ScenarioStats stats, OperationType operationType)
        {
            List<Task> stepTasks = new List<Task>();

            foreach (var s in stats.StepStats)
            {
                stepTasks.Add(SaveFinalStepStats(s, operationType, stats.ScenarioName));
            }

            return Task.WhenAll(stepTasks);
        }

        private Task SaveFinalStepStats(StepStats stats, OperationType operationType, string scenarioName)
        {
            string context = "nbomber";

            string measurementOkSummary = $"{context}__ok.summary";
            string measurementFailSummary = $"{context}__fail.summary";

            var commonTags = new Dictionary<string, string>()
            {
                { "node_type", _context.NodeInfo.NodeType.ToString() },
                { "operation", GetOperationName(operationType) },
                { "scenario", scenarioName },
                { "step", stats.StepName },
                { "stat_type", "default" }
            };

            var writeApiAsync = _influxdbClient.GetWriteApiAsync();
            List<Task> stepTasks = new List<Task>();

            if (stats.Ok.Request.Count > 0)
            {
                commonTags["stat_type"] = "request";
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Request.Count, "count"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Request.Count + stats.Fail.Request.Count, "all"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Request.RPS, "rps"));

                commonTags["stat_type"] = "latency";
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.MaxMs, "max"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.MinMs, "min"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.MeanMs, "mean"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.StdDev, "stddev"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.Percent50, "percent50"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.Percent75, "percent75"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.Percent95, "percent95"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.Latency.Percent99, "percent99"));

                commonTags["stat_type"] = "datatransfer";
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.DataTransfer.MinBytes, "min"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.DataTransfer.MaxBytes, "max"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.DataTransfer.MeanBytes, "mean"));
                stepTasks.Add(SaveOnePoint(measurementOkSummary, writeApiAsync, commonTags, stats.Ok.DataTransfer.AllBytes, "allbytes"));
            }

            if (stats.Fail.Request.Count > 0)
            {
                commonTags["stat_type"] = "request";
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Request.Count, "count"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Ok.Request.Count + stats.Fail.Request.Count, "all"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Request.RPS, "rps"));

                commonTags["stat_type"] = "latency";
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.MaxMs, "max"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.MinMs, "min"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.MeanMs, "mean"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.StdDev, "stddev"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.Percent50, "percent50"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.Percent75, "percent75"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.Percent95, "percent95"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.Latency.Percent99, "percent99"));

                commonTags["stat_type"] = "datatransfer";
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.DataTransfer.MinBytes, "min"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.DataTransfer.MaxBytes, "max"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.DataTransfer.MeanBytes, "mean"));
                stepTasks.Add(SaveOnePoint(measurementFailSummary, writeApiAsync, commonTags, stats.Fail.DataTransfer.AllBytes, "allbytes"));
            }

            return Task.WhenAll(stepTasks);
        }

        public Task Stop() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }
}