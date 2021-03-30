const viewModel = {
    "NodeStats": {
        "RequestCount": 491,
        "OkCount": 480,
        "FailCount": 11,
        "AllDataMB": 0.14,
        "ScenarioStats": [
            {
                "ScenarioName": "scenario_1",
                "RequestCount": 198,
                "OkCount": 198,
                "FailCount": 0,
                "AllDataMB": 0.0,
                "StepStats": [
                    {
                        "StepName": "pull_html_1",
                        "Ok": {
                            "Request": {
                                "Count": 198,
                                "RPS": 6.6
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 797.4,
                                "MaxMs": 1504.9,
                                "Percent50": 726.6,
                                "Percent75": 1064.7,
                                "Percent95": 1407.8,
                                "Percent99": 1429.8,
                                "StdDev": 423.1,
                                "LatencyCount": {
                                    "LessOrEq800": 104,
                                    "More800Less1200": 46,
                                    "MoreOrEq1200": 48
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.0,
                                "MaxKb": 0.0,
                                "Percent50": 0.0,
                                "Percent75": 0.0,
                                "Percent95": 0.0,
                                "Percent99": 0.0,
                                "StdDev": 0.0,
                                "AllMB": 0.0
                            },
                            "StatusCodes": []
                        },
                        "Fail": {
                            "Request": {
                                "Count": 0,
                                "RPS": 0.0
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 0.0,
                                "MaxMs": 0.0,
                                "Percent50": 0.0,
                                "Percent75": 0.0,
                                "Percent95": 0.0,
                                "Percent99": 0.0,
                                "StdDev": 0.0,
                                "LatencyCount": {
                                    "LessOrEq800": 0,
                                    "More800Less1200": 0,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.0,
                                "MaxKb": 0.0,
                                "Percent50": 0.0,
                                "Percent75": 0.0,
                                "Percent95": 0.0,
                                "Percent99": 0.0,
                                "StdDev": 0.0,
                                "AllMB": 0.0
                            },
                            "StatusCodes": []
                        }
                    }
                ],
                "LatencyCount": {
                    "LessOrEq800": 104,
                    "More800Less1200": 46,
                    "MoreOrEq1200": 48
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 10
                },
                "StatusCodes": [],
                "CurrentOperation": 5,
                "Duration": "00:00:30"
            },
            {
                "ScenarioName": "scenario_2",
                "RequestCount": 192,
                "OkCount": 191,
                "FailCount": 1,
                "AllDataMB": 0.09,
                "StepStats": [
                    {
                        "StepName": "pull_html_2",
                        "Ok": {
                            "Request": {
                                "Count": 191,
                                "RPS": 6.4
                            },
                            "Latency": {
                                "MinMs": 15.5,
                                "MeanMs": 739.2,
                                "MaxMs": 1486.8,
                                "Percent50": 729.8,
                                "Percent75": 943.1,
                                "Percent95": 1235.9,
                                "Percent99": 1289.4,
                                "StdDev": 331.8,
                                "LatencyCount": {
                                    "LessOrEq800": 103,
                                    "More800Less1200": 61,
                                    "MoreOrEq1200": 27
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.01,
                                "MeanKb": 0.5,
                                "MaxKb": 0.97,
                                "Percent50": 0.48,
                                "Percent75": 0.66,
                                "Percent95": 0.85,
                                "Percent99": 0.87,
                                "StdDev": 0.25,
                                "AllMB": 0.09
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 191,
                                    "StatusCode": 200,
                                    "IsError": false,
                                    "Message": "",
                                    "Count": 191
                                }
                            ]
                        },
                        "Fail": {
                            "Request": {
                                "Count": 1,
                                "RPS": 0.0
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 73.2,
                                "MaxMs": 1317.3,
                                "Percent50": 73.2,
                                "Percent75": 73.2,
                                "Percent95": 73.2,
                                "Percent99": 73.2,
                                "StdDev": 0.0,
                                "LatencyCount": {
                                    "LessOrEq800": 0,
                                    "More800Less1200": 0,
                                    "MoreOrEq1200": 1
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.0,
                                "MaxKb": 0.09,
                                "Percent50": 0.0,
                                "Percent75": 0.0,
                                "Percent95": 0.0,
                                "Percent99": 0.0,
                                "StdDev": 0.0,
                                "AllMB": 0.0
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 1,
                                    "StatusCode": 500,
                                    "IsError": true,
                                    "Message": "Internal Server Error",
                                    "Count": 1
                                }
                            ]
                        }
                    }
                ],
                "LatencyCount": {
                    "LessOrEq800": 103,
                    "More800Less1200": 61,
                    "MoreOrEq1200": 28
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 5
                },
                "StatusCodes": [
                    {
                        "Count@": 191,
                        "StatusCode": 200,
                        "IsError": false,
                        "Message": "",
                        "Count": 191
                    },
                    {
                        "Count@": 1,
                        "StatusCode": 500,
                        "IsError": true,
                        "Message": "Internal Server Error",
                        "Count": 1
                    }
                ],
                "CurrentOperation": 5,
                "Duration": "00:00:30"
            },
            {
                "ScenarioName": "scenario_3",
                "RequestCount": 101,
                "OkCount": 91,
                "FailCount": 10,
                "AllDataMB": 0.05,
                "StepStats": [
                    {
                        "StepName": "pull_html_3",
                        "Ok": {
                            "Request": {
                                "Count": 33,
                                "RPS": 2.4
                            },
                            "Latency": {
                                "MinMs": 60.5,
                                "MeanMs": 845.6,
                                "MaxMs": 1495.0,
                                "Percent50": 758.8,
                                "Percent75": 1009.7,
                                "Percent95": 1192.4,
                                "Percent99": 1192.4,
                                "StdDev": 272.1,
                                "LatencyCount": {
                                    "LessOrEq800": 14,
                                    "More800Less1200": 15,
                                    "MoreOrEq1200": 4
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.47,
                                "MaxKb": 0.97,
                                "Percent50": 0.39,
                                "Percent75": 0.55,
                                "Percent95": 0.77,
                                "Percent99": 0.77,
                                "StdDev": 0.23,
                                "AllMB": 0.02
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 20,
                                    "StatusCode": 200,
                                    "IsError": false,
                                    "Message": "",
                                    "Count": 20
                                }
                            ]
                        },
                        "Fail": {
                            "Request": {
                                "Count": 5,
                                "RPS": 0.4
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 331.8,
                                "MaxMs": 1124.8,
                                "Percent50": 277.2,
                                "Percent75": 386.7,
                                "Percent95": 386.7,
                                "Percent99": 386.7,
                                "StdDev": 54.7,
                                "LatencyCount": {
                                    "LessOrEq800": 2,
                                    "More800Less1200": 3,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.22,
                                "MaxKb": 0.82,
                                "Percent50": 0.21,
                                "Percent75": 0.23,
                                "Percent95": 0.23,
                                "Percent99": 0.23,
                                "StdDev": 0.01,
                                "AllMB": 0.0
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 1,
                                    "StatusCode": 400,
                                    "IsError": true,
                                    "Message": "Bad Request",
                                    "Count": 1
                                },
                                {
                                    "Count@": 1,
                                    "StatusCode": 401,
                                    "IsError": true,
                                    "Message": "Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "Count@": 1,
                                    "StatusCode": 502,
                                    "IsError": true,
                                    "Message": "Bad Gateway",
                                    "Count": 1
                                }
                            ]
                        }
                    },
                    {
                        "StepName": "pull_html_4",
                        "Ok": {
                            "Request": {
                                "Count": 30,
                                "RPS": 2.2
                            },
                            "Latency": {
                                "MinMs": 31.8,
                                "MeanMs": 724.9,
                                "MaxMs": 1460.6,
                                "Percent50": 723.7,
                                "Percent75": 854.6,
                                "Percent95": 921.4,
                                "Percent99": 921.4,
                                "StdDev": 212.4,
                                "LatencyCount": {
                                    "LessOrEq800": 15,
                                    "More800Less1200": 7,
                                    "MoreOrEq1200": 6
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.05,
                                "MeanKb": 0.53,
                                "MaxKb": 0.92,
                                "Percent50": 0.47,
                                "Percent75": 0.62,
                                "Percent95": 0.78,
                                "Percent99": 0.78,
                                "StdDev": 0.2,
                                "AllMB": 0.01
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 18,
                                    "StatusCode": 200,
                                    "IsError": false,
                                    "Message": "",
                                    "Count": 18
                                }
                            ]
                        },
                        "Fail": {
                            "Request": {
                                "Count": 3,
                                "RPS": 0.2
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 161.5,
                                "MaxMs": 947.8,
                                "Percent50": 161.6,
                                "Percent75": 161.6,
                                "Percent95": 161.6,
                                "Percent99": 161.6,
                                "StdDev": 0.0,
                                "LatencyCount": {
                                    "LessOrEq800": 1,
                                    "More800Less1200": 1,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.11,
                                "MaxKb": 0.93,
                                "Percent50": 0.11,
                                "Percent75": 0.11,
                                "Percent95": 0.11,
                                "Percent99": 0.11,
                                "StdDev": 0.0,
                                "AllMB": 0.0
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 2,
                                    "StatusCode": 503,
                                    "IsError": true,
                                    "Message": "Service Unavailable",
                                    "Count": 2
                                }
                            ]
                        }
                    },
                    {
                        "StepName": "pull_html_5",
                        "Ok": {
                            "Request": {
                                "Count": 28,
                                "RPS": 2.0
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 414.8,
                                "MaxMs": 1359.9,
                                "Percent50": 246.0,
                                "Percent75": 492.9,
                                "Percent95": 692.8,
                                "Percent99": 692.8,
                                "StdDev": 215.3,
                                "LatencyCount": {
                                    "LessOrEq800": 18,
                                    "More800Less1200": 2,
                                    "MoreOrEq1200": 3
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.47,
                                "MaxKb": 0.93,
                                "Percent50": 0.42,
                                "Percent75": 0.54,
                                "Percent95": 0.63,
                                "Percent99": 0.63,
                                "StdDev": 0.14,
                                "AllMB": 0.01
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 17,
                                    "StatusCode": 200,
                                    "IsError": false,
                                    "Message": "",
                                    "Count": 17
                                }
                            ]
                        },
                        "Fail": {
                            "Request": {
                                "Count": 2,
                                "RPS": 0.1
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 247.3,
                                "MaxMs": 1420.5,
                                "Percent50": 247.4,
                                "Percent75": 247.4,
                                "Percent95": 247.4,
                                "Percent99": 247.4,
                                "StdDev": 0.0,
                                "LatencyCount": {
                                    "LessOrEq800": 0,
                                    "More800Less1200": 1,
                                    "MoreOrEq1200": 1
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.1,
                                "MaxKb": 0.83,
                                "Percent50": 0.1,
                                "Percent75": 0.1,
                                "Percent95": 0.1,
                                "Percent99": 0.1,
                                "StdDev": 0.0,
                                "AllMB": 0.0
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 1,
                                    "StatusCode": 403,
                                    "IsError": true,
                                    "Message": "Forbidden",
                                    "Count": 1
                                },
                                {
                                    "Count@": 1,
                                    "StatusCode": 502,
                                    "IsError": true,
                                    "Message": "Bad Gateway",
                                    "Count": 1
                                }
                            ]
                        }
                    }
                ],
                "LatencyCount": {
                    "LessOrEq800": 50,
                    "More800Less1200": 29,
                    "MoreOrEq1200": 14
                },
                "LoadSimulationStats": {
                    "SimulationName": "ramp_constant",
                    "Value": 9
                },
                "StatusCodes": [
                    {
                        "Count@": 55,
                        "StatusCode": 200,
                        "IsError": false,
                        "Message": "",
                        "Count": 55
                    },
                    {
                        "Count@": 1,
                        "StatusCode": 400,
                        "IsError": true,
                        "Message": "Bad Request",
                        "Count": 1
                    },
                    {
                        "Count@": 1,
                        "StatusCode": 401,
                        "IsError": true,
                        "Message": "Unauthorized",
                        "Count": 1
                    },
                    {
                        "Count@": 1,
                        "StatusCode": 403,
                        "IsError": true,
                        "Message": "Forbidden",
                        "Count": 1
                    },
                    {
                        "Count@": 2,
                        "StatusCode": 502,
                        "IsError": true,
                        "Message": "Bad Gateway",
                        "Count": 2
                    },
                    {
                        "Count@": 2,
                        "StatusCode": 503,
                        "IsError": true,
                        "Message": "Service Unavailable",
                        "Count": 2
                    }
                ],
                "CurrentOperation": 5,
                "Duration": "00:00:13"
            }
        ],
        "PluginStats": [
            {
                "CaseSensitive": false,
                "DataSetName": "NewDataSet",
                "EnforceConstraints": true,
                "ExtendedProperties": [],
                "Locale": "en-US",
                "Namespace": "",
                "Prefix": "",
                "RemotingFormat": 0,
                "SchemaSerializationMode": 1,
                "Tables": {
                    "CustomPlugin1": {
                        "CaseSensitive": false,
                        "DisplayExpression": "",
                        "Locale": "en-US",
                        "MinimumCapacity": 50,
                        "Namespace": "",
                        "Prefix": "",
                        "RemotingFormat": 0,
                        "TableName": "CustomPlugin1",
                        "Columns": [
                            {
                                "AllowDBNull": true,
                                "AutoIncrement": false,
                                "AutoIncrementSeed": 0,
                                "AutoIncrementStep": 1,
                                "Caption": "Property",
                                "ColumnMapping": 1,
                                "ColumnName": "Key",
                                "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                "DateTimeMode": 3,
                                "DefaultValue": null,
                                "Expression": "",
                                "ExtendedProperties": [],
                                "MaxLength": -1,
                                "Namespace": "",
                                "Prefix": "",
                                "ReadOnly": false
                            },
                            {
                                "AllowDBNull": true,
                                "AutoIncrement": false,
                                "AutoIncrementSeed": 0,
                                "AutoIncrementStep": 1,
                                "Caption": "Value",
                                "ColumnMapping": 1,
                                "ColumnName": "Value",
                                "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                "DateTimeMode": 3,
                                "DefaultValue": null,
                                "Expression": "",
                                "ExtendedProperties": [],
                                "MaxLength": -1,
                                "Namespace": "",
                                "Prefix": "",
                                "ReadOnly": false
                            }
                        ],
                        "Constraints": [],
                        "Rows": [
                            {
                                "OriginalRow": null,
                                "Key": "Property1",
                                "Value": "1463184558",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Key": "Property2",
                                "Value": "1519294472",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Key": "Property3",
                                "Value": "615655038",
                                "RowState": 4
                            }
                        ]
                    }
                },
                "Relations": {}
            },
            {
                "CaseSensitive": false,
                "DataSetName": "NewDataSet",
                "EnforceConstraints": true,
                "ExtendedProperties": [],
                "Locale": "en-US",
                "Namespace": "",
                "Prefix": "",
                "RemotingFormat": 0,
                "SchemaSerializationMode": 1,
                "Tables": {
                    "CustomPlugin2": {
                        "CaseSensitive": false,
                        "DisplayExpression": "",
                        "Locale": "en-US",
                        "MinimumCapacity": 50,
                        "Namespace": "",
                        "Prefix": "",
                        "RemotingFormat": 0,
                        "TableName": "CustomPlugin2",
                        "Columns": [
                            {
                                "AllowDBNull": true,
                                "AutoIncrement": false,
                                "AutoIncrementSeed": 0,
                                "AutoIncrementStep": 1,
                                "Caption": "Property",
                                "ColumnMapping": 1,
                                "ColumnName": "Property",
                                "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                "DateTimeMode": 3,
                                "DefaultValue": null,
                                "Expression": "",
                                "ExtendedProperties": [],
                                "MaxLength": -1,
                                "Namespace": "",
                                "Prefix": "",
                                "ReadOnly": false
                            },
                            {
                                "AllowDBNull": true,
                                "AutoIncrement": false,
                                "AutoIncrementSeed": 0,
                                "AutoIncrementStep": 1,
                                "Caption": "Value",
                                "ColumnMapping": 1,
                                "ColumnName": "Value",
                                "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                "DateTimeMode": 3,
                                "DefaultValue": null,
                                "Expression": "",
                                "ExtendedProperties": [],
                                "MaxLength": -1,
                                "Namespace": "",
                                "Prefix": "",
                                "ReadOnly": false
                            }
                        ],
                        "Constraints": [],
                        "Rows": [
                            {
                                "OriginalRow": null,
                                "Property": "Property 1",
                                "Value": "1836339229",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Property": "Property 2",
                                "Value": "818697400",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Property": "Property 3",
                                "Value": "99395292",
                                "RowState": 4
                            }
                        ]
                    }
                },
                "Relations": {}
            }
        ],
        "NodeInfo": {
            "MachineName": "DESKTOP-DMNJHK6",
            "NodeType": {
                "Case": "SingleNode"
            },
            "CurrentOperation": 5,
            "OS": {
                "Platform": 2,
                "ServicePack": "",
                "Version": "6.2.9200.0",
                "VersionString": "Microsoft Windows NT 6.2.9200.0"
            },
            "DotNetVersion": ".NETCoreApp,Version=v3.1",
            "Processor": "Intel64 Family 6 Model 94 Stepping 3, GenuineIntel",
            "CoresCount": 8,
            "NBomberVersion": "2.0.0"
        },
        "TestInfo": {
            "SessionId": "2021-03-30_14.12.01_session_dee007fd",
            "TestSuite": "nbomber_default_test_suite_name",
            "TestName": "nbomber_default_test_name"
        },
        "ReportFiles": [],
        "Duration": "00:00:30"
    },
    "TimeLineStats": [
        {
            "Duration": "00:00:05",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 4,
                    "OkCount": 4,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.8
                                },
                                "Latency": {
                                    "MinMs": 17.0,
                                    "MeanMs": 340.5,
                                    "MaxMs": 1164.9,
                                    "Percent50": 379.2,
                                    "Percent75": 379.2,
                                    "Percent95": 591.0,
                                    "Percent99": 591.0,
                                    "StdDev": 221.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 3,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 3,
                        "More800Less1200": 1,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "StatusCodes": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:05"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 7,
                    "OkCount": 6,
                    "FailCount": 1,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 6,
                                    "RPS": 1.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 583.6,
                                    "MaxMs": 1388.5,
                                    "Percent50": 588.7,
                                    "Percent75": 637.3,
                                    "Percent95": 792.7,
                                    "Percent99": 792.7,
                                    "StdDev": 128.3,
                                    "LatencyCount": {
                                        "LessOrEq800": 3,
                                        "More800Less1200": 2,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.44,
                                    "MaxKb": 0.84,
                                    "Percent50": 0.47,
                                    "Percent75": 0.49,
                                    "Percent95": 0.5,
                                    "Percent99": 0.5,
                                    "StdDev": 0.06,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 6,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 6
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 439.0,
                                    "MaxMs": 1317.3,
                                    "Percent50": 439.1,
                                    "Percent75": 439.1,
                                    "Percent95": 439.1,
                                    "Percent99": 439.1,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.03,
                                    "MaxKb": 0.09,
                                    "Percent50": 0.03,
                                    "Percent75": 0.03,
                                    "Percent95": 0.03,
                                    "Percent99": 0.03,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 500,
                                        "IsError": true,
                                        "Message": "Internal Server Error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 3,
                        "More800Less1200": 2,
                        "MoreOrEq1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "StatusCodes": [
                        {
                            "Count@": 6,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 6
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 500,
                            "IsError": true,
                            "Message": "Internal Server Error",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:05"
                },
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 7,
                    "OkCount": 7,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 407.7,
                                    "MaxMs": 1136.2,
                                    "Percent50": 229.9,
                                    "Percent75": 585.7,
                                    "Percent95": 585.7,
                                    "Percent99": 585.7,
                                    "StdDev": 177.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.47,
                                    "MaxKb": 0.94,
                                    "Percent50": 0.36,
                                    "Percent75": 0.58,
                                    "Percent95": 0.58,
                                    "Percent99": 0.58,
                                    "StdDev": 0.11,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 3,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 3
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 701.4,
                                    "MaxMs": 1407.4,
                                    "Percent50": 482.8,
                                    "Percent75": 920.5,
                                    "Percent95": 920.5,
                                    "Percent99": 920.5,
                                    "StdDev": 218.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 2
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.51,
                                    "MaxKb": 0.92,
                                    "Percent50": 0.42,
                                    "Percent75": 0.59,
                                    "Percent95": 0.59,
                                    "Percent99": 0.59,
                                    "StdDev": 0.09,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 1
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 57.2,
                                    "MaxMs": 171.7,
                                    "Percent50": 57.2,
                                    "Percent75": 57.2,
                                    "Percent95": 57.2,
                                    "Percent99": 57.2,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.18,
                                    "MaxKb": 0.53,
                                    "Percent50": 0.18,
                                    "Percent75": 0.18,
                                    "Percent95": 0.18,
                                    "Percent99": 0.18,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 4,
                        "More800Less1200": 1,
                        "MoreOrEq1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "StatusCodes": [
                        {
                            "Count@": 4,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 4
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:05"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "761409244",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1670451810",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "458083731",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "1222059642",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1755591597",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1607172069",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        },
        {
            "Duration": "00:00:10",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 18,
                    "OkCount": 18,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 18,
                                    "RPS": 1.8
                                },
                                "Latency": {
                                    "MinMs": 17.0,
                                    "MeanMs": 868.5,
                                    "MaxMs": 1454.1,
                                    "Percent50": 871.8,
                                    "Percent75": 957.8,
                                    "Percent95": 1362.1,
                                    "Percent99": 1362.1,
                                    "StdDev": 301.9,
                                    "LatencyCount": {
                                        "LessOrEq800": 11,
                                        "More800Less1200": 3,
                                        "MoreOrEq1200": 4
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 11,
                        "More800Less1200": 3,
                        "MoreOrEq1200": 4
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "StatusCodes": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:10"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 37,
                    "OkCount": 36,
                    "FailCount": 1,
                    "AllDataMB": 0.02,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 36,
                                    "RPS": 3.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 632.8,
                                    "MaxMs": 1388.5,
                                    "Percent50": 635.2,
                                    "Percent75": 778.1,
                                    "Percent95": 954.0,
                                    "Percent99": 962.4,
                                    "StdDev": 240.9,
                                    "LatencyCount": {
                                        "LessOrEq800": 20,
                                        "More800Less1200": 12,
                                        "MoreOrEq1200": 4
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.47,
                                    "MaxKb": 0.97,
                                    "Percent50": 0.47,
                                    "Percent75": 0.58,
                                    "Percent95": 0.71,
                                    "Percent99": 0.73,
                                    "StdDev": 0.19,
                                    "AllMB": 0.02
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 36,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 36
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 188.1,
                                    "MaxMs": 1317.3,
                                    "Percent50": 188.2,
                                    "Percent75": 188.2,
                                    "Percent95": 188.2,
                                    "Percent99": 188.2,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.01,
                                    "MaxKb": 0.09,
                                    "Percent50": 0.01,
                                    "Percent75": 0.01,
                                    "Percent95": 0.01,
                                    "Percent99": 0.01,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 500,
                                        "IsError": true,
                                        "Message": "Internal Server Error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 20,
                        "More800Less1200": 12,
                        "MoreOrEq1200": 5
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "StatusCodes": [
                        {
                            "Count@": 36,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 36
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 500,
                            "IsError": true,
                            "Message": "Internal Server Error",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:10"
                },
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 38,
                    "OkCount": 31,
                    "FailCount": 7,
                    "AllDataMB": 0.02,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 13,
                                    "RPS": 1.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 664.9,
                                    "MaxMs": 1136.2,
                                    "Percent50": 609.4,
                                    "Percent75": 770.6,
                                    "Percent95": 783.6,
                                    "Percent99": 783.6,
                                    "StdDev": 104.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 7,
                                        "More800Less1200": 6,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.53,
                                    "MaxKb": 0.97,
                                    "Percent50": 0.46,
                                    "Percent75": 0.63,
                                    "Percent95": 0.65,
                                    "Percent99": 0.65,
                                    "StdDev": 0.11,
                                    "AllMB": 0.01
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 11,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 11
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 272.2,
                                    "MaxMs": 1124.8,
                                    "Percent50": 201.9,
                                    "Percent75": 342.7,
                                    "Percent95": 342.7,
                                    "Percent99": 342.7,
                                    "StdDev": 70.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 2,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.17,
                                    "MaxKb": 0.75,
                                    "Percent50": 0.16,
                                    "Percent75": 0.18,
                                    "Percent95": 0.18,
                                    "Percent99": 0.18,
                                    "StdDev": 0.01,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 400,
                                        "IsError": true,
                                        "Message": "Bad Request",
                                        "Count": 1
                                    },
                                    {
                                        "Count@": 1,
                                        "StatusCode": 502,
                                        "IsError": true,
                                        "Message": "Bad Gateway",
                                        "Count": 1
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 10,
                                    "RPS": 1.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 628.5,
                                    "MaxMs": 1408.2,
                                    "Percent50": 639.4,
                                    "Percent75": 742.9,
                                    "Percent95": 743.0,
                                    "Percent99": 743.0,
                                    "StdDev": 140.2,
                                    "LatencyCount": {
                                        "LessOrEq800": 4,
                                        "More800Less1200": 3,
                                        "MoreOrEq1200": 3
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.47,
                                    "MaxKb": 0.92,
                                    "Percent50": 0.4,
                                    "Percent75": 0.51,
                                    "Percent95": 0.59,
                                    "Percent99": 0.59,
                                    "StdDev": 0.11,
                                    "AllMB": 0.01
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 5,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 5
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 207.7,
                                    "MaxMs": 947.8,
                                    "Percent50": 207.8,
                                    "Percent75": 207.8,
                                    "Percent95": 207.8,
                                    "Percent99": 207.8,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.14,
                                    "MaxKb": 0.93,
                                    "Percent50": 0.14,
                                    "Percent75": 0.14,
                                    "Percent95": 0.14,
                                    "Percent99": 0.14,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 503,
                                        "IsError": true,
                                        "Message": "Service Unavailable",
                                        "Count": 1
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 8,
                                    "RPS": 0.8
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 466.0,
                                    "MaxMs": 1262.4,
                                    "Percent50": 392.5,
                                    "Percent75": 539.7,
                                    "Percent95": 539.7,
                                    "Percent99": 539.7,
                                    "StdDev": 73.6,
                                    "LatencyCount": {
                                        "LessOrEq800": 6,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 2
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.4,
                                    "MaxKb": 0.91,
                                    "Percent50": 0.34,
                                    "Percent75": 0.46,
                                    "Percent95": 0.46,
                                    "Percent99": 0.46,
                                    "StdDev": 0.06,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 6,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 6
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 202.9,
                                    "MaxMs": 1420.5,
                                    "Percent50": 202.9,
                                    "Percent75": 202.9,
                                    "Percent95": 202.9,
                                    "Percent99": 202.9,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.01,
                                    "MaxKb": 0.09,
                                    "Percent50": 0.01,
                                    "Percent75": 0.01,
                                    "Percent95": 0.01,
                                    "Percent99": 0.01,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 403,
                                        "IsError": true,
                                        "Message": "Forbidden",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 20,
                        "More800Less1200": 12,
                        "MoreOrEq1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "StatusCodes": [
                        {
                            "Count@": 22,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 22
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 400,
                            "IsError": true,
                            "Message": "Bad Request",
                            "Count": 1
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 403,
                            "IsError": true,
                            "Message": "Forbidden",
                            "Count": 1
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 502,
                            "IsError": true,
                            "Message": "Bad Gateway",
                            "Count": 1
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 503,
                            "IsError": true,
                            "Message": "Service Unavailable",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:10"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "267908808",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "2014528706",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1482951831",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "2059188925",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "460169579",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1036887488",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        },
        {
            "Duration": "00:00:13",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 85,
                    "OkCount": 76,
                    "FailCount": 9,
                    "AllDataMB": 0.04,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 30,
                                    "RPS": 2.2
                                },
                                "Latency": {
                                    "MinMs": 60.5,
                                    "MeanMs": 847.0,
                                    "MaxMs": 1495.0,
                                    "Percent50": 741.7,
                                    "Percent75": 995.2,
                                    "Percent95": 1192.4,
                                    "Percent99": 1192.4,
                                    "StdDev": 273.1,
                                    "LatencyCount": {
                                        "LessOrEq800": 13,
                                        "More800Less1200": 13,
                                        "MoreOrEq1200": 4
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.47,
                                    "MaxKb": 0.97,
                                    "Percent50": 0.41,
                                    "Percent75": 0.56,
                                    "Percent95": 0.77,
                                    "Percent99": 0.77,
                                    "StdDev": 0.24,
                                    "AllMB": 0.01
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 19,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 19
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 5,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 331.8,
                                    "MaxMs": 1124.8,
                                    "Percent50": 277.2,
                                    "Percent75": 386.7,
                                    "Percent95": 386.7,
                                    "Percent99": 386.7,
                                    "StdDev": 54.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 3,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.22,
                                    "MaxKb": 0.82,
                                    "Percent50": 0.21,
                                    "Percent75": 0.23,
                                    "Percent95": 0.23,
                                    "Percent99": 0.23,
                                    "StdDev": 0.01,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 400,
                                        "IsError": true,
                                        "Message": "Bad Request",
                                        "Count": 1
                                    },
                                    {
                                        "Count@": 1,
                                        "StatusCode": 401,
                                        "IsError": true,
                                        "Message": "Unauthorized",
                                        "Count": 1
                                    },
                                    {
                                        "Count@": 1,
                                        "StatusCode": 502,
                                        "IsError": true,
                                        "Message": "Bad Gateway",
                                        "Count": 1
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 26,
                                    "RPS": 1.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 642.2,
                                    "MaxMs": 1460.6,
                                    "Percent50": 665.6,
                                    "Percent75": 747.2,
                                    "Percent95": 814.1,
                                    "Percent99": 814.1,
                                    "StdDev": 187.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 13,
                                        "More800Less1200": 7,
                                        "MoreOrEq1200": 6
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.52,
                                    "MaxKb": 0.92,
                                    "Percent50": 0.48,
                                    "Percent75": 0.58,
                                    "Percent95": 0.74,
                                    "Percent99": 0.74,
                                    "StdDev": 0.17,
                                    "AllMB": 0.01
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 16,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 16
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 161.5,
                                    "MaxMs": 947.8,
                                    "Percent50": 161.6,
                                    "Percent75": 161.6,
                                    "Percent95": 161.6,
                                    "Percent99": 161.6,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.11,
                                    "MaxKb": 0.93,
                                    "Percent50": 0.11,
                                    "Percent75": 0.11,
                                    "Percent95": 0.11,
                                    "Percent99": 0.11,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 503,
                                        "IsError": true,
                                        "Message": "Service Unavailable",
                                        "Count": 1
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 20,
                                    "RPS": 1.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 420.7,
                                    "MaxMs": 1359.9,
                                    "Percent50": 271.0,
                                    "Percent75": 504.4,
                                    "Percent95": 651.3,
                                    "Percent99": 651.3,
                                    "StdDev": 188.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 16,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 3
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.46,
                                    "MaxKb": 0.91,
                                    "Percent50": 0.43,
                                    "Percent75": 0.54,
                                    "Percent95": 0.58,
                                    "Percent99": 0.58,
                                    "StdDev": 0.11,
                                    "AllMB": 0.01
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 13,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 13
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 247.3,
                                    "MaxMs": 1420.5,
                                    "Percent50": 247.4,
                                    "Percent75": 247.4,
                                    "Percent95": 247.4,
                                    "Percent99": 247.4,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.1,
                                    "MaxKb": 0.83,
                                    "Percent50": 0.1,
                                    "Percent75": 0.1,
                                    "Percent95": 0.1,
                                    "Percent99": 0.1,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 403,
                                        "IsError": true,
                                        "Message": "Forbidden",
                                        "Count": 1
                                    },
                                    {
                                        "Count@": 1,
                                        "StatusCode": 502,
                                        "IsError": true,
                                        "Message": "Bad Gateway",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 45,
                        "More800Less1200": 26,
                        "MoreOrEq1200": 14
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 9
                    },
                    "StatusCodes": [
                        {
                            "Count@": 48,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 48
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 400,
                            "IsError": true,
                            "Message": "Bad Request",
                            "Count": 1
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 401,
                            "IsError": true,
                            "Message": "Unauthorized",
                            "Count": 1
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 403,
                            "IsError": true,
                            "Message": "Forbidden",
                            "Count": 1
                        },
                        {
                            "Count@": 2,
                            "StatusCode": 502,
                            "IsError": true,
                            "Message": "Bad Gateway",
                            "Count": 2
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 503,
                            "IsError": true,
                            "Message": "Service Unavailable",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 5,
                    "Duration": "00:00:13"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "1574458357",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1583804868",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "2115592634",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "356109187",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1524824661",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "131797208",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        },
        {
            "Duration": "00:00:15",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 43,
                    "OkCount": 43,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 43,
                                    "RPS": 2.9
                                },
                                "Latency": {
                                    "MinMs": 17.0,
                                    "MeanMs": 744.3,
                                    "MaxMs": 1454.1,
                                    "Percent50": 740.7,
                                    "Percent75": 992.4,
                                    "Percent95": 1203.4,
                                    "Percent99": 1224.2,
                                    "StdDev": 340.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 25,
                                        "More800Less1200": 7,
                                        "MoreOrEq1200": 11
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 25,
                        "More800Less1200": 7,
                        "MoreOrEq1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "StatusCodes": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:15"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 85,
                    "OkCount": 84,
                    "FailCount": 1,
                    "AllDataMB": 0.04,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 84,
                                    "RPS": 5.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 728.2,
                                    "MaxMs": 1470.5,
                                    "Percent50": 699.7,
                                    "Percent75": 924.6,
                                    "Percent95": 1152.0,
                                    "Percent99": 1180.7,
                                    "StdDev": 308.1,
                                    "LatencyCount": {
                                        "LessOrEq800": 40,
                                        "More800Less1200": 31,
                                        "MoreOrEq1200": 13
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.41,
                                    "MaxKb": 0.97,
                                    "Percent50": 0.38,
                                    "Percent75": 0.55,
                                    "Percent95": 0.74,
                                    "Percent99": 0.75,
                                    "StdDev": 0.23,
                                    "AllMB": 0.04
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 84,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 84
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 131.7,
                                    "MaxMs": 1317.3,
                                    "Percent50": 131.7,
                                    "Percent75": 131.7,
                                    "Percent95": 131.7,
                                    "Percent99": 131.7,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.01,
                                    "MaxKb": 0.09,
                                    "Percent50": 0.01,
                                    "Percent75": 0.01,
                                    "Percent95": 0.01,
                                    "Percent99": 0.01,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 500,
                                        "IsError": true,
                                        "Message": "Internal Server Error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 40,
                        "More800Less1200": 31,
                        "MoreOrEq1200": 14
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "StatusCodes": [
                        {
                            "Count@": 84,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 84
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 500,
                            "IsError": true,
                            "Message": "Internal Server Error",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:15"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "1384256449",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "542971540",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1860551561",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "1969062186",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "910208197",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1066366184",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        },
        {
            "Duration": "00:00:20",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 88,
                    "OkCount": 88,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 88,
                                    "RPS": 4.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 751.2,
                                    "MaxMs": 1504.9,
                                    "Percent50": 527.0,
                                    "Percent75": 1018.3,
                                    "Percent95": 1096.6,
                                    "Percent99": 1101.3,
                                    "StdDev": 298.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 45,
                                        "More800Less1200": 21,
                                        "MoreOrEq1200": 22
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 45,
                        "More800Less1200": 21,
                        "MoreOrEq1200": 22
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "StatusCodes": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:20"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 137,
                    "OkCount": 136,
                    "FailCount": 1,
                    "AllDataMB": 0.07,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 136,
                                    "RPS": 6.8
                                },
                                "Latency": {
                                    "MinMs": 18.0,
                                    "MeanMs": 779.4,
                                    "MaxMs": 1486.8,
                                    "Percent50": 689.2,
                                    "Percent75": 993.8,
                                    "Percent95": 1178.2,
                                    "Percent99": 1215.2,
                                    "StdDev": 298.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 73,
                                        "More800Less1200": 44,
                                        "MoreOrEq1200": 19
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.01,
                                    "MeanKb": 0.46,
                                    "MaxKb": 0.97,
                                    "Percent50": 0.41,
                                    "Percent75": 0.59,
                                    "Percent95": 0.73,
                                    "Percent99": 0.75,
                                    "StdDev": 0.21,
                                    "AllMB": 0.07
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 136,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 136
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 77.5,
                                    "MaxMs": 1317.3,
                                    "Percent50": 77.5,
                                    "Percent75": 77.5,
                                    "Percent95": 77.5,
                                    "Percent99": 77.5,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.01,
                                    "MaxKb": 0.09,
                                    "Percent50": 0.01,
                                    "Percent75": 0.01,
                                    "Percent95": 0.01,
                                    "Percent99": 0.01,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 500,
                                        "IsError": true,
                                        "Message": "Internal Server Error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 73,
                        "More800Less1200": 44,
                        "MoreOrEq1200": 20
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "StatusCodes": [
                        {
                            "Count@": 136,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 136
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 500,
                            "IsError": true,
                            "Message": "Internal Server Error",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:20"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "112180903",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "617661534",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1319557861",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "1510580814",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1244196361",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "367022624",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        },
        {
            "Duration": "00:00:25",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 139,
                    "OkCount": 139,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 139,
                                    "RPS": 5.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 797.3,
                                    "MaxMs": 1504.9,
                                    "Percent50": 711.4,
                                    "Percent75": 1058.9,
                                    "Percent95": 1375.2,
                                    "Percent99": 1379.9,
                                    "StdDev": 425.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 72,
                                        "More800Less1200": 31,
                                        "MoreOrEq1200": 36
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 72,
                        "More800Less1200": 31,
                        "MoreOrEq1200": 36
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "StatusCodes": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:25"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 163,
                    "OkCount": 162,
                    "FailCount": 1,
                    "AllDataMB": 0.08,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 162,
                                    "RPS": 6.5
                                },
                                "Latency": {
                                    "MinMs": 15.5,
                                    "MeanMs": 744.4,
                                    "MaxMs": 1486.8,
                                    "Percent50": 754.9,
                                    "Percent75": 951.2,
                                    "Percent95": 1248.1,
                                    "Percent99": 1285.1,
                                    "StdDev": 342.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 88,
                                        "More800Less1200": 52,
                                        "MoreOrEq1200": 22
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.01,
                                    "MeanKb": 0.49,
                                    "MaxKb": 0.97,
                                    "Percent50": 0.48,
                                    "Percent75": 0.64,
                                    "Percent95": 0.85,
                                    "Percent99": 0.87,
                                    "StdDev": 0.25,
                                    "AllMB": 0.08
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 162,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 162
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 77.5,
                                    "MaxMs": 1317.3,
                                    "Percent50": 77.5,
                                    "Percent75": 77.5,
                                    "Percent95": 77.5,
                                    "Percent99": 77.5,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.01,
                                    "MaxKb": 0.09,
                                    "Percent50": 0.01,
                                    "Percent75": 0.01,
                                    "Percent95": 0.01,
                                    "Percent99": 0.01,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 500,
                                        "IsError": true,
                                        "Message": "Internal Server Error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 88,
                        "More800Less1200": 52,
                        "MoreOrEq1200": 23
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "StatusCodes": [
                        {
                            "Count@": 162,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 162
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 500,
                            "IsError": true,
                            "Message": "Internal Server Error",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:25"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "214707501",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1713937183",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "735359403",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "1426313622",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1271711627",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1412125875",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        },
        {
            "Duration": "00:00:30",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 198,
                    "OkCount": 198,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 198,
                                    "RPS": 6.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 797.4,
                                    "MaxMs": 1504.9,
                                    "Percent50": 726.6,
                                    "Percent75": 1064.7,
                                    "Percent95": 1407.8,
                                    "Percent99": 1429.8,
                                    "StdDev": 423.1,
                                    "LatencyCount": {
                                        "LessOrEq800": 104,
                                        "More800Less1200": 46,
                                        "MoreOrEq1200": 48
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 0,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 0.0,
                                    "MaxMs": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.0,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 104,
                        "More800Less1200": 46,
                        "MoreOrEq1200": 48
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "StatusCodes": [],
                    "CurrentOperation": 5,
                    "Duration": "00:00:30"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "1809190723",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "68810574",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1076272833",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "232207329",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "7643438",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1478961227",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        },
        {
            "Duration": "00:00:30",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 192,
                    "OkCount": 191,
                    "FailCount": 1,
                    "AllDataMB": 0.09,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 191,
                                    "RPS": 6.4
                                },
                                "Latency": {
                                    "MinMs": 15.5,
                                    "MeanMs": 739.2,
                                    "MaxMs": 1486.8,
                                    "Percent50": 729.8,
                                    "Percent75": 943.1,
                                    "Percent95": 1235.9,
                                    "Percent99": 1289.4,
                                    "StdDev": 331.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 103,
                                        "More800Less1200": 61,
                                        "MoreOrEq1200": 27
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.01,
                                    "MeanKb": 0.5,
                                    "MaxKb": 0.97,
                                    "Percent50": 0.48,
                                    "Percent75": 0.66,
                                    "Percent95": 0.85,
                                    "Percent99": 0.87,
                                    "StdDev": 0.25,
                                    "AllMB": 0.09
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 191,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 191
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 73.2,
                                    "MaxMs": 1317.3,
                                    "Percent50": 73.2,
                                    "Percent75": 73.2,
                                    "Percent95": 73.2,
                                    "Percent99": 73.2,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.0,
                                    "MaxKb": 0.09,
                                    "Percent50": 0.0,
                                    "Percent75": 0.0,
                                    "Percent95": 0.0,
                                    "Percent99": 0.0,
                                    "StdDev": 0.0,
                                    "AllMB": 0.0
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": 500,
                                        "IsError": true,
                                        "Message": "Internal Server Error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 103,
                        "More800Less1200": 61,
                        "MoreOrEq1200": 28
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "StatusCodes": [
                        {
                            "Count@": 191,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 191
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 500,
                            "IsError": true,
                            "Message": "Internal Server Error",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 5,
                    "Duration": "00:00:30"
                }
            ],
            "PluginStats": [
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin1": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin1",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Key",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Key": "Property1",
                                    "Value": "18320773",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1848932836",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "784333200",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                },
                {
                    "CaseSensitive": false,
                    "DataSetName": "NewDataSet",
                    "EnforceConstraints": true,
                    "ExtendedProperties": [],
                    "Locale": "en-US",
                    "Namespace": "",
                    "Prefix": "",
                    "RemotingFormat": 0,
                    "SchemaSerializationMode": 1,
                    "Tables": {
                        "CustomPlugin2": {
                            "CaseSensitive": false,
                            "DisplayExpression": "",
                            "Locale": "en-US",
                            "MinimumCapacity": 50,
                            "Namespace": "",
                            "Prefix": "",
                            "RemotingFormat": 0,
                            "TableName": "CustomPlugin2",
                            "Columns": [
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Property",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Property",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                },
                                {
                                    "AllowDBNull": true,
                                    "AutoIncrement": false,
                                    "AutoIncrementSeed": 0,
                                    "AutoIncrementStep": 1,
                                    "Caption": "Value",
                                    "ColumnMapping": 1,
                                    "ColumnName": "Value",
                                    "DataType": "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
                                    "DateTimeMode": 3,
                                    "DefaultValue": null,
                                    "Expression": "",
                                    "ExtendedProperties": [],
                                    "MaxLength": -1,
                                    "Namespace": "",
                                    "Prefix": "",
                                    "ReadOnly": false
                                }
                            ],
                            "Constraints": [],
                            "Rows": [
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 1",
                                    "Value": "2056057390",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "566052651",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1414310610",
                                    "RowState": 4
                                }
                            ]
                        }
                    },
                    "Relations": {}
                }
            ]
        }
    ],
    "Hints": [
        {
            "SourceName": "scenario_1",
            "SourceType": "Scenario",
            "Hint": "Step 'pull_html_1' in scenario 'scenario_1' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
        },
        {
            "SourceName": "scenario_3",
            "SourceType": "Scenario",
            "Hint": "Step 'pull_html_3' in scenario 'scenario_3' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
        },
        {
            "SourceName": "scenario_3",
            "SourceType": "Scenario",
            "Hint": "Step 'pull_html_5' in scenario 'scenario_3' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
        },
        {
            "SourceName": "CustomPlugin1",
            "SourceType": "WorkerPlugin",
            "Hint": "Test worker plugin hint"
        },
        {
            "SourceName": "CustomPlugin2",
            "SourceType": "WorkerPlugin",
            "Hint": "Test worker plugin hint"
        }
    ]
};
