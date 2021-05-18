const viewModel = {
    "NodeStats": {
        "RequestCount": 594,
        "OkCount": 379,
        "FailCount": 215,
        "AllBytes": 120497,
        "ScenarioStats": [
            {
                "ScenarioName": "scenario_1",
                "RequestCount": 209,
                "OkCount": 151,
                "FailCount": 58,
                "AllBytes": 0,
                "StepStats": [
                    {
                        "StepName": "pull_html_1",
                        "Ok": {
                            "Request": {
                                "Count": 151,
                                "RPS": 5.0
                            },
                            "Latency": {
                                "MinMs": 25.6,
                                "MeanMs": 532.6,
                                "MaxMs": 1016.6,
                                "Percent50": 532.5,
                                "Percent75": 673.6,
                                "Percent95": 830.6,
                                "Percent99": 837.9,
                                "StdDev": 221.3,
                                "LatencyCount": {
                                    "LessOrEq800": 117,
                                    "More800Less1200": 34,
                                    "MoreOrEq1200": 0
                                }
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
                            "StatusCodes": []
                        },
                        "Fail": {
                            "Request": {
                                "Count": 58,
                                "RPS": 1.9
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 960.5,
                                "MaxMs": 1019.1,
                                "Percent50": 960.1,
                                "Percent75": 962.5,
                                "Percent95": 963.9,
                                "Percent99": 963.9,
                                "StdDev": 2.6,
                                "LatencyCount": {
                                    "LessOrEq800": 0,
                                    "More800Less1200": 58,
                                    "MoreOrEq1200": 0
                                }
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
                            "StatusCodes": [
                                {
                                    "Count@": 58,
                                    "StatusCode": -100,
                                    "IsError": true,
                                    "Message": "step timeout",
                                    "Count": 58
                                }
                            ]
                        }
                    }
                ],
                "LatencyCount": {
                    "LessOrEq800": 117,
                    "More800Less1200": 92,
                    "MoreOrEq1200": 0
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 10
                },
                "StatusCodes": [
                    {
                        "Count@": 58,
                        "StatusCode": -100,
                        "IsError": true,
                        "Message": "step timeout",
                        "Count": 58
                    }
                ],
                "CurrentOperation": 5,
                "Duration": "00:00:30"
            },
            {
                "ScenarioName": "scenario_2",
                "RequestCount": 206,
                "OkCount": 136,
                "FailCount": 70,
                "AllBytes": 68286,
                "StepStats": [
                    {
                        "StepName": "pull_html_2",
                        "Ok": {
                            "Request": {
                                "Count": 136,
                                "RPS": 4.5
                            },
                            "Latency": {
                                "MinMs": 10.2,
                                "MeanMs": 565.9,
                                "MaxMs": 984.7,
                                "Percent50": 509.0,
                                "Percent75": 706.1,
                                "Percent95": 887.6,
                                "Percent99": 907.6,
                                "StdDev": 232.1,
                                "LatencyCount": {
                                    "LessOrEq800": 105,
                                    "More800Less1200": 31,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 32,
                                "MeanBytes": 493,
                                "MaxBytes": 998,
                                "Percent50": 447,
                                "Percent75": 647,
                                "Percent95": 829,
                                "Percent99": 845,
                                "StdDev": 233.9,
                                "AllBytes": 67501
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
                                "Count": 70,
                                "RPS": 2.3
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 947.0,
                                "MaxMs": 1019.9,
                                "Percent50": 953.5,
                                "Percent75": 954.8,
                                "Percent95": 957.3,
                                "Percent99": 957.3,
                                "StdDev": 19.9,
                                "LatencyCount": {
                                    "LessOrEq800": 1,
                                    "More800Less1200": 69,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 0,
                                "MeanBytes": 43,
                                "MaxBytes": 785,
                                "Percent50": 43,
                                "Percent75": 43,
                                "Percent95": 43,
                                "Percent99": 43,
                                "StdDev": 0.0,
                                "AllBytes": 785
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 69,
                                    "StatusCode": -100,
                                    "IsError": true,
                                    "Message": "step timeout",
                                    "Count": 69
                                },
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
                    "LessOrEq800": 106,
                    "More800Less1200": 100,
                    "MoreOrEq1200": 0
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 5
                },
                "StatusCodes": [
                    {
                        "Count@": 69,
                        "StatusCode": -100,
                        "IsError": true,
                        "Message": "step timeout",
                        "Count": 69
                    },
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
                "CurrentOperation": 5,
                "Duration": "00:00:30"
            },
            {
                "ScenarioName": "scenario_3",
                "RequestCount": 179,
                "OkCount": 92,
                "FailCount": 87,
                "AllBytes": 52211,
                "StepStats": [
                    {
                        "StepName": "pull_html_3",
                        "Ok": {
                            "Request": {
                                "Count": 49,
                                "RPS": 2.7
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 224.7,
                                "MaxMs": 1012.5,
                                "Percent50": 182.8,
                                "Percent75": 285.8,
                                "Percent95": 323.8,
                                "Percent99": 323.8,
                                "StdDev": 72.7,
                                "LatencyCount": {
                                    "LessOrEq800": 44,
                                    "More800Less1200": 5,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 0,
                                "MeanBytes": 246,
                                "MaxBytes": 993,
                                "Percent50": 203,
                                "Percent75": 305,
                                "Percent95": 348,
                                "Percent99": 348,
                                "StdDev": 76.0,
                                "AllBytes": 21359
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 29,
                                    "StatusCode": 200,
                                    "IsError": false,
                                    "Message": "",
                                    "Count": 29
                                }
                            ]
                        },
                        "Fail": {
                            "Request": {
                                "Count": 53,
                                "RPS": 2.9
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 631.3,
                                "MaxMs": 1017.4,
                                "Percent50": 656.7,
                                "Percent75": 657.7,
                                "Percent95": 658.7,
                                "Percent99": 658.7,
                                "StdDev": 46.3,
                                "LatencyCount": {
                                    "LessOrEq800": 7,
                                    "More800Less1200": 46,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 0,
                                "MeanBytes": 144,
                                "MaxBytes": 818,
                                "Percent50": 142,
                                "Percent75": 146,
                                "Percent95": 146,
                                "Percent99": 146,
                                "StdDev": 2.1,
                                "AllBytes": 4757
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 45,
                                    "StatusCode": -100,
                                    "IsError": true,
                                    "Message": "step timeout",
                                    "Count": 45
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
                            ]
                        }
                    },
                    {
                        "StepName": "pull_html_4",
                        "Ok": {
                            "Request": {
                                "Count": 28,
                                "RPS": 1.6
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 305.8,
                                "MaxMs": 999.4,
                                "Percent50": 291.2,
                                "Percent75": 331.3,
                                "Percent95": 345.2,
                                "Percent99": 345.2,
                                "StdDev": 37.4,
                                "LatencyCount": {
                                    "LessOrEq800": 19,
                                    "More800Less1200": 9,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 0,
                                "MeanBytes": 283,
                                "MaxBytes": 988,
                                "Percent50": 244,
                                "Percent75": 314,
                                "Percent95": 356,
                                "Percent99": 356,
                                "StdDev": 59.7,
                                "AllBytes": 14550
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 21,
                                    "StatusCode": 200,
                                    "IsError": false,
                                    "Message": "",
                                    "Count": 21
                                }
                            ]
                        },
                        "Fail": {
                            "Request": {
                                "Count": 21,
                                "RPS": 1.2
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 299.8,
                                "MaxMs": 1015.8,
                                "Percent50": 279.8,
                                "Percent75": 324.3,
                                "Percent95": 325.1,
                                "Percent99": 325.1,
                                "StdDev": 25.3,
                                "LatencyCount": {
                                    "LessOrEq800": 6,
                                    "More800Less1200": 15,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 0,
                                "MeanBytes": 27,
                                "MaxBytes": 614,
                                "Percent50": 20,
                                "Percent75": 34,
                                "Percent95": 34,
                                "Percent99": 34,
                                "StdDev": 7.0,
                                "AllBytes": 1504
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 16,
                                    "StatusCode": -100,
                                    "IsError": true,
                                    "Message": "step timeout",
                                    "Count": 16
                                },
                                {
                                    "Count@": 1,
                                    "StatusCode": 500,
                                    "IsError": true,
                                    "Message": "Internal Server Error",
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
                            ]
                        }
                    },
                    {
                        "StepName": "pull_html_5",
                        "Ok": {
                            "Request": {
                                "Count": 15,
                                "RPS": 0.8
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 216.0,
                                "MaxMs": 996.1,
                                "Percent50": 202.1,
                                "Percent75": 220.8,
                                "Percent95": 234.8,
                                "Percent99": 234.8,
                                "StdDev": 16.0,
                                "LatencyCount": {
                                    "LessOrEq800": 13,
                                    "More800Less1200": 2,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 0,
                                "MeanBytes": 237,
                                "MaxBytes": 991,
                                "Percent50": 214,
                                "Percent75": 263,
                                "Percent95": 270,
                                "Percent99": 270,
                                "StdDev": 31.2,
                                "AllBytes": 9242
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 8,
                                    "StatusCode": 200,
                                    "IsError": false,
                                    "Message": "",
                                    "Count": 8
                                }
                            ]
                        },
                        "Fail": {
                            "Request": {
                                "Count": 13,
                                "RPS": 0.7
                            },
                            "Latency": {
                                "MinMs": 0.0,
                                "MeanMs": 277.7,
                                "MaxMs": 1014.2,
                                "Percent50": 266.7,
                                "Percent75": 288.9,
                                "Percent95": 288.9,
                                "Percent99": 288.9,
                                "StdDev": 11.1,
                                "LatencyCount": {
                                    "LessOrEq800": 3,
                                    "More800Less1200": 9,
                                    "MoreOrEq1200": 0
                                }
                            },
                            "DataTransfer": {
                                "MinBytes": 0,
                                "MeanBytes": 27,
                                "MaxBytes": 799,
                                "Percent50": 27,
                                "Percent75": 27,
                                "Percent95": 27,
                                "Percent99": 27,
                                "StdDev": 0.0,
                                "AllBytes": 799
                            },
                            "StatusCodes": [
                                {
                                    "Count@": 11,
                                    "StatusCode": -100,
                                    "IsError": true,
                                    "Message": "step timeout",
                                    "Count": 11
                                },
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
                    "LessOrEq800": 92,
                    "More800Less1200": 86,
                    "MoreOrEq1200": 0
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 10
                },
                "StatusCodes": [
                    {
                        "Count@": 72,
                        "StatusCode": -100,
                        "IsError": true,
                        "Message": "step timeout",
                        "Count": 72
                    },
                    {
                        "Count@": 58,
                        "StatusCode": 200,
                        "IsError": false,
                        "Message": "",
                        "Count": 58
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
                        "StatusCode": 500,
                        "IsError": true,
                        "Message": "Internal Server Error",
                        "Count": 2
                    },
                    {
                        "Count@": 3,
                        "StatusCode": 502,
                        "IsError": true,
                        "Message": "Bad Gateway",
                        "Count": 3
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
                "Duration": "00:00:17"
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
                                "Value": "1958891016",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Key": "Property2",
                                "Value": "441860245",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Key": "Property3",
                                "Value": "2106844768",
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
                                "Value": "733347866",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Property": "Property 2",
                                "Value": "2029391232",
                                "RowState": 4
                            },
                            {
                                "OriginalRow": null,
                                "Property": "Property 3",
                                "Value": "1426080878",
                                "RowState": 4
                            }
                        ]
                    }
                },
                "Relations": {}
            }
        ],
        "NodeInfo": {
            "MachineName": "DESKTOP",
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
            "SessionId": "2021-04-17_09.54.94_session_5faa29ab",
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
                    "RequestCount": 3,
                    "OkCount": 2,
                    "FailCount": 1,
                    "AllBytes": 0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 406.2,
                                    "MaxMs": 877.4,
                                    "Percent50": 374.0,
                                    "Percent75": 438.7,
                                    "Percent95": 438.7,
                                    "Percent99": 438.7,
                                    "StdDev": 32.3,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 500.7,
                                    "MaxMs": 1001.9,
                                    "Percent50": 500.9,
                                    "Percent75": 500.9,
                                    "Percent95": 500.9,
                                    "Percent99": 500.9,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 1,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 1
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 1,
                        "More800Less1200": 2,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "StatusCodes": [
                        {
                            "Count@": 1,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 1
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:05"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 8,
                    "OkCount": 3,
                    "FailCount": 5,
                    "AllBytes": 2610,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 418.3,
                                    "MaxMs": 813.5,
                                    "Percent50": 340.6,
                                    "Percent75": 496.3,
                                    "Percent95": 496.3,
                                    "Percent99": 496.3,
                                    "StdDev": 77.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 469,
                                    "MaxBytes": 990,
                                    "Percent50": 404,
                                    "Percent75": 533,
                                    "Percent95": 533,
                                    "Percent99": 533,
                                    "StdDev": 64.5,
                                    "AllBytes": 1825
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
                                    "Count": 5,
                                    "RPS": 1.0
                                },
                                "Latency": {
                                    "MinMs": 41.6,
                                    "MeanMs": 898.0,
                                    "MaxMs": 1012.5,
                                    "Percent50": 1004.9,
                                    "Percent75": 1004.9,
                                    "Percent95": 1005.2,
                                    "Percent99": 1005.2,
                                    "StdDev": 150.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 4,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 261,
                                    "MaxBytes": 785,
                                    "Percent50": 261,
                                    "Percent75": 261,
                                    "Percent95": 261,
                                    "Percent99": 261,
                                    "StdDev": 0.0,
                                    "AllBytes": 785
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 4,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 4
                                    },
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
                        "More800Less1200": 5,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "StatusCodes": [
                        {
                            "Count@": 4,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 4
                        },
                        {
                            "Count@": 3,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 3
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
                    "RequestCount": 9,
                    "OkCount": 5,
                    "FailCount": 4,
                    "AllBytes": 3585,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.8
                                },
                                "Latency": {
                                    "MinMs": 30.1,
                                    "MeanMs": 443.8,
                                    "MaxMs": 745.9,
                                    "Percent50": 324.6,
                                    "Percent75": 563.2,
                                    "Percent95": 563.2,
                                    "Percent99": 563.2,
                                    "StdDev": 119.3,
                                    "LatencyCount": {
                                        "LessOrEq800": 4,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 411,
                                    "MeanBytes": 689,
                                    "MaxBytes": 991,
                                    "Percent50": 634,
                                    "Percent75": 745,
                                    "Percent95": 745,
                                    "Percent99": 745,
                                    "StdDev": 55.5,
                                    "AllBytes": 2735
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 2,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 2
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 335.7,
                                    "MaxMs": 1013.4,
                                    "Percent50": 334.0,
                                    "Percent75": 337.8,
                                    "Percent95": 337.8,
                                    "Percent99": 337.8,
                                    "StdDev": 1.9,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 2,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 2,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 2
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 1,
                                    "RPS": 0.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 20.2,
                                    "MaxMs": 60.5,
                                    "Percent50": 20.2,
                                    "Percent75": 20.2,
                                    "Percent95": 20.2,
                                    "Percent99": 20.2,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 283,
                                    "MaxBytes": 850,
                                    "Percent50": 283,
                                    "Percent75": 283,
                                    "Percent95": 283,
                                    "Percent99": 283,
                                    "StdDev": 0.0,
                                    "AllBytes": 850
                                },
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 2,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 670.7,
                                    "MaxMs": 1010.9,
                                    "Percent50": 670.9,
                                    "Percent75": 670.9,
                                    "Percent95": 670.9,
                                    "Percent99": 670.9,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 2,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 2,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 2
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
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
                                "StatusCodes": []
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 5,
                        "More800Less1200": 4,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "StatusCodes": [
                        {
                            "Count@": 4,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 4
                        },
                        {
                            "Count@": 2,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 2
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
                                    "Value": "1638580369",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "508760032",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "24064058",
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
                                    "Value": "60367864",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "861828910",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "427398048",
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
                    "RequestCount": 22,
                    "OkCount": 18,
                    "FailCount": 4,
                    "AllBytes": 0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 18,
                                    "RPS": 1.8
                                },
                                "Latency": {
                                    "MinMs": 33.9,
                                    "MeanMs": 534.9,
                                    "MaxMs": 984.7,
                                    "Percent50": 493.4,
                                    "Percent75": 695.8,
                                    "Percent95": 879.8,
                                    "Percent99": 879.8,
                                    "StdDev": 241.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 15,
                                        "More800Less1200": 3,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 4,
                                    "RPS": 0.4
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 670.3,
                                    "MaxMs": 1015.0,
                                    "Percent50": 671.5,
                                    "Percent75": 671.5,
                                    "Percent95": 672.3,
                                    "Percent99": 672.3,
                                    "StdDev": 1.9,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 4,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 4,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 4
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 15,
                        "More800Less1200": 7,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "StatusCodes": [
                        {
                            "Count@": 4,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 4
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:10"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 38,
                    "OkCount": 25,
                    "FailCount": 13,
                    "AllBytes": 16078,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 25,
                                    "RPS": 2.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 460.0,
                                    "MaxMs": 954.4,
                                    "Percent50": 416.3,
                                    "Percent75": 552.6,
                                    "Percent95": 662.6,
                                    "Percent99": 662.6,
                                    "StdDev": 154.6,
                                    "LatencyCount": {
                                        "LessOrEq800": 15,
                                        "More800Less1200": 10,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 426,
                                    "MaxBytes": 990,
                                    "Percent50": 414,
                                    "Percent75": 522,
                                    "Percent95": 645,
                                    "Percent99": 645,
                                    "StdDev": 174.0,
                                    "AllBytes": 15293
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 25,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 25
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 13,
                                    "RPS": 1.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 692.0,
                                    "MaxMs": 1014.2,
                                    "Percent50": 719.4,
                                    "Percent75": 720.8,
                                    "Percent95": 722.1,
                                    "Percent99": 722.1,
                                    "StdDev": 56.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 12,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 112,
                                    "MaxBytes": 785,
                                    "Percent50": 112,
                                    "Percent75": 112,
                                    "Percent95": 112,
                                    "Percent99": 112,
                                    "StdDev": 0.0,
                                    "AllBytes": 785
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 12,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 12
                                    },
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
                        "LessOrEq800": 16,
                        "More800Less1200": 22,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "StatusCodes": [
                        {
                            "Count@": 12,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 12
                        },
                        {
                            "Count@": 25,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 25
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
                    "RequestCount": 42,
                    "OkCount": 25,
                    "FailCount": 17,
                    "AllBytes": 11682,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 15,
                                    "RPS": 1.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 252.8,
                                    "MaxMs": 1012.5,
                                    "Percent50": 107.5,
                                    "Percent75": 338.8,
                                    "Percent95": 500.1,
                                    "Percent99": 500.1,
                                    "StdDev": 182.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 13,
                                        "More800Less1200": 2,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 313,
                                    "MaxBytes": 991,
                                    "Percent50": 283,
                                    "Percent75": 349,
                                    "Percent95": 449,
                                    "Percent99": 449,
                                    "StdDev": 103.0,
                                    "AllBytes": 6651
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 9,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 9
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 9,
                                    "RPS": 0.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 705.3,
                                    "MaxMs": 1014.2,
                                    "Percent50": 705.7,
                                    "Percent75": 706.0,
                                    "Percent95": 706.5,
                                    "Percent99": 706.5,
                                    "StdDev": 0.9,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 9,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 72,
                                    "MaxBytes": 508,
                                    "Percent50": 72,
                                    "Percent75": 72,
                                    "Percent95": 72,
                                    "Percent99": 72,
                                    "StdDev": 0.0,
                                    "AllBytes": 508
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 8,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 8
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 7,
                                    "RPS": 0.7
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 265.2,
                                    "MaxMs": 765.5,
                                    "Percent50": 273.2,
                                    "Percent75": 282.3,
                                    "Percent95": 284.0,
                                    "Percent99": 284.0,
                                    "StdDev": 23.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 7,
                                        "More800Less1200": 0,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 212,
                                    "MaxBytes": 988,
                                    "Percent50": 147,
                                    "Percent75": 275,
                                    "Percent95": 322,
                                    "Percent99": 322,
                                    "StdDev": 101.1,
                                    "AllBytes": 3084
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 4,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 4
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 5,
                                    "RPS": 0.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 433.1,
                                    "MaxMs": 1015.8,
                                    "Percent50": 432.7,
                                    "Percent75": 433.9,
                                    "Percent95": 433.9,
                                    "Percent99": 433.9,
                                    "StdDev": 0.6,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 5,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 5,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 5
                                    }
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 3,
                                    "RPS": 0.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 190.2,
                                    "MaxMs": 893.7,
                                    "Percent50": 190.3,
                                    "Percent75": 190.3,
                                    "Percent95": 190.3,
                                    "Percent99": 190.3,
                                    "StdDev": 0.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 2,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 205,
                                    "MaxBytes": 940,
                                    "Percent50": 205,
                                    "Percent75": 205,
                                    "Percent95": 205,
                                    "Percent99": 205,
                                    "StdDev": 0.0,
                                    "AllBytes": 1439
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
                                    "Count": 3,
                                    "RPS": 0.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 289.1,
                                    "MaxMs": 1014.2,
                                    "Percent50": 288.9,
                                    "Percent75": 289.4,
                                    "Percent95": 289.4,
                                    "Percent99": 289.4,
                                    "StdDev": 0.2,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 3,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 3,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 3
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 22,
                        "More800Less1200": 20,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "StatusCodes": [
                        {
                            "Count@": 16,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 16
                        },
                        {
                            "Count@": 14,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 14
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
                                    "Value": "1824427894",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1485055958",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "882574567",
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
                                    "Value": "567906406",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1691513742",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "1219022426",
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
                    "RequestCount": 49,
                    "OkCount": 38,
                    "FailCount": 11,
                    "AllBytes": 0,
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
                                    "MeanMs": 481.8,
                                    "MaxMs": 1016.6,
                                    "Percent50": 529.6,
                                    "Percent75": 602.5,
                                    "Percent95": 747.1,
                                    "Percent99": 778.1,
                                    "StdDev": 206.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 29,
                                        "More800Less1200": 9,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 11,
                                    "RPS": 0.7
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 804.4,
                                    "MaxMs": 1015.0,
                                    "Percent50": 803.5,
                                    "Percent75": 806.4,
                                    "Percent95": 807.4,
                                    "Percent99": 807.4,
                                    "StdDev": 2.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 11,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 11,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 11
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 29,
                        "More800Less1200": 20,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "StatusCodes": [
                        {
                            "Count@": 11,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 11
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:15"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 95,
                    "OkCount": 58,
                    "FailCount": 37,
                    "AllBytes": 32877,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 58,
                                    "RPS": 3.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 462.8,
                                    "MaxMs": 954.4,
                                    "Percent50": 414.6,
                                    "Percent75": 581.8,
                                    "Percent95": 682.4,
                                    "Percent99": 708.6,
                                    "StdDev": 179.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 45,
                                        "More800Less1200": 13,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 462,
                                    "MaxBytes": 990,
                                    "Percent50": 459,
                                    "Percent75": 580,
                                    "Percent95": 738,
                                    "Percent99": 741,
                                    "StdDev": 201.2,
                                    "AllBytes": 32092
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 58,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 58
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 37,
                                    "RPS": 2.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 794.8,
                                    "MaxMs": 1015.0,
                                    "Percent50": 806.8,
                                    "Percent75": 809.0,
                                    "Percent95": 810.8,
                                    "Percent99": 810.8,
                                    "StdDev": 34.5,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 36,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 78,
                                    "MaxBytes": 785,
                                    "Percent50": 78,
                                    "Percent75": 78,
                                    "Percent95": 78,
                                    "Percent99": 78,
                                    "StdDev": 0.0,
                                    "AllBytes": 785
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 36,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 36
                                    },
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
                        "LessOrEq800": 46,
                        "More800Less1200": 49,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "StatusCodes": [
                        {
                            "Count@": 36,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 36
                        },
                        {
                            "Count@": 58,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 58
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
                },
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 106,
                    "OkCount": 57,
                    "FailCount": 49,
                    "AllBytes": 34322,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 33,
                                    "RPS": 2.2
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 360.3,
                                    "MaxMs": 1012.5,
                                    "Percent50": 271.4,
                                    "Percent75": 460.3,
                                    "Percent95": 596.3,
                                    "Percent99": 596.3,
                                    "StdDev": 157.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 29,
                                        "More800Less1200": 4,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 489,
                                    "MaxBytes": 993,
                                    "Percent50": 425,
                                    "Percent75": 563,
                                    "Percent95": 707,
                                    "Percent99": 707,
                                    "StdDev": 151.3,
                                    "AllBytes": 16414
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
                                    "Count": 29,
                                    "RPS": 1.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 708.6,
                                    "MaxMs": 1015.8,
                                    "Percent50": 722.5,
                                    "Percent75": 735.9,
                                    "Percent95": 737.6,
                                    "Percent99": 737.6,
                                    "StdDev": 47.9,
                                    "LatencyCount": {
                                        "LessOrEq800": 4,
                                        "More800Less1200": 25,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 242,
                                    "MaxBytes": 637,
                                    "Percent50": 236,
                                    "Percent75": 249,
                                    "Percent95": 249,
                                    "Percent99": 249,
                                    "StdDev": 6.0,
                                    "AllBytes": 2998
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 24,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 24
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
                                    "Count": 16,
                                    "RPS": 1.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 426.4,
                                    "MaxMs": 999.4,
                                    "Percent50": 353.6,
                                    "Percent75": 469.8,
                                    "Percent95": 548.8,
                                    "Percent99": 548.8,
                                    "StdDev": 109.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 12,
                                        "More800Less1200": 4,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 487,
                                    "MaxBytes": 988,
                                    "Percent50": 360,
                                    "Percent75": 575,
                                    "Percent95": 672,
                                    "Percent99": 672,
                                    "StdDev": 166.0,
                                    "AllBytes": 9272
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 12,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 12
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 13,
                                    "RPS": 0.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 573.4,
                                    "MaxMs": 1015.8,
                                    "Percent50": 534.2,
                                    "Percent75": 613.2,
                                    "Percent95": 613.7,
                                    "Percent99": 613.7,
                                    "StdDev": 40.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 3,
                                        "More800Less1200": 10,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 65,
                                    "MaxBytes": 614,
                                    "Percent50": 58,
                                    "Percent75": 72,
                                    "Percent95": 72,
                                    "Percent99": 72,
                                    "StdDev": 7.0,
                                    "AllBytes": 1195
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 10,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 10
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
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 8,
                                    "RPS": 0.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 380.2,
                                    "MaxMs": 893.7,
                                    "Percent50": 359.8,
                                    "Percent75": 400.9,
                                    "Percent95": 400.9,
                                    "Percent99": 400.9,
                                    "StdDev": 20.5,
                                    "LatencyCount": {
                                        "LessOrEq800": 7,
                                        "More800Less1200": 1,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 375,
                                    "MaxBytes": 954,
                                    "Percent50": 351,
                                    "Percent75": 399,
                                    "Percent95": 399,
                                    "Percent99": 399,
                                    "StdDev": 24.0,
                                    "AllBytes": 4443
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 4,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 4
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 7,
                                    "RPS": 0.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 504.8,
                                    "MaxMs": 1014.2,
                                    "Percent50": 504.7,
                                    "Percent75": 505.2,
                                    "Percent95": 505.2,
                                    "Percent99": 505.2,
                                    "StdDev": 0.2,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 7,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 7,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 7
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 55,
                        "More800Less1200": 51,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "StatusCodes": [
                        {
                            "Count@": 41,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 41
                        },
                        {
                            "Count@": 34,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 34
                        },
                        {
                            "Count@": 1,
                            "StatusCode": 400,
                            "IsError": true,
                            "Message": "Bad Request",
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
                                    "Value": "2031614404",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "522799890",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "2053055039",
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
                                    "Value": "1977413755",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "139201604",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "767070351",
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
            "Duration": "00:00:17",
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario_3",
                    "RequestCount": 174,
                    "OkCount": 91,
                    "FailCount": 83,
                    "AllBytes": 51844,
                    "StepStats": [
                        {
                            "StepName": "pull_html_3",
                            "Ok": {
                                "Request": {
                                    "Count": 49,
                                    "RPS": 2.7
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 224.7,
                                    "MaxMs": 1012.5,
                                    "Percent50": 182.8,
                                    "Percent75": 285.8,
                                    "Percent95": 323.8,
                                    "Percent99": 323.8,
                                    "StdDev": 72.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 44,
                                        "More800Less1200": 5,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 246,
                                    "MaxBytes": 993,
                                    "Percent50": 203,
                                    "Percent75": 305,
                                    "Percent95": 348,
                                    "Percent99": 348,
                                    "StdDev": 76.0,
                                    "AllBytes": 21359
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 29,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 29
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 53,
                                    "RPS": 2.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 631.3,
                                    "MaxMs": 1017.4,
                                    "Percent50": 656.7,
                                    "Percent75": 657.7,
                                    "Percent95": 658.7,
                                    "Percent99": 658.7,
                                    "StdDev": 46.3,
                                    "LatencyCount": {
                                        "LessOrEq800": 7,
                                        "More800Less1200": 46,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 144,
                                    "MaxBytes": 818,
                                    "Percent50": 142,
                                    "Percent75": 146,
                                    "Percent95": 146,
                                    "Percent99": 146,
                                    "StdDev": 2.1,
                                    "AllBytes": 4757
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 45,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 45
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
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_4",
                            "Ok": {
                                "Request": {
                                    "Count": 28,
                                    "RPS": 1.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 305.8,
                                    "MaxMs": 999.4,
                                    "Percent50": 291.2,
                                    "Percent75": 331.3,
                                    "Percent95": 345.2,
                                    "Percent99": 345.2,
                                    "StdDev": 37.4,
                                    "LatencyCount": {
                                        "LessOrEq800": 19,
                                        "More800Less1200": 9,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 283,
                                    "MaxBytes": 988,
                                    "Percent50": 244,
                                    "Percent75": 314,
                                    "Percent95": 356,
                                    "Percent99": 356,
                                    "StdDev": 59.7,
                                    "AllBytes": 14550
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 21,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 21
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 20,
                                    "RPS": 1.1
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 290.2,
                                    "MaxMs": 1015.8,
                                    "Percent50": 270.1,
                                    "Percent75": 314.7,
                                    "Percent95": 315.5,
                                    "Percent99": 315.5,
                                    "StdDev": 25.3,
                                    "LatencyCount": {
                                        "LessOrEq800": 5,
                                        "More800Less1200": 15,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 27,
                                    "MaxBytes": 614,
                                    "Percent50": 20,
                                    "Percent75": 34,
                                    "Percent95": 34,
                                    "Percent99": 34,
                                    "StdDev": 7.0,
                                    "AllBytes": 1504
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 15,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 15
                                    },
                                    {
                                        "Count@": 1,
                                        "StatusCode": 500,
                                        "IsError": true,
                                        "Message": "Internal Server Error",
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
                                ]
                            }
                        },
                        {
                            "StepName": "pull_html_5",
                            "Ok": {
                                "Request": {
                                    "Count": 14,
                                    "RPS": 0.8
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 215.9,
                                    "MaxMs": 996.1,
                                    "Percent50": 202.0,
                                    "Percent75": 220.7,
                                    "Percent95": 234.8,
                                    "Percent99": 234.8,
                                    "StdDev": 16.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 12,
                                        "More800Less1200": 2,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 225,
                                    "MaxBytes": 991,
                                    "Percent50": 201,
                                    "Percent75": 250,
                                    "Percent95": 257,
                                    "Percent99": 257,
                                    "StdDev": 31.2,
                                    "AllBytes": 8875
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 7,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 7
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 10,
                                    "RPS": 0.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 232.1,
                                    "MaxMs": 1014.2,
                                    "Percent50": 221.1,
                                    "Percent75": 243.2,
                                    "Percent95": 243.2,
                                    "Percent99": 243.2,
                                    "StdDev": 11.1,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 9,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 27,
                                    "MaxBytes": 799,
                                    "Percent50": 27,
                                    "Percent75": 27,
                                    "Percent95": 27,
                                    "Percent99": 27,
                                    "StdDev": 0.0,
                                    "AllBytes": 799
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 9,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 9
                                    },
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
                        "More800Less1200": 86,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "StatusCodes": [
                        {
                            "Count@": 69,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 69
                        },
                        {
                            "Count@": 57,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 57
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
                            "StatusCode": 500,
                            "IsError": true,
                            "Message": "Internal Server Error",
                            "Count": 2
                        },
                        {
                            "Count@": 3,
                            "StatusCode": 502,
                            "IsError": true,
                            "Message": "Bad Gateway",
                            "Count": 3
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
                    "Duration": "00:00:17"
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
                                    "Value": "74127588",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1518660903",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "293432812",
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
                                    "Value": "812571500",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "832208558",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "672504236",
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
                    "RequestCount": 101,
                    "OkCount": 70,
                    "FailCount": 31,
                    "AllBytes": 0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 70,
                                    "RPS": 3.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 405.5,
                                    "MaxMs": 1016.6,
                                    "Percent50": 380.7,
                                    "Percent75": 437.4,
                                    "Percent95": 538.1,
                                    "Percent99": 545.9,
                                    "StdDev": 98.6,
                                    "LatencyCount": {
                                        "LessOrEq800": 57,
                                        "More800Less1200": 14,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 31,
                                    "RPS": 1.5
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 706.3,
                                    "MaxMs": 1016.6,
                                    "Percent50": 706.4,
                                    "Percent75": 707.2,
                                    "Percent95": 707.7,
                                    "Percent99": 707.7,
                                    "StdDev": 1.0,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 31,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 31,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 31
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 57,
                        "More800Less1200": 45,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "StatusCodes": [
                        {
                            "Count@": 31,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 31
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:20"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 151,
                    "OkCount": 98,
                    "FailCount": 53,
                    "AllBytes": 51213,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 98,
                                    "RPS": 4.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 470.9,
                                    "MaxMs": 983.0,
                                    "Percent50": 434.4,
                                    "Percent75": 553.6,
                                    "Percent95": 677.2,
                                    "Percent99": 691.2,
                                    "StdDev": 147.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 78,
                                        "More800Less1200": 20,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 418,
                                    "MaxBytes": 990,
                                    "Percent50": 388,
                                    "Percent75": 518,
                                    "Percent95": 642,
                                    "Percent99": 651,
                                    "StdDev": 158.9,
                                    "AllBytes": 50428
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 99,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 99
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 53,
                                    "RPS": 2.6
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 834.2,
                                    "MaxMs": 1015.0,
                                    "Percent50": 840.7,
                                    "Percent75": 841.6,
                                    "Percent95": 843.1,
                                    "Percent99": 843.1,
                                    "StdDev": 18.6,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 52,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 43,
                                    "MaxBytes": 785,
                                    "Percent50": 43,
                                    "Percent75": 43,
                                    "Percent95": 43,
                                    "Percent99": 43,
                                    "StdDev": 0.0,
                                    "AllBytes": 785
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 52,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 52
                                    },
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
                        "LessOrEq800": 79,
                        "More800Less1200": 72,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "StatusCodes": [
                        {
                            "Count@": 52,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 52
                        },
                        {
                            "Count@": 99,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 99
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
                                    "Value": "1226725719",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "603295306",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1321360285",
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
                                    "Value": "1448908700",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1507536807",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "733038756",
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
                    "RequestCount": 151,
                    "OkCount": 107,
                    "FailCount": 44,
                    "AllBytes": 0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 107,
                                    "RPS": 4.3
                                },
                                "Latency": {
                                    "MinMs": 25.6,
                                    "MeanMs": 517.7,
                                    "MaxMs": 1016.6,
                                    "Percent50": 500.7,
                                    "Percent75": 572.9,
                                    "Percent95": 764.1,
                                    "Percent99": 771.4,
                                    "StdDev": 180.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 86,
                                        "More800Less1200": 21,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 44,
                                    "RPS": 1.8
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 911.6,
                                    "MaxMs": 1016.6,
                                    "Percent50": 911.7,
                                    "Percent75": 912.7,
                                    "Percent95": 913.9,
                                    "Percent99": 913.9,
                                    "StdDev": 1.7,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 44,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 44,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 44
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 86,
                        "More800Less1200": 65,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "StatusCodes": [
                        {
                            "Count@": 44,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 44
                        }
                    ],
                    "CurrentOperation": 3,
                    "Duration": "00:00:25"
                },
                {
                    "ScenarioName": "scenario_2",
                    "RequestCount": 176,
                    "OkCount": 118,
                    "FailCount": 58,
                    "AllBytes": 61928,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 118,
                                    "RPS": 4.7
                                },
                                "Latency": {
                                    "MinMs": 10.2,
                                    "MeanMs": 560.8,
                                    "MaxMs": 984.7,
                                    "Percent50": 488.3,
                                    "Percent75": 690.5,
                                    "Percent95": 885.1,
                                    "Percent99": 899.2,
                                    "StdDev": 235.6,
                                    "LatencyCount": {
                                        "LessOrEq800": 91,
                                        "More800Less1200": 27,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 32,
                                    "MeanBytes": 504,
                                    "MaxBytes": 998,
                                    "Percent50": 439,
                                    "Percent75": 671,
                                    "Percent95": 829,
                                    "Percent99": 837,
                                    "StdDev": 241.2,
                                    "AllBytes": 61143
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 118,
                                        "StatusCode": 200,
                                        "IsError": false,
                                        "Message": "",
                                        "Count": 118
                                    }
                                ]
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 58,
                                    "RPS": 2.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 945.9,
                                    "MaxMs": 1015.0,
                                    "Percent50": 952.4,
                                    "Percent75": 953.5,
                                    "Percent95": 955.0,
                                    "Percent99": 955.0,
                                    "StdDev": 18.8,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 57,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 43,
                                    "MaxBytes": 785,
                                    "Percent50": 43,
                                    "Percent75": 43,
                                    "Percent95": 43,
                                    "Percent99": 43,
                                    "StdDev": 0.0,
                                    "AllBytes": 785
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 57,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 57
                                    },
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
                        "LessOrEq800": 92,
                        "More800Less1200": 84,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "StatusCodes": [
                        {
                            "Count@": 57,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 57
                        },
                        {
                            "Count@": 118,
                            "StatusCode": 200,
                            "IsError": false,
                            "Message": "",
                            "Count": 118
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
                                    "Value": "876403835",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1417225218",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "931000930",
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
                                    "Value": "389755462",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1382697089",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "256218052",
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
                    "RequestCount": 206,
                    "OkCount": 136,
                    "FailCount": 70,
                    "AllBytes": 68286,
                    "StepStats": [
                        {
                            "StepName": "pull_html_2",
                            "Ok": {
                                "Request": {
                                    "Count": 136,
                                    "RPS": 4.5
                                },
                                "Latency": {
                                    "MinMs": 10.2,
                                    "MeanMs": 565.9,
                                    "MaxMs": 984.7,
                                    "Percent50": 509.0,
                                    "Percent75": 706.1,
                                    "Percent95": 887.6,
                                    "Percent99": 907.6,
                                    "StdDev": 232.1,
                                    "LatencyCount": {
                                        "LessOrEq800": 105,
                                        "More800Less1200": 31,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 32,
                                    "MeanBytes": 493,
                                    "MaxBytes": 998,
                                    "Percent50": 447,
                                    "Percent75": 647,
                                    "Percent95": 829,
                                    "Percent99": 845,
                                    "StdDev": 233.9,
                                    "AllBytes": 67501
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
                                    "Count": 70,
                                    "RPS": 2.3
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 947.0,
                                    "MaxMs": 1019.9,
                                    "Percent50": 953.5,
                                    "Percent75": 954.8,
                                    "Percent95": 957.3,
                                    "Percent99": 957.3,
                                    "StdDev": 19.9,
                                    "LatencyCount": {
                                        "LessOrEq800": 1,
                                        "More800Less1200": 69,
                                        "MoreOrEq1200": 0
                                    }
                                },
                                "DataTransfer": {
                                    "MinBytes": 0,
                                    "MeanBytes": 43,
                                    "MaxBytes": 785,
                                    "Percent50": 43,
                                    "Percent75": 43,
                                    "Percent95": 43,
                                    "Percent99": 43,
                                    "StdDev": 0.0,
                                    "AllBytes": 785
                                },
                                "StatusCodes": [
                                    {
                                        "Count@": 69,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 69
                                    },
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
                        "LessOrEq800": 106,
                        "More800Less1200": 100,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "StatusCodes": [
                        {
                            "Count@": 69,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 69
                        },
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
                                    "Value": "1750584273",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "2029284104",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1432116165",
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
                                    "Value": "1197284196",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1080681202",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "969060989",
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
                    "RequestCount": 209,
                    "OkCount": 151,
                    "FailCount": 58,
                    "AllBytes": 0,
                    "StepStats": [
                        {
                            "StepName": "pull_html_1",
                            "Ok": {
                                "Request": {
                                    "Count": 151,
                                    "RPS": 5.0
                                },
                                "Latency": {
                                    "MinMs": 25.6,
                                    "MeanMs": 532.6,
                                    "MaxMs": 1016.6,
                                    "Percent50": 532.5,
                                    "Percent75": 673.6,
                                    "Percent95": 830.6,
                                    "Percent99": 837.9,
                                    "StdDev": 221.3,
                                    "LatencyCount": {
                                        "LessOrEq800": 117,
                                        "More800Less1200": 34,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": []
                            },
                            "Fail": {
                                "Request": {
                                    "Count": 58,
                                    "RPS": 1.9
                                },
                                "Latency": {
                                    "MinMs": 0.0,
                                    "MeanMs": 960.5,
                                    "MaxMs": 1019.1,
                                    "Percent50": 960.1,
                                    "Percent75": 962.5,
                                    "Percent95": 963.9,
                                    "Percent99": 963.9,
                                    "StdDev": 2.6,
                                    "LatencyCount": {
                                        "LessOrEq800": 0,
                                        "More800Less1200": 58,
                                        "MoreOrEq1200": 0
                                    }
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
                                "StatusCodes": [
                                    {
                                        "Count@": 58,
                                        "StatusCode": -100,
                                        "IsError": true,
                                        "Message": "step timeout",
                                        "Count": 58
                                    }
                                ]
                            }
                        }
                    ],
                    "LatencyCount": {
                        "LessOrEq800": 117,
                        "More800Less1200": 92,
                        "MoreOrEq1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "StatusCodes": [
                        {
                            "Count@": 58,
                            "StatusCode": -100,
                            "IsError": true,
                            "Message": "step timeout",
                            "Count": 58
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
                                    "Value": "1175682952",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property2",
                                    "Value": "1971978941",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Key": "Property3",
                                    "Value": "1648410948",
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
                                    "Value": "123373102",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 2",
                                    "Value": "1563601686",
                                    "RowState": 4
                                },
                                {
                                    "OriginalRow": null,
                                    "Property": "Property 3",
                                    "Value": "913982101",
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
