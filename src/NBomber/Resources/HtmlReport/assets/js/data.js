const viewModel = {
    "NBomberInfo":{
       "NBomberVersion":"1.1.0"
    },
    "TestInfo":{
       "TestSuite":"nbomber_default_test_suite_name",
       "TestName":"nbomber_default_test_name"
    },
    "StatsData":{
       "RequestCount":128,
       "OkCount":106,
       "FailCount":22,
       "AllDataMB":0.0,
       "ScenarioStats":[
          {
             "ScenarioName":"scenario 1",
             "RequestCount":128,
             "OkCount":106,
             "FailCount":22,
             "AllDataMB":0.0,
             "StepStats":[
                {
                   "StepName":"pull html 1",
                   "RequestCount":128,
                   "OkCount":106,
                   "FailCount":22,
                   "Min":34,
                   "Mean":815,
                   "Max":1501,
                   "RPS":3,
                   "Percent50":793,
                   "Percent75":1219,
                   "Percent95":1455,
                   "Percent99":1484,
                   "StdDev":409,
                   "MinDataKb":0.0,
                   "MeanDataKb":0.0,
                   "MaxDataKb":0.0,
                   "AllDataMB":0.0,
                   "ErrorStats":[
                      {
                         "ErrorCode":0,
                         "Message":"unknown client's error",
                         "Count":22
                      }
                   ]
                }
             ],
             "LatencyCount":{
                "Less800":53,
                "More800Less1200":25,
                "More1200":28
             },
             "LoadSimulationStats":{
                "SimulationName":"inject_per_sec",
                "Value":10
             },
             "ErrorStats":[
                {
                   "ErrorCode":0,
                   "Message":"unknown client's error",
                   "Count":22
                }
             ],
             "Duration":"00:00:20"
          }
       ],
       "PluginStats":[
          {
             "TableName":"CustomPlugin1",
             "Columns":[
                "Property",
                "Value"
             ],
             "Rows":[
                [
                   "Property1",
                   "7281496"
                ],
                [
                   "Property2",
                   "669353760"
                ],
                [
                   "Property3",
                   "1044407343"
                ]
             ]
          }
       ],
       "NodeInfo":{
          "MachineName":"DESKTOP-DMNJHK6",
          "NodeType":{
             "Case":"SingleNode"
          },
          "CurrentOperation":5,
          "OS":{
             "Platform":2,
             "ServicePack":"",
             "Version":{
                "Major":10,
                "Minor":0,
                "Build":18363,
                "Revision":0,
                "MajorRevision":0,
                "MinorRevision":0
             },
             "VersionString":"Microsoft Windows NT 10.0.18363.0"
          },
          "DotNetVersion":".NETCoreApp,Version=v3.1",
          "Processor":"Intel64 Family 6 Model 94 Stepping 3, GenuineIntel",
          "CoresCount":8,
          "NBomberVersion":"1.1.0"
       }
    },
    "TimeLineStatsData":{
       "TimeStamps":[
          "00:00:05",
          "00:00:10",
          "00:00:15",
          "00:00:20"
       ],
       "ScenarioStats":[
          [
             {
                "ScenarioName":"scenario 1",
                "RequestCount":5,
                "OkCount":3,
                "FailCount":2,
                "AllDataMB":0.0,
                "StepStats":[
                   {
                      "StepName":"pull html 1",
                      "RequestCount":5,
                      "OkCount":3,
                      "FailCount":2,
                      "Min":34,
                      "Mean":673,
                      "Max":1455,
                      "RPS":0,
                      "Percent50":531,
                      "Percent75":531,
                      "Percent95":1455,
                      "Percent99":1455,
                      "StdDev":589,
                      "MinDataKb":0.0,
                      "MeanDataKb":0.0,
                      "MaxDataKb":0.0,
                      "AllDataMB":0.0,
                      "ErrorStats":[
                         {
                            "ErrorCode":0,
                            "Message":"unknown client's error",
                            "Count":2
                         }
                      ]
                   }
                ],
                "LatencyCount":{
                   "Less800":2,
                   "More800Less1200":0,
                   "More1200":1
                },
                "LoadSimulationStats":{
                   "SimulationName":"ramp_constant",
                   "Value":2
                },
                "ErrorStats":[
                   {
                      "ErrorCode":0,
                      "Message":"unknown client's error",
                      "Count":2
                   }
                ],
                "Duration":"00:00:05.0067076"
             }
          ],
          [
             {
                "ScenarioName":"scenario 1",
                "RequestCount":16,
                "OkCount":15,
                "FailCount":1,
                "AllDataMB":0.0,
                "StepStats":[
                   {
                      "StepName":"pull html 1",
                      "RequestCount":21,
                      "OkCount":18,
                      "FailCount":3,
                      "Min":34,
                      "Mean":877,
                      "Max":1455,
                      "RPS":0,
                      "Percent50":861,
                      "Percent75":1277,
                      "Percent95":1293,
                      "Percent99":1455,
                      "StdDev":408,
                      "MinDataKb":0.0,
                      "MeanDataKb":0.0,
                      "MaxDataKb":0.0,
                      "AllDataMB":0.0,
                      "ErrorStats":[
                         {
                            "ErrorCode":0,
                            "Message":"unknown client's error",
                            "Count":3
                         }
                      ]
                   }
                ],
                "LatencyCount":{
                   "Less800":4,
                   "More800Less1200":6,
                   "More1200":5
                },
                "LoadSimulationStats":{
                   "SimulationName":"ramp_constant",
                   "Value":5
                },
                "ErrorStats":[
                   {
                      "ErrorCode":0,
                      "Message":"unknown client's error",
                      "Count":3
                   }
                ],
                "Duration":"00:00:10.0041841"
             }
          ],
          [
             {
                "ScenarioName":"scenario 1",
                "RequestCount":46,
                "OkCount":41,
                "FailCount":5,
                "AllDataMB":0.0,
                "StepStats":[
                   {
                      "StepName":"pull html 1",
                      "RequestCount":67,
                      "OkCount":59,
                      "FailCount":8,
                      "Min":34,
                      "Mean":809,
                      "Max":1501,
                      "RPS":2,
                      "Percent50":712,
                      "Percent75":1219,
                      "Percent95":1461,
                      "Percent99":1484,
                      "StdDev":409,
                      "MinDataKb":0.0,
                      "MeanDataKb":0.0,
                      "MaxDataKb":0.0,
                      "AllDataMB":0.0,
                      "ErrorStats":[
                         {
                            "ErrorCode":0,
                            "Message":"unknown client's error",
                            "Count":8
                         }
                      ]
                   }
                ],
                "LatencyCount":{
                   "Less800":25,
                   "More800Less1200":6,
                   "More1200":10
                },
                "LoadSimulationStats":{
                   "SimulationName":"inject_per_sec",
                   "Value":10
                },
                "ErrorStats":[
                   {
                      "ErrorCode":0,
                      "Message":"unknown client's error",
                      "Count":8
                   }
                ],
                "Duration":"00:00:15.0069894"
             }
          ],
          [
             {
                "ScenarioName":"scenario 1",
                "RequestCount":48,
                "OkCount":38,
                "FailCount":10,
                "AllDataMB":0.0,
                "StepStats":[
                   {
                      "StepName":"pull html 1",
                      "RequestCount":115,
                      "OkCount":97,
                      "FailCount":18,
                      "Min":34,
                      "Mean":820,
                      "Max":1501,
                      "RPS":3,
                      "Percent50":817,
                      "Percent75":1232,
                      "Percent95":1455,
                      "Percent99":1484,
                      "StdDev":413,
                      "MinDataKb":0.0,
                      "MeanDataKb":0.0,
                      "MaxDataKb":0.0,
                      "AllDataMB":0.0,
                      "ErrorStats":[
                         {
                            "ErrorCode":0,
                            "Message":"unknown client's error",
                            "Count":18
                         }
                      ]
                   }
                ],
                "LatencyCount":{
                   "Less800":17,
                   "More800Less1200":10,
                   "More1200":11
                },
                "LoadSimulationStats":{
                   "SimulationName":"inject_per_sec",
                   "Value":10
                },
                "ErrorStats":[
                   {
                      "ErrorCode":0,
                      "Message":"unknown client's error",
                      "Count":18
                   }
                ],
                "Duration":"00:00:20"
             }
          ]
       ]
    },
    "Hints":[
        {
            "SourceName": "scenario_1",
            "SourceType": "Scenario",
            "Hint": "Scenario 'scenario_1' has '16' errors that affect overall statistics. NBomber is not taking error request's latency into latency statistics. So make sure that your load tests don't have errors."
        },
        {
            "SourceName": "scenario_1",
            "SourceType": "Scenario",
            "Hint": "Step 'pull-html-1' in scenario 'scenario_1' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
        }
      ]
 };
