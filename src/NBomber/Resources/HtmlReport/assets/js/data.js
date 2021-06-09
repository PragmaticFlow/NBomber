const viewModel = {
    "FinalStats": {
        "RequestCount": 3000,
        "OkCount": 2979,
        "FailCount": 21,
        "AllBytes": 0,
        "ScenarioStats": [{
            "ScenarioName": "simple_http",
            "RequestCount": 3000,
            "OkCount": 2979,
            "FailCount": 21,
            "AllBytes": 0,
            "StepStats": [{
                "StepName": "fetch_html_page",
                "Ok": {
                    "Request": {"Count": 2979, "RPS": 99.3},
                    "Latency": {
                        "MinMs": 136.47,
                        "MeanMs": 472.62,
                        "MaxMs": 989.02,
                        "Percent50": 485.63,
                        "Percent75": 547.33,
                        "Percent95": 761.34,
                        "Percent99": 839.68,
                        "StdDev": 165.59,
                        "LatencyCount": {"LessOrEq800": 2882, "More800Less1200": 97, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{"Count@": 2979, "StatusCode": 200, "IsError": false, "Message": "", "Count": 2979}]
                },
                "Fail": {
                    "Request": {"Count": 21, "RPS": 0.7},
                    "Latency": {
                        "MinMs": 1000.05,
                        "MeanMs": 1001.92,
                        "MaxMs": 1004.13,
                        "Percent50": 1001.98,
                        "Percent75": 1003.01,
                        "Percent95": 1003.52,
                        "Percent99": 1004.54,
                        "StdDev": 1.08,
                        "LatencyCount": {"LessOrEq800": 0, "More800Less1200": 21, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{
                        "Count@": 21,
                        "StatusCode": -100,
                        "IsError": true,
                        "Message": "step timeout",
                        "Count": 21
                    }]
                }
            }],
            "LatencyCount": {"LessOrEq800": 2882, "More800Less1200": 118, "MoreOrEq1200": 0},
            "LoadSimulationStats": {"SimulationName": "inject_per_sec", "Value": 100},
            "StatusCodes": [{
                "Count@": 21,
                "StatusCode": -100,
                "IsError": true,
                "Message": "step timeout",
                "Count": 21
            }, {"Count@": 2979, "StatusCode": 200, "IsError": false, "Message": "", "Count": 2979}],
            "CurrentOperation": 5,
            "Duration": "00:00:30"
        }],
        "PluginStats": [{
            "CaseSensitive": false,
            "DataSetName": "NewDataSet",
            "EnforceConstraints": true,
            "ExtendedProperties": [],
            "Locale": "",
            "Namespace": "",
            "Prefix": "",
            "RemotingFormat": 0,
            "SchemaSerializationMode": 1,
            "Tables": {
                "NBomber.Plugins.Network.PingPlugin": {
                    "CaseSensitive": false,
                    "DisplayExpression": "",
                    "Locale": "",
                    "MinimumCapacity": 50,
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "TableName": "NBomber.Plugins.Network.PingPlugin",
                    "Columns": [{
                        "AllowDBNull": true,
                        "AutoIncrement": false,
                        "AutoIncrementSeed": 0,
                        "AutoIncrementStep": 1,
                        "Caption": "Host",
                        "ColumnMapping": 1,
                        "ColumnName": "Host",
                        "DataType": "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                        "DateTimeMode": 3,
                        "DefaultValue": null,
                        "Expression": "",
                        "ExtendedProperties": [],
                        "MaxLength": -1,
                        "Namespace": "",
                        "Prefix": "",
                        "ReadOnly": false
                    }, {
                        "AllowDBNull": true,
                        "AutoIncrement": false,
                        "AutoIncrementSeed": 0,
                        "AutoIncrementStep": 1,
                        "Caption": "Status",
                        "ColumnMapping": 1,
                        "ColumnName": "Status",
                        "DataType": "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                        "DateTimeMode": 3,
                        "DefaultValue": null,
                        "Expression": "",
                        "ExtendedProperties": [],
                        "MaxLength": -1,
                        "Namespace": "",
                        "Prefix": "",
                        "ReadOnly": false
                    }, {
                        "AllowDBNull": true,
                        "AutoIncrement": false,
                        "AutoIncrementSeed": 0,
                        "AutoIncrementStep": 1,
                        "Caption": "Address",
                        "ColumnMapping": 1,
                        "ColumnName": "Address",
                        "DataType": "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                        "DateTimeMode": 3,
                        "DefaultValue": null,
                        "Expression": "",
                        "ExtendedProperties": [],
                        "MaxLength": -1,
                        "Namespace": "",
                        "Prefix": "",
                        "ReadOnly": false
                    }, {
                        "AllowDBNull": true,
                        "AutoIncrement": false,
                        "AutoIncrementSeed": 0,
                        "AutoIncrementStep": 1,
                        "Caption": "Round Trip Time",
                        "ColumnMapping": 1,
                        "ColumnName": "RoundTripTime",
                        "DataType": "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                        "DateTimeMode": 3,
                        "DefaultValue": null,
                        "Expression": "",
                        "ExtendedProperties": [],
                        "MaxLength": -1,
                        "Namespace": "",
                        "Prefix": "",
                        "ReadOnly": false
                    }, {
                        "AllowDBNull": true,
                        "AutoIncrement": false,
                        "AutoIncrementSeed": 0,
                        "AutoIncrementStep": 1,
                        "Caption": "Time to Live",
                        "ColumnMapping": 1,
                        "ColumnName": "Ttl",
                        "DataType": "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                        "DateTimeMode": 3,
                        "DefaultValue": null,
                        "Expression": "",
                        "ExtendedProperties": [],
                        "MaxLength": -1,
                        "Namespace": "",
                        "Prefix": "",
                        "ReadOnly": false
                    }, {
                        "AllowDBNull": true,
                        "AutoIncrement": false,
                        "AutoIncrementSeed": 0,
                        "AutoIncrementStep": 1,
                        "Caption": "Don't Fragment",
                        "ColumnMapping": 1,
                        "ColumnName": "DontFragment",
                        "DataType": "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                        "DateTimeMode": 3,
                        "DefaultValue": null,
                        "Expression": "",
                        "ExtendedProperties": [],
                        "MaxLength": -1,
                        "Namespace": "",
                        "Prefix": "",
                        "ReadOnly": false
                    }, {
                        "AllowDBNull": true,
                        "AutoIncrement": false,
                        "AutoIncrementSeed": 0,
                        "AutoIncrementStep": 1,
                        "Caption": "Buffer Size",
                        "ColumnMapping": 1,
                        "ColumnName": "BufferSize",
                        "DataType": "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                        "DateTimeMode": 3,
                        "DefaultValue": null,
                        "Expression": "",
                        "ExtendedProperties": [],
                        "MaxLength": -1,
                        "Namespace": "",
                        "Prefix": "",
                        "ReadOnly": false
                    }],
                    "Constraints": [],
                    "Rows": [{
                        "OriginalRow": null,
                        "Host": "nbomber.com",
                        "Status": "Success",
                        "Address": "104.248.140.128",
                        "RoundTripTime": "45 ms",
                        "Ttl": "128",
                        "DontFragment": "False",
                        "BufferSize": "32 bytes",
                        "RowState": 4
                    }]
                }
            },
            "Relations": {}
        }],
        "NodeInfo": {
            "MachineName": "amoldovan",
            "NodeType": {"Case": "SingleNode"},
            "CurrentOperation": 5,
            "OS": {"Platform": 4, "ServicePack": "", "Version": "10.15.6", "VersionString": "Unix 10.15.6"},
            "DotNetVersion": ".NETCoreApp,Version=v5.0",
            "Processor": "",
            "CoresCount": 16,
            "NBomberVersion": "2.0.2"
        },
        "TestInfo": {
            "SessionId": "2021-06-09_14.13.67_session_ef978fb",
            "TestSuite": "nbomber_default_test_suite_name",
            "TestName": "nbomber_default_test_name"
        },
        "ReportFiles": [],
        "Duration": "00:00:30"
    },
    "TimeLineHistory": [{
        "ScenarioStats": [{
            "ScenarioName": "simple_http",
            "RequestCount": 900,
            "OkCount": 899,
            "FailCount": 1,
            "AllBytes": 0,
            "StepStats": [{
                "StepName": "fetch_html_page",
                "Ok": {
                    "Request": {"Count": 899, "RPS": 89.9},
                    "Latency": {
                        "MinMs": 152.34,
                        "MeanMs": 502.94,
                        "MaxMs": 937.13,
                        "Percent50": 525.31,
                        "Percent75": 584.19,
                        "Percent95": 786.94,
                        "Percent99": 848.9,
                        "StdDev": 177.92,
                        "LatencyCount": {"LessOrEq800": 860, "More800Less1200": 39, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{"Count@": 2979, "StatusCode": 200, "IsError": false, "Message": "", "Count": 2979}]
                },
                "Fail": {
                    "Request": {"Count": 1, "RPS": 0.1},
                    "Latency": {
                        "MinMs": 1000.88,
                        "MeanMs": 1000.7,
                        "MaxMs": 1000.88,
                        "Percent50": 1000.96,
                        "Percent75": 1000.96,
                        "Percent95": 1000.96,
                        "Percent99": 1000.96,
                        "StdDev": 0.0,
                        "LatencyCount": {"LessOrEq800": 0, "More800Less1200": 1, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{
                        "Count@": 21,
                        "StatusCode": -100,
                        "IsError": true,
                        "Message": "step timeout",
                        "Count": 21
                    }]
                }
            }],
            "LatencyCount": {"LessOrEq800": 860, "More800Less1200": 40, "MoreOrEq1200": 0},
            "LoadSimulationStats": {"SimulationName": "inject_per_sec", "Value": 100},
            "StatusCodes": [{
                "Count@": 1,
                "StatusCode": -100,
                "IsError": true,
                "Message": "step timeout",
                "Count": 1
            }, {"Count@": 899, "StatusCode": 200, "IsError": false, "Message": "", "Count": 899}],
            "CurrentOperation": 3,
            "Duration": "00:00:10"
        }], "Duration": "00:00:10"
    }, {
        "ScenarioStats": [{
            "ScenarioName": "simple_http",
            "RequestCount": 1900,
            "OkCount": 1879,
            "FailCount": 21,
            "AllBytes": 0,
            "StepStats": [{
                "StepName": "fetch_html_page",
                "Ok": {
                    "Request": {"Count": 1879, "RPS": 93.9},
                    "Latency": {
                        "MinMs": 136.47,
                        "MeanMs": 479.16,
                        "MaxMs": 989.02,
                        "Percent50": 490.75,
                        "Percent75": 553.47,
                        "Percent95": 775.68,
                        "Percent99": 849.41,
                        "StdDev": 171.43,
                        "LatencyCount": {"LessOrEq800": 1801, "More800Less1200": 78, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{"Count@": 2979, "StatusCode": 200, "IsError": false, "Message": "", "Count": 2979}]
                },
                "Fail": {
                    "Request": {"Count": 21, "RPS": 1.0},
                    "Latency": {
                        "MinMs": 1000.05,
                        "MeanMs": 1001.92,
                        "MaxMs": 1004.13,
                        "Percent50": 1001.98,
                        "Percent75": 1003.01,
                        "Percent95": 1003.52,
                        "Percent99": 1004.54,
                        "StdDev": 1.08,
                        "LatencyCount": {"LessOrEq800": 0, "More800Less1200": 21, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{
                        "Count@": 21,
                        "StatusCode": -100,
                        "IsError": true,
                        "Message": "step timeout",
                        "Count": 21
                    }]
                }
            }],
            "LatencyCount": {"LessOrEq800": 1801, "More800Less1200": 99, "MoreOrEq1200": 0},
            "LoadSimulationStats": {"SimulationName": "inject_per_sec", "Value": 100},
            "StatusCodes": [{
                "Count@": 21,
                "StatusCode": -100,
                "IsError": true,
                "Message": "step timeout",
                "Count": 21
            }, {"Count@": 1879, "StatusCode": 200, "IsError": false, "Message": "", "Count": 1879}],
            "CurrentOperation": 3,
            "Duration": "00:00:20"
        }], "Duration": "00:00:20"
    }, {
        "ScenarioStats": [{
            "ScenarioName": "simple_http",
            "RequestCount": 3000,
            "OkCount": 2979,
            "FailCount": 21,
            "AllBytes": 0,
            "StepStats": [{
                "StepName": "fetch_html_page",
                "Ok": {
                    "Request": {"Count": 2979, "RPS": 99.3},
                    "Latency": {
                        "MinMs": 136.47,
                        "MeanMs": 472.62,
                        "MaxMs": 989.02,
                        "Percent50": 485.63,
                        "Percent75": 547.33,
                        "Percent95": 761.34,
                        "Percent99": 839.68,
                        "StdDev": 165.59,
                        "LatencyCount": {"LessOrEq800": 2882, "More800Less1200": 97, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{"Count@": 2979, "StatusCode": 200, "IsError": false, "Message": "", "Count": 2979}]
                },
                "Fail": {
                    "Request": {"Count": 21, "RPS": 0.7},
                    "Latency": {
                        "MinMs": 1000.05,
                        "MeanMs": 1001.92,
                        "MaxMs": 1004.13,
                        "Percent50": 1001.98,
                        "Percent75": 1003.01,
                        "Percent95": 1003.52,
                        "Percent99": 1004.54,
                        "StdDev": 1.08,
                        "LatencyCount": {"LessOrEq800": 0, "More800Less1200": 21, "MoreOrEq1200": 0}
                    },
                    "DataTransfer": {
                        "MinBytes": 0,
                        "MeanBytes": 0,
                        "MaxBytes": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0.0,
                        "AllBytes": 0
                    },
                    "StatusCodes": [{
                        "Count@": 21,
                        "StatusCode": -100,
                        "IsError": true,
                        "Message": "step timeout",
                        "Count": 21
                    }]
                }
            }],
            "LatencyCount": {"LessOrEq800": 2882, "More800Less1200": 118, "MoreOrEq1200": 0},
            "LoadSimulationStats": {"SimulationName": "inject_per_sec", "Value": 100},
            "StatusCodes": [{
                "Count@": 21,
                "StatusCode": -100,
                "IsError": true,
                "Message": "step timeout",
                "Count": 21
            }, {"Count@": 2979, "StatusCode": 200, "IsError": false, "Message": "", "Count": 2979}],
            "CurrentOperation": 5,
            "Duration": "00:00:30"
        }], "Duration": "00:00:30"
    }],
    "Hints": [{
        "SourceName": "simple_http",
        "SourceType": "Scenario",
        "Hint": "Step 'fetch_html_page' in scenario 'simple_http' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
    }, {
        "SourceName": "NBomber.Plugins.Network.PingPlugin",
        "SourceType": "WorkerPlugin",
        "Hint": "Physical latency to host: 'nbomber.com' is '45'.  This is bigger than 2ms which is not appropriate for load testing. You should run your test in an environment with very small latency."
    }]
};
