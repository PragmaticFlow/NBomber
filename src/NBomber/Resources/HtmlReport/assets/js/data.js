const viewModel = {
    "NodeStats": {
        "RequestCount": 502,
        "OkCount": 431,
        "FailCount": 71,
        "AllDataMB": 0.121,
        "ScenarioStats": [
            {
                "ScenarioName": "scenario_1",
                "RequestCount": 195,
                "OkCount": 169,
                "FailCount": 26,
                "AllDataMB": 0.0,
                "StepStats": [
                    {
                        "StepName": "pull_html_1",
                        "Ok": {
                            "Request": {
                                "Count": 169,
                                "RPS": 5.6
                            },
                            "Latency": {
                                "MinMs": 9.83,
                                "MeanMs": 792.981,
                                "MaxMs": 1520.435,
                                "Percent50": 757.536,
                                "Percent75": 1110.537,
                                "Percent95": 1365.532,
                                "Percent99": 1379.831,
                                "StdDev": 392.519,
                                "LatencyCount": {
                                    "LessOrEq800": 87,
                                    "More800Less1200": 45,
                                    "MoreOrEq1200": 37
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
                            }
                        },
                        "Fail": {
                            "Request": {
                                "Count": 26,
                                "RPS": 0.9
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 531.71,
                                "MaxMs": 1415.578,
                                "Percent50": 461.731,
                                "Percent75": 640.763,
                                "Percent95": 645.53,
                                "Percent99": 645.53,
                                "StdDev": 100.399,
                                "LatencyCount": {
                                    "LessOrEq800": 14,
                                    "More800Less1200": 8,
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
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "unknown client's error",
                                    "Count": 26
                                }
                            ]
                        }
                    }
                ],
                "LatencyCount": {
                    "LessOrEq800": 101,
                    "More800Less1200": 53,
                    "MoreOrEq1200": 41
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 0
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 0,
                        "Message": "unknown client's error",
                        "Count": 26
                    }
                ],
                "CurrentOperation": 5,
                "Duration": "00:00:30"
            },
            {
                "ScenarioName": "scenario_2",
                "RequestCount": 193,
                "OkCount": 162,
                "FailCount": 31,
                "AllDataMB": 0.075,
                "StepStats": [
                    {
                        "StepName": "pull_html_2",
                        "Ok": {
                            "Request": {
                                "Count": 162,
                                "RPS": 5.4
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 690.412,
                                "MaxMs": 1520.435,
                                "Percent50": 620.771,
                                "Percent75": 979.399,
                                "Percent95": 1264.117,
                                "Percent99": 1278.68,
                                "StdDev": 358.069,
                                "LatencyCount": {
                                    "LessOrEq800": 89,
                                    "More800Less1200": 42,
                                    "MoreOrEq1200": 31
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.46,
                                "MaxKb": 0.999,
                                "Percent50": 0.416,
                                "Percent75": 0.657,
                                "Percent95": 0.841,
                                "Percent99": 0.878,
                                "StdDev": 0.265,
                                "AllMB": 0.075
                            }
                        },
                        "Fail": {
                            "Request": {
                                "Count": 31,
                                "RPS": 1.0
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 647.638,
                                "MaxMs": 1520.435,
                                "Percent50": 626.233,
                                "Percent75": 683.031,
                                "Percent95": 847.599,
                                "Percent99": 847.599,
                                "StdDev": 149.411,
                                "LatencyCount": {
                                    "LessOrEq800": 14,
                                    "More800Less1200": 10,
                                    "MoreOrEq1200": 7
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
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "unknown client's error",
                                    "Count": 31
                                }
                            ]
                        }
                    }
                ],
                "LatencyCount": {
                    "LessOrEq800": 103,
                    "More800Less1200": 52,
                    "MoreOrEq1200": 38
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 0
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 0,
                        "Message": "unknown client's error",
                        "Count": 31
                    }
                ],
                "CurrentOperation": 5,
                "Duration": "00:00:30"
            },
            {
                "ScenarioName": "scenario_3",
                "RequestCount": 114,
                "OkCount": 100,
                "FailCount": 14,
                "AllDataMB": 0.046,
                "StepStats": [
                    {
                        "StepName": "pull_html_3",
                        "Ok": {
                            "Request": {
                                "Count": 37,
                                "RPS": 2.6
                            },
                            "Latency": {
                                "MinMs": 47.514,
                                "MeanMs": 686.105,
                                "MaxMs": 1468.006,
                                "Percent50": 591.135,
                                "Percent75": 830.996,
                                "Percent95": 1038.09,
                                "Percent99": 1038.09,
                                "StdDev": 240.373,
                                "LatencyCount": {
                                    "LessOrEq800": 23,
                                    "More800Less1200": 11,
                                    "MoreOrEq1200": 3
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.046,
                                "MeanKb": 0.536,
                                "MaxKb": 0.968,
                                "Percent50": 0.481,
                                "Percent75": 0.669,
                                "Percent95": 0.824,
                                "Percent99": 0.824,
                                "StdDev": 0.209,
                                "AllMB": 0.019
                            }
                        },
                        "Fail": {
                            "Request": {
                                "Count": 6,
                                "RPS": 0.4
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 318.177,
                                "MaxMs": 1520.435,
                                "Percent50": 280.494,
                                "Percent75": 368.312,
                                "Percent95": 368.312,
                                "Percent99": 368.312,
                                "StdDev": 42.926,
                                "LatencyCount": {
                                    "LessOrEq800": 3,
                                    "More800Less1200": 1,
                                    "MoreOrEq1200": 2
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
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "Unauthorized",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "Bad Request",
                                    "Count": 2
                                }
                            ]
                        }
                    },
                    {
                        "StepName": "pull_html_4",
                        "Ok": {
                            "Request": {
                                "Count": 34,
                                "RPS": 2.4
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 580.228,
                                "MaxMs": 1520.435,
                                "Percent50": 524.288,
                                "Percent75": 752.353,
                                "Percent95": 888.668,
                                "Percent99": 888.668,
                                "StdDev": 214.051,
                                "LatencyCount": {
                                    "LessOrEq800": 17,
                                    "More800Less1200": 9,
                                    "MoreOrEq1200": 4
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.439,
                                "MaxKb": 0.968,
                                "Percent50": 0.45,
                                "Percent75": 0.527,
                                "Percent95": 0.552,
                                "Percent99": 0.552,
                                "StdDev": 0.094,
                                "AllMB": 0.015
                            }
                        },
                        "Fail": {
                            "Request": {
                                "Count": 3,
                                "RPS": 0.2
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 249.692,
                                "MaxMs": 1363.149,
                                "Percent50": 254.28,
                                "Percent75": 254.28,
                                "Percent95": 254.28,
                                "Percent99": 254.28,
                                "StdDev": 0.0,
                                "LatencyCount": {
                                    "LessOrEq800": 1,
                                    "More800Less1200": 1,
                                    "MoreOrEq1200": 1
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
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "Bad Request",
                                    "Count": 1
                                }
                            ]
                        }
                    },
                    {
                        "StepName": "pull_html_5",
                        "Ok": {
                            "Request": {
                                "Count": 29,
                                "RPS": 2.0
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 616.328,
                                "MaxMs": 1310.72,
                                "Percent50": 574.095,
                                "Percent75": 718.274,
                                "Percent95": 849.346,
                                "Percent99": 849.346,
                                "StdDev": 176.634,
                                "LatencyCount": {
                                    "LessOrEq800": 13,
                                    "More800Less1200": 9,
                                    "MoreOrEq1200": 2
                                }
                            },
                            "DataTransfer": {
                                "MinKb": 0.0,
                                "MeanKb": 0.358,
                                "MaxKb": 0.937,
                                "Percent50": 0.326,
                                "Percent75": 0.452,
                                "Percent95": 0.546,
                                "Percent99": 0.546,
                                "StdDev": 0.151,
                                "AllMB": 0.012
                            }
                        },
                        "Fail": {
                            "Request": {
                                "Count": 5,
                                "RPS": 0.3
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 327.107,
                                "MaxMs": 1363.149,
                                "Percent50": 333.742,
                                "Percent75": 333.742,
                                "Percent95": 333.742,
                                "Percent99": 333.742,
                                "StdDev": 0.0,
                                "LatencyCount": {
                                    "LessOrEq800": 2,
                                    "More800Less1200": 1,
                                    "MoreOrEq1200": 1
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
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "unknown client's error",
                                    "Count": 5
                                }
                            ]
                        }
                    }
                ],
                "LatencyCount": {
                    "LessOrEq800": 59,
                    "More800Less1200": 32,
                    "MoreOrEq1200": 13
                },
                "LoadSimulationStats": {
                    "SimulationName": "ramp_constant",
                    "Value": 0
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 401,
                        "Message": "Unauthorized",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 400,
                        "Message": "Bad Request",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 0,
                        "Message": "unknown client's error",
                        "Count": 5
                    }
                ],
                "CurrentOperation": 5,
                "Duration": "00:00:14"
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
                                "Value": "389785274",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Key": "Property2",
                                "Value": "1826083514",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Key": "Property3",
                                "Value": "143613083",
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
                                "Value": "2004633901",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Property": "Property 2",
                                "Value": "422304489",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Property": "Property 3",
                                "Value": "548269377",
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
            "NBomberVersion": "1.1.0"
        },
        "TestInfo": {
            "SessionId": "2021-03-01_09.15.28_session_a2230441",
            "TestSuite": "nbomber_default_test_suite_name",
            "TestName": "nbomber_default_test_name"
        },
        "ReportFiles": [],
        "Duration": "00:00:30"
    },
    "TimeLineStats": [
        {
            "Duration": "00:00:00",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 0,
                    "OkCount": 0,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [],
                    "LatencyCount": {
                        "LessOrEq800": 0,
                        "More800Less1200": 0,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 0
                    },
                    "ErrorStats": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:00"
                }
            ],
            "PluginStats": []
        },
        {
            "Duration": "00:00:00",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 0,
                    "OkCount": 0,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [],
                    "LatencyCount": {
                        "LessOrEq800": 0,
                        "More800Less1200": 0,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 0
                    },
                    "ErrorStats": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:00"
                }
            ],
            "PluginStats": []
        },
        {
            "Duration": "00:00:00",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 0,
                    "OkCount": 0,
                    "FailCount": 0,
                    "AllDataMB": 0.0,
                    "StepStats": [],
                    "LatencyCount": {
                        "LessOrEq800": 0,
                        "More800Less1200": 0,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 0
                    },
                    "ErrorStats": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:00"
                }
            ],
            "PluginStats": []
        },
        {
            "Duration": "00:00:05",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 4,
                    "OkCount": 3,
                    "FailCount": 1,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 281.941,
                                    "MaxMs": 891.29,
                                    "Percent50": 406.323,
                                    "Percent75": 406.323,
                                    "Percent95": 445.645,
                                    "Percent99": 445.645,
                                    "StdDev": 190.276,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 2,
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
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 432.538,
                                    "MaxMs": 891.29,
                                    "Percent50": 445.645,
                                    "Percent75": 445.645,
                                    "Percent95": 445.645,
                                    "Percent99": 445.645,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 1,
                        "More800Less1200": 3,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:05"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 7,
                    "OkCount": 7,
                    "FailCount": 0,
                    "AllDataMB": 0.003,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 7,
                                    "RPS": 1.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 526.29,
                                    "MaxMs": 1153.434,
                                    "Percent50": 567.979,
                                    "Percent75": 585.455,
                                    "Percent95": 751.479,
                                    "Percent99": 751.479,
                                    "StdDev": 180.672,
                                    "LatencyCount": {
                                        "LessOrEq800": 3,
                                        "More800Less1200": 4,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.332,
                                    "MaxKb": 0.937,
                                    "Percent50": 0.234,
                                    "Percent75": 0.395,
                                    "Percent95": 0.604,
                                    "Percent99": 0.604,
                                    "StdDev": 0.226,
                                    "AllMB": 0.003
                                }
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
                                "ErrorStats": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 3,
                        "More800Less1200": 4,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "ErrorStats": [],
                    "CurrentOperation": 3,
                    "Duration": "00:00:05"
                },
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 10,
                    "OkCount": 10,
                    "FailCount": 0,
                    "AllDataMB": 0.006,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.8
                                },
                                "Latency": {
                                    "MinMs": 111.411,
                                    "MeanMs": 420.523,
                                    "MaxMs": 943.718,
                                    "Percent50": 408.508,
                                    "Percent75": 452.198,
                                    "Percent95": 452.198,
                                    "Percent99": 452.198,
                                    "StdDev": 19.661,
                                    "LatencyCount": {
                                        "LessOrEq800": 3,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.132,
                                    "MeanKb": 0.424,
                                    "MaxKb": 0.624,
                                    "Percent50": 0.429,
                                    "Percent75": 0.442,
                                    "Percent95": 0.442,
                                    "Percent99": 0.442,
                                    "StdDev": 0.007,
                                    "AllMB": 0.001
                                }
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
                                "ErrorStats": []
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.8
                                },
                                "Latency": {
                                    "MinMs": 203.162,
                                    "MeanMs": 669.013,
                                    "MaxMs": 1101.005,
                                    "Percent50": 535.211,
                                    "Percent75": 834.492,
                                    "Percent95": 834.492,
                                    "Percent99": 834.492,
                                    "StdDev": 145.818,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 2,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.437,
                                    "MeanKb": 0.658,
                                    "MaxKb": 0.843,
                                    "Percent50": 0.603,
                                    "Percent75": 0.739,
                                    "Percent95": 0.739,
                                    "Percent99": 0.739,
                                    "StdDev": 0.066,
                                    "AllMB": 0.003
                                }
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
                                "ErrorStats": []
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 367.002,
                                    "MaxMs": 602.931,
                                    "Percent50": 375.74,
                                    "Percent75": 375.74,
                                    "Percent95": 375.74,
                                    "Percent99": 375.74,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.604,
                                    "MaxKb": 0.937,
                                    "Percent50": 0.614,
                                    "Percent75": 0.614,
                                    "Percent95": 0.614,
                                    "Percent99": 0.614,
                                    "StdDev": 0.0,
                                    "AllMB": 0.002
                                }
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
                                "ErrorStats": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 7,
                        "More800Less1200": 3,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "ErrorStats": [],
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
                                    "Value": "692914140",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1746958777",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "974047668",
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
                                    "Value": "1298566609",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1018346965",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "722013790",
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
                    "RequestCount": 20,
                    "OkCount": 17,
                    "FailCount": 3,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 17,
                                    "RPS": 1.7
                                },
                                "Latency": {
                                    "MinMs": 27.853,
                                    "MeanMs": 590.404,
                                    "MaxMs": 1468.006,
                                    "Percent50": 554.871,
                                    "Percent75": 707.789,
                                    "Percent95": 996.147,
                                    "Percent99": 996.147,
                                    "StdDev": 226.245,
                                    "LatencyCount": {
                                        "LessOrEq800": 11,
                                        "More800Less1200": 5,
                                        "MoreOrEq1200": 1
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
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 243.94,
                                    "MaxMs": 943.718,
                                    "Percent50": 297.096,
                                    "Percent75": 297.096,
                                    "Percent95": 314.573,
                                    "Percent99": 314.573,
                                    "StdDev": 75.513,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 2,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 3
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 12,
                        "More800Less1200": 7,
                        "MoreOrEq1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 3
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:10"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 35,
                    "OkCount": 30,
                    "FailCount": 5,
                    "AllDataMB": 0.013,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 30,
                                    "RPS": 3.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 545.588,
                                    "MaxMs": 1468.006,
                                    "Percent50": 444.708,
                                    "Percent75": 766.771,
                                    "Percent95": 1028.915,
                                    "Percent99": 1028.915,
                                    "StdDev": 321.129,
                                    "LatencyCount": {
                                        "LessOrEq800": 17,
                                        "More800Less1200": 8,
                                        "MoreOrEq1200": 5
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.348,
                                    "MaxKb": 0.968,
                                    "Percent50": 0.233,
                                    "Percent75": 0.52,
                                    "Percent95": 0.64,
                                    "Percent99": 0.64,
                                    "StdDev": 0.203,
                                    "AllMB": 0.013
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 5,
                                    "RPS": 0.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 516.798,
                                    "MaxMs": 1310.72,
                                    "Percent50": 477.477,
                                    "Percent75": 578.589,
                                    "Percent95": 578.589,
                                    "Percent99": 578.589,
                                    "StdDev": 49.62,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 3
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 5
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 19,
                        "More800Less1200": 8,
                        "MoreOrEq1200": 8
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 5
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:10"
                },
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 39,
                    "OkCount": 36,
                    "FailCount": 3,
                    "AllDataMB": 0.017,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 14,
                                    "RPS": 1.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 479.895,
                                    "MaxMs": 1468.006,
                                    "Percent50": 399.77,
                                    "Percent75": 585.143,
                                    "Percent95": 778.006,
                                    "Percent99": 778.006,
                                    "StdDev": 259.902,
                                    "LatencyCount": {
                                        "LessOrEq800": 9,
                                        "More800Less1200": 3,
                                        "MoreOrEq1200": 2
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.426,
                                    "MaxKb": 0.968,
                                    "Percent50": 0.388,
                                    "Percent75": 0.452,
                                    "Percent95": 0.564,
                                    "Percent99": 0.564,
                                    "StdDev": 0.095,
                                    "AllMB": 0.006
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 259.335,
                                    "MaxMs": 1520.435,
                                    "Percent50": 264.016,
                                    "Percent75": 264.016,
                                    "Percent95": 264.016,
                                    "Percent99": 264.016,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 1
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 401,
                                        "Message": "Unauthorized",
                                        "Count": 1
                                    },
                                    {
                                        "ErrorCode": 400,
                                        "Message": "Bad Request",
                                        "Count": 1
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 12,
                                    "RPS": 1.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 524.59,
                                    "MaxMs": 1520.435,
                                    "Percent50": 460.975,
                                    "Percent75": 529.905,
                                    "Percent95": 752.728,
                                    "Percent99": 752.728,
                                    "StdDev": 170.916,
                                    "LatencyCount": {
                                        "LessOrEq800": 7,
                                        "More800Less1200": 4,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.328,
                                    "MaxKb": 0.843,
                                    "Percent50": 0.34,
                                    "Percent75": 0.353,
                                    "Percent95": 0.438,
                                    "Percent99": 0.438,
                                    "StdDev": 0.09,
                                    "AllMB": 0.006
                                }
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
                                "ErrorStats": []
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 10,
                                    "RPS": 1.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 535.991,
                                    "MaxMs": 1153.434,
                                    "Percent50": 428.793,
                                    "Percent75": 681.574,
                                    "Percent95": 685.319,
                                    "Percent99": 685.319,
                                    "StdDev": 136.442,
                                    "LatencyCount": {
                                        "LessOrEq800": 6,
                                        "More800Less1200": 4,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.375,
                                    "MaxKb": 0.937,
                                    "Percent50": 0.302,
                                    "Percent75": 0.44,
                                    "Percent95": 0.512,
                                    "Percent99": 0.512,
                                    "StdDev": 0.114,
                                    "AllMB": 0.005
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 4.798,
                                    "MaxMs": 34.406,
                                    "Percent50": 4.915,
                                    "Percent75": 4.915,
                                    "Percent95": 4.915,
                                    "Percent99": 4.915,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 24,
                        "More800Less1200": 11,
                        "MoreOrEq1200": 4
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 401,
                            "Message": "Unauthorized",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "Bad Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
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
                                    "Value": "1296856745",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1561822799",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "355418103",
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
                                    "Value": "1593063984",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "495645920",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1224184581",
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
                    "RequestCount": 44,
                    "OkCount": 38,
                    "FailCount": 6,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 38,
                                    "RPS": 2.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 568.167,
                                    "MaxMs": 1468.006,
                                    "Percent50": 576.717,
                                    "Percent75": 807.403,
                                    "Percent95": 964.69,
                                    "Percent99": 1027.604,
                                    "StdDev": 301.621,
                                    "LatencyCount": {
                                        "LessOrEq800": 20,
                                        "More800Less1200": 14,
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
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 6,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 532.371,
                                    "MaxMs": 1205.862,
                                    "Percent50": 484.966,
                                    "Percent75": 660.603,
                                    "Percent95": 671.089,
                                    "Percent99": 671.089,
                                    "StdDev": 131.16,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 4,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 6
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 22,
                        "More800Less1200": 18,
                        "MoreOrEq1200": 4
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 6
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:15"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 85,
                    "OkCount": 72,
                    "FailCount": 13,
                    "AllDataMB": 0.032,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 72,
                                    "RPS": 4.8
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 683.762,
                                    "MaxMs": 1520.435,
                                    "Percent50": 632.422,
                                    "Percent75": 946.995,
                                    "Percent95": 1211.105,
                                    "Percent99": 1226.834,
                                    "StdDev": 395.285,
                                    "LatencyCount": {
                                        "LessOrEq800": 34,
                                        "More800Less1200": 20,
                                        "MoreOrEq1200": 18
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.453,
                                    "MaxKb": 0.968,
                                    "Percent50": 0.389,
                                    "Percent75": 0.632,
                                    "Percent95": 0.768,
                                    "Percent99": 0.799,
                                    "StdDev": 0.227,
                                    "AllMB": 0.032
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 13,
                                    "RPS": 0.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 641.106,
                                    "MaxMs": 1310.72,
                                    "Percent50": 639.631,
                                    "Percent75": 669.778,
                                    "Percent95": 706.478,
                                    "Percent99": 706.478,
                                    "StdDev": 43.285,
                                    "LatencyCount": {
                                        "LessOrEq800": 5,
                                        "More800Less1200": 5,
                                        "MoreOrEq1200": 3
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 13
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 39,
                        "More800Less1200": 25,
                        "MoreOrEq1200": 21
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 13
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:15"
                },
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 89,
                    "OkCount": 77,
                    "FailCount": 12,
                    "AllDataMB": 0.04,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 31,
                                    "RPS": 2.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 659.401,
                                    "MaxMs": 1468.006,
                                    "Percent50": 672.399,
                                    "Percent75": 754.975,
                                    "Percent95": 996.147,
                                    "Percent99": 996.147,
                                    "StdDev": 245.704,
                                    "LatencyCount": {
                                        "LessOrEq800": 18,
                                        "More800Less1200": 10,
                                        "MoreOrEq1200": 3
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.463,
                                    "MaxKb": 0.968,
                                    "Percent50": 0.448,
                                    "Percent75": 0.537,
                                    "Percent95": 0.701,
                                    "Percent99": 0.701,
                                    "StdDev": 0.173,
                                    "AllMB": 0.016
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 5,
                                    "RPS": 0.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 243.466,
                                    "MaxMs": 1520.435,
                                    "Percent50": 204.472,
                                    "Percent75": 292.291,
                                    "Percent95": 292.291,
                                    "Percent99": 292.291,
                                    "StdDev": 42.926,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 2
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 401,
                                        "Message": "Unauthorized",
                                        "Count": 3
                                    },
                                    {
                                        "ErrorCode": 400,
                                        "Message": "Bad Request",
                                        "Count": 2
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 27,
                                    "RPS": 1.8
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 604.009,
                                    "MaxMs": 1520.435,
                                    "Percent50": 526.909,
                                    "Percent75": 752.353,
                                    "Percent95": 888.668,
                                    "Percent99": 888.668,
                                    "StdDev": 203.712,
                                    "LatencyCount": {
                                        "LessOrEq800": 14,
                                        "More800Less1200": 9,
                                        "MoreOrEq1200": 4
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.444,
                                    "MaxKb": 0.968,
                                    "Percent50": 0.465,
                                    "Percent75": 0.527,
                                    "Percent95": 0.552,
                                    "Percent99": 0.552,
                                    "StdDev": 0.089,
                                    "AllMB": 0.014
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 249.692,
                                    "MaxMs": 1363.149,
                                    "Percent50": 254.28,
                                    "Percent75": 254.28,
                                    "Percent95": 254.28,
                                    "Percent99": 254.28,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 1
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 401,
                                        "Message": "Unauthorized",
                                        "Count": 2
                                    },
                                    {
                                        "ErrorCode": 400,
                                        "Message": "Bad Request",
                                        "Count": 1
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 19,
                                    "RPS": 1.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 557.029,
                                    "MaxMs": 1310.72,
                                    "Percent50": 546.57,
                                    "Percent75": 587.202,
                                    "Percent95": 718.274,
                                    "Percent99": 718.274,
                                    "StdDev": 121.994,
                                    "LatencyCount": {
                                        "LessOrEq800": 11,
                                        "More800Less1200": 7,
                                        "MoreOrEq1200": 1
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.365,
                                    "MaxKb": 0.937,
                                    "Percent50": 0.355,
                                    "Percent75": 0.404,
                                    "Percent95": 0.523,
                                    "Percent99": 0.523,
                                    "StdDev": 0.12,
                                    "AllMB": 0.01
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 327.107,
                                    "MaxMs": 1363.149,
                                    "Percent50": 333.742,
                                    "Percent75": 333.742,
                                    "Percent95": 333.742,
                                    "Percent99": 333.742,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 1
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 4
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 48,
                        "More800Less1200": 29,
                        "MoreOrEq1200": 12
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 401,
                            "Message": "Unauthorized",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "Bad Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 4
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
                                    "Value": "1086879855",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1620139795",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1502224672",
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
                                    "Value": "1631799005",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1393480528",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "2100408111",
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
            "Duration": "00:00:14",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 94,
                    "OkCount": 82,
                    "FailCount": 12,
                    "AllDataMB": 0.042,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 33,
                                    "RPS": 2.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 651.507,
                                    "MaxMs": 1468.006,
                                    "Percent50": 601.62,
                                    "Percent75": 789.053,
                                    "Percent95": 996.147,
                                    "Percent99": 996.147,
                                    "StdDev": 244.478,
                                    "LatencyCount": {
                                        "LessOrEq800": 19,
                                        "More800Less1200": 11,
                                        "MoreOrEq1200": 3
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.466,
                                    "MaxKb": 0.968,
                                    "Percent50": 0.459,
                                    "Percent75": 0.547,
                                    "Percent95": 0.701,
                                    "Percent99": 0.701,
                                    "StdDev": 0.17,
                                    "AllMB": 0.017
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 5,
                                    "RPS": 0.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 243.466,
                                    "MaxMs": 1520.435,
                                    "Percent50": 204.472,
                                    "Percent75": 292.291,
                                    "Percent95": 292.291,
                                    "Percent99": 292.291,
                                    "StdDev": 42.926,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 2
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 401,
                                        "Message": "Unauthorized",
                                        "Count": 3
                                    },
                                    {
                                        "ErrorCode": 400,
                                        "Message": "Bad Request",
                                        "Count": 2
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 28,
                                    "RPS": 1.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 597.183,
                                    "MaxMs": 1520.435,
                                    "Percent50": 526.909,
                                    "Percent75": 752.353,
                                    "Percent95": 888.668,
                                    "Percent99": 888.668,
                                    "StdDev": 202.631,
                                    "LatencyCount": {
                                        "LessOrEq800": 15,
                                        "More800Less1200": 9,
                                        "MoreOrEq1200": 4
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.443,
                                    "MaxKb": 0.968,
                                    "Percent50": 0.465,
                                    "Percent75": 0.527,
                                    "Percent95": 0.552,
                                    "Percent99": 0.552,
                                    "StdDev": 0.09,
                                    "AllMB": 0.014
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 249.692,
                                    "MaxMs": 1363.149,
                                    "Percent50": 254.28,
                                    "Percent75": 254.28,
                                    "Percent95": 254.28,
                                    "Percent99": 254.28,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 1
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 401,
                                        "Message": "Unauthorized",
                                        "Count": 2
                                    },
                                    {
                                        "ErrorCode": 400,
                                        "Message": "Bad Request",
                                        "Count": 1
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 21,
                                    "RPS": 1.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 533.763,
                                    "MaxMs": 1310.72,
                                    "Percent50": 474.481,
                                    "Percent75": 610.795,
                                    "Percent95": 744.489,
                                    "Percent99": 744.489,
                                    "StdDev": 165.432,
                                    "LatencyCount": {
                                        "LessOrEq800": 12,
                                        "More800Less1200": 7,
                                        "MoreOrEq1200": 2
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.335,
                                    "MaxKb": 0.937,
                                    "Percent50": 0.284,
                                    "Percent75": 0.404,
                                    "Percent95": 0.523,
                                    "Percent99": 0.523,
                                    "StdDev": 0.144,
                                    "AllMB": 0.011
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 327.107,
                                    "MaxMs": 1363.149,
                                    "Percent50": 333.742,
                                    "Percent75": 333.742,
                                    "Percent95": 333.742,
                                    "Percent99": 333.742,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 1
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 4
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 51,
                        "More800Less1200": 30,
                        "MoreOrEq1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 0
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 401,
                            "Message": "Unauthorized",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "Bad Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 4
                        }
                    ],
                    "CurrentOperation": 5,
                    "Duration": "00:00:14"
                }
            ],
            "PluginStats": []
        },
        {
            "Duration": "00:00:20",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_1",
                    "RequestCount": 88,
                    "OkCount": 78,
                    "FailCount": 10,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 78,
                                    "RPS": 3.9
                                },
                                "Latency": {
                                    "MinMs": 17.203,
                                    "MeanMs": 810.081,
                                    "MaxMs": 1520.435,
                                    "Percent50": 634.295,
                                    "Percent75": 1079.784,
                                    "Percent95": 1132.212,
                                    "Percent99": 1147.192,
                                    "StdDev": 282.443,
                                    "LatencyCount": {
                                        "LessOrEq800": 39,
                                        "More800Less1200": 21,
                                        "MoreOrEq1200": 18
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
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 10,
                                    "RPS": 0.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 165.218,
                                    "MaxMs": 1205.862,
                                    "Percent50": 149.172,
                                    "Percent75": 204.722,
                                    "Percent95": 209.715,
                                    "Percent99": 209.715,
                                    "StdDev": 40.58,
                                    "LatencyCount": {
                                        "LessOrEq800": 4,
                                        "More800Less1200": 6,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 10
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 43,
                        "More800Less1200": 27,
                        "MoreOrEq1200": 18
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 10
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:20"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 139,
                    "OkCount": 115,
                    "FailCount": 24,
                    "AllDataMB": 0.053,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 115,
                                    "RPS": 5.7
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 682.615,
                                    "MaxMs": 1520.435,
                                    "Percent50": 608.714,
                                    "Percent75": 943.718,
                                    "Percent95": 1082.5,
                                    "Percent99": 1097.921,
                                    "StdDev": 283.82,
                                    "LatencyCount": {
                                        "LessOrEq800": 59,
                                        "More800Less1200": 32,
                                        "MoreOrEq1200": 24
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.413,
                                    "MaxKb": 0.999,
                                    "Percent50": 0.367,
                                    "Percent75": 0.595,
                                    "Percent95": 0.667,
                                    "Percent99": 0.706,
                                    "StdDev": 0.2,
                                    "AllMB": 0.053
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 24,
                                    "RPS": 1.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 462.398,
                                    "MaxMs": 1310.72,
                                    "Percent50": 435.622,
                                    "Percent75": 486.508,
                                    "Percent95": 629.917,
                                    "Percent99": 629.917,
                                    "StdDev": 122.72,
                                    "LatencyCount": {
                                        "LessOrEq800": 11,
                                        "More800Less1200": 10,
                                        "MoreOrEq1200": 3
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 24
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 70,
                        "More800Less1200": 42,
                        "MoreOrEq1200": 27
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 24
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
                                    "Value": "2141043792",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "827307595",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "759955783",
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
                                    "Value": "2088582166",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "195828337",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1490946380",
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
                    "RequestCount": 135,
                    "OkCount": 116,
                    "FailCount": 19,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 116,
                                    "RPS": 4.6
                                },
                                "Latency": {
                                    "MinMs": 13.107,
                                    "MeanMs": 800.288,
                                    "MaxMs": 1520.435,
                                    "Percent50": 766.473,
                                    "Percent75": 1014.021,
                                    "Percent95": 1289.272,
                                    "Percent99": 1303.571,
                                    "StdDev": 368.508,
                                    "LatencyCount": {
                                        "LessOrEq800": 59,
                                        "More800Less1200": 31,
                                        "MoreOrEq1200": 26
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
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 19,
                                    "RPS": 0.8
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 473.771,
                                    "MaxMs": 1415.578,
                                    "Percent50": 465.901,
                                    "Percent75": 518.926,
                                    "Percent95": 523.692,
                                    "Percent99": 523.692,
                                    "StdDev": 38.735,
                                    "LatencyCount": {
                                        "LessOrEq800": 8,
                                        "More800Less1200": 8,
                                        "MoreOrEq1200": 3
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 19
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 67,
                        "More800Less1200": 39,
                        "MoreOrEq1200": 29
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 19
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:25"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 164,
                    "OkCount": 136,
                    "FailCount": 28,
                    "AllDataMB": 0.064,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 136,
                                    "RPS": 5.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 652.889,
                                    "MaxMs": 1520.435,
                                    "Percent50": 615.46,
                                    "Percent75": 876.64,
                                    "Percent95": 1179.648,
                                    "Percent99": 1195.068,
                                    "StdDev": 341.961,
                                    "LatencyCount": {
                                        "LessOrEq800": 75,
                                        "More800Less1200": 35,
                                        "MoreOrEq1200": 26
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.458,
                                    "MaxKb": 0.999,
                                    "Percent50": 0.398,
                                    "Percent75": 0.689,
                                    "Percent95": 0.796,
                                    "Percent99": 0.835,
                                    "StdDev": 0.259,
                                    "AllMB": 0.064
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 28,
                                    "RPS": 1.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 517.654,
                                    "MaxMs": 1520.435,
                                    "Percent50": 496.532,
                                    "Percent75": 547.418,
                                    "Percent95": 721.667,
                                    "Percent99": 721.667,
                                    "StdDev": 153.574,
                                    "LatencyCount": {
                                        "LessOrEq800": 14,
                                        "More800Less1200": 10,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 28
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 89,
                        "More800Less1200": 45,
                        "MoreOrEq1200": 30
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 28
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
                                    "Value": "579310590",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1779710904",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "638909594",
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
                                    "Value": "261897730",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "578489934",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "962496084",
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
                    "RequestCount": 188,
                    "OkCount": 162,
                    "FailCount": 26,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 162,
                                    "RPS": 5.4
                                },
                                "Latency": {
                                    "MinMs": 13.107,
                                    "MeanMs": 806.687,
                                    "MaxMs": 1520.435,
                                    "Percent50": 769.154,
                                    "Percent75": 1105.771,
                                    "Percent95": 1365.532,
                                    "Percent99": 1379.831,
                                    "StdDev": 388.911,
                                    "LatencyCount": {
                                        "LessOrEq800": 81,
                                        "More800Less1200": 44,
                                        "MoreOrEq1200": 37
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
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 26,
                                    "RPS": 0.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 531.71,
                                    "MaxMs": 1415.578,
                                    "Percent50": 461.731,
                                    "Percent75": 640.763,
                                    "Percent95": 645.53,
                                    "Percent99": 645.53,
                                    "StdDev": 100.399,
                                    "LatencyCount": {
                                        "LessOrEq800": 14,
                                        "More800Less1200": 8,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 26
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 95,
                        "More800Less1200": 52,
                        "MoreOrEq1200": 41
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 26
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:30"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 191,
                    "OkCount": 160,
                    "FailCount": 31,
                    "AllDataMB": 0.074,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 160,
                                    "RPS": 5.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 695.524,
                                    "MaxMs": 1520.435,
                                    "Percent50": 617.131,
                                    "Percent75": 988.137,
                                    "Percent95": 1264.117,
                                    "Percent99": 1278.68,
                                    "StdDev": 358.882,
                                    "LatencyCount": {
                                        "LessOrEq800": 87,
                                        "More800Less1200": 42,
                                        "MoreOrEq1200": 31
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.462,
                                    "MaxKb": 0.999,
                                    "Percent50": 0.416,
                                    "Percent75": 0.669,
                                    "Percent95": 0.841,
                                    "Percent99": 0.878,
                                    "StdDev": 0.265,
                                    "AllMB": 0.074
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 31,
                                    "RPS": 1.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 647.638,
                                    "MaxMs": 1520.435,
                                    "Percent50": 626.233,
                                    "Percent75": 683.031,
                                    "Percent95": 847.599,
                                    "Percent99": 847.599,
                                    "StdDev": 149.411,
                                    "LatencyCount": {
                                        "LessOrEq800": 14,
                                        "More800Less1200": 10,
                                        "MoreOrEq1200": 7
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 31
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 101,
                        "More800Less1200": 52,
                        "MoreOrEq1200": 38
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 31
                        }
                    ],
                    "CurrentOperation": 3,
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
                                    "Value": "2014709941",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1814928256",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1061936148",
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
                                    "Value": "1373539628",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1776990643",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "614835844",
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
                    "RequestCount": 195,
                    "OkCount": 169,
                    "FailCount": 26,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 169,
                                    "RPS": 5.6
                                },
                                "Latency": {
                                    "MinMs": 9.83,
                                    "MeanMs": 792.981,
                                    "MaxMs": 1520.435,
                                    "Percent50": 757.536,
                                    "Percent75": 1110.537,
                                    "Percent95": 1365.532,
                                    "Percent99": 1379.831,
                                    "StdDev": 392.519,
                                    "LatencyCount": {
                                        "LessOrEq800": 87,
                                        "More800Less1200": 45,
                                        "MoreOrEq1200": 37
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
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 26,
                                    "RPS": 0.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 531.71,
                                    "MaxMs": 1415.578,
                                    "Percent50": 461.731,
                                    "Percent75": 640.763,
                                    "Percent95": 645.53,
                                    "Percent99": 645.53,
                                    "StdDev": 100.399,
                                    "LatencyCount": {
                                        "LessOrEq800": 14,
                                        "More800Less1200": 8,
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 26
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 101,
                        "More800Less1200": 53,
                        "MoreOrEq1200": 41
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 0
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 26
                        }
                    ],
                    "CurrentOperation": 5,
                    "Duration": "00:00:30"
                }
            ],
            "PluginStats": []
        },
        {
            "Duration": "00:00:30",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 193,
                    "OkCount": 162,
                    "FailCount": 31,
                    "AllDataMB": 0.075,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 162,
                                    "RPS": 5.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 690.412,
                                    "MaxMs": 1520.435,
                                    "Percent50": 620.771,
                                    "Percent75": 979.399,
                                    "Percent95": 1264.117,
                                    "Percent99": 1278.68,
                                    "StdDev": 358.069,
                                    "LatencyCount": {
                                        "LessOrEq800": 89,
                                        "More800Less1200": 42,
                                        "MoreOrEq1200": 31
                                    }
                                },
                                "DataTransfer": {
                                    "MinKb": 0.0,
                                    "MeanKb": 0.46,
                                    "MaxKb": 0.999,
                                    "Percent50": 0.416,
                                    "Percent75": 0.657,
                                    "Percent95": 0.841,
                                    "Percent99": 0.878,
                                    "StdDev": 0.265,
                                    "AllMB": 0.075
                                }
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 31,
                                    "RPS": 1.0
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 647.638,
                                    "MaxMs": 1520.435,
                                    "Percent50": 626.233,
                                    "Percent75": 683.031,
                                    "Percent95": 847.599,
                                    "Percent99": 847.599,
                                    "StdDev": 149.411,
                                    "LatencyCount": {
                                        "LessOrEq800": 14,
                                        "More800Less1200": 10,
                                        "MoreOrEq1200": 7
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
                                "ErrorStats": [
                                    {
                                        "ErrorCode": 0,
                                        "Message": "unknown client's error",
                                        "Count": 31
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 103,
                        "More800Less1200": 52,
                        "MoreOrEq1200": 38
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 0
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "unknown client's error",
                            "Count": 31
                        }
                    ],
                    "CurrentOperation": 5,
                    "Duration": "00:00:30"
                }
            ],
            "PluginStats": []
        }
    ],
    "Hints": [
        {
            "SourceName": "scenario_1",
            "SourceType": "Scenario",
            "Hint": "Step 'pull_html_1' in scenario 'scenario_1' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
        },
        {
            "SourceName": "scenario_2",
            "SourceType": "Scenario",
            "Hint": "Step 'pull_html_2' in scenario 'scenario_2' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
        },
        {
            "SourceName": "scenario_3",
            "SourceType": "Scenario",
            "Hint": "Step 'pull_html_4' in scenario 'scenario_3' didn't track data transfer. In order to track data transfer, you should use Response.Ok(sizeInBytes: value)"
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
