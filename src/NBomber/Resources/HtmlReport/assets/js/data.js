const viewModel = {
    nBomberInfo: {
        "NBomberVersion": "1.1.0"
    }, testInfo: {
        "TestSuite": "nbomber_default_test_suite_name",
        "TestName": "nbomber_default_test_name"
    }, statsData: {
        "RequestCount": 246,
        "OkCount": 152,
        "FailCount": 94,
        "AllDataMB": 0.0,
        "ScenarioStats": [
            {
                "ScenarioName": "scenario 1",
                "RequestCount": 123,
                "OkCount": 76,
                "FailCount": 47,
                "AllDataMB": 0.0,
                "StepStats": [
                    {
                        "StepName": "ok step",
                        "RequestCount": 49,
                        "OkCount": 49,
                        "FailCount": 0,
                        "Min": 127,
                        "Mean": 511,
                        "Max": 978,
                        "RPS": 0,
                        "Percent50": 407,
                        "Percent75": 758,
                        "Percent95": 964,
                        "Percent99": 978,
                        "StdDev": 276,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 0.0,
                        "MaxDataKb": 0.0,
                        "AllDataMB": 0.0,
                        "ErrorStats": []
                    },
                    {
                        "StepName": "fail step 1",
                        "RequestCount": 47,
                        "OkCount": 27,
                        "FailCount": 20,
                        "Min": 574,
                        "Mean": 1207,
                        "Max": 1952,
                        "RPS": 0,
                        "Percent50": 1191,
                        "Percent75": 1566,
                        "Percent95": 1936,
                        "Percent99": 1952,
                        "StdDev": 435,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 0.0,
                        "MaxDataKb": 0.0,
                        "AllDataMB": 0.0,
                        "ErrorStats": [
                            {
                                "ErrorCode": 500,
                                "Exception": {
                                    "TargetSite": null,
                                    "StackTrace": null,
                                    "Message": "Internal Server Error",
                                    "Data": {},
                                    "InnerException": null,
                                    "HelpLink": null,
                                    "Source": null,
                                    "HResult": -2146233088
                                },
                                "Count": 20
                            }
                        ]
                    },
                    {
                        "StepName": "fail step 2",
                        "RequestCount": 27,
                        "OkCount": 0,
                        "FailCount": 27,
                        "Min": 0,
                        "Mean": 0,
                        "Max": 0,
                        "RPS": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 0.0,
                        "MaxDataKb": 0.0,
                        "AllDataMB": 0.0,
                        "ErrorStats": [
                            {
                                "ErrorCode": 400,
                                "Exception": {
                                    "TargetSite": null,
                                    "StackTrace": null,
                                    "Message": "Bad Request",
                                    "Data": {},
                                    "InnerException": null,
                                    "HelpLink": null,
                                    "Source": null,
                                    "HResult": -2146233088
                                },
                                "Count": 27
                            }
                        ]
                    }
                ],
                "LatencyCount": {
                    "Less800": 46,
                    "More800Less1200": 17,
                    "More1200": 13
                },
                "LoadSimulationStats": {
                    "SimulationName": "keep_constant",
                    "Value": 2
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 500,
                        "Exception": {
                            "TargetSite": null,
                            "StackTrace": null,
                            "Message": "Internal Server Error",
                            "Data": {},
                            "InnerException": null,
                            "HelpLink": null,
                            "Source": null,
                            "HResult": -2146233088
                        },
                        "Count": 20
                    },
                    {
                        "ErrorCode": 400,
                        "Exception": {
                            "TargetSite": null,
                            "StackTrace": null,
                            "Message": "Bad Request",
                            "Data": {},
                            "InnerException": null,
                            "HelpLink": null,
                            "Source": null,
                            "HResult": -2146233088
                        },
                        "Count": 27
                    }
                ],
                "Duration": "00:01:00"
            },
            {
                "ScenarioName": "scenario 2",
                "RequestCount": 123,
                "OkCount": 76,
                "FailCount": 47,
                "AllDataMB": 0.0,
                "StepStats": [
                    {
                        "StepName": "ok step",
                        "RequestCount": 49,
                        "OkCount": 49,
                        "FailCount": 0,
                        "Min": 110,
                        "Mean": 596,
                        "Max": 993,
                        "RPS": 0,
                        "Percent50": 657,
                        "Percent75": 811,
                        "Percent95": 969,
                        "Percent99": 993,
                        "StdDev": 257,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 0.0,
                        "MaxDataKb": 0.0,
                        "AllDataMB": 0.0,
                        "ErrorStats": []
                    },
                    {
                        "StepName": "fail step 1",
                        "RequestCount": 47,
                        "OkCount": 27,
                        "FailCount": 20,
                        "Min": 559,
                        "Mean": 1225,
                        "Max": 1889,
                        "RPS": 0,
                        "Percent50": 1209,
                        "Percent75": 1601,
                        "Percent95": 1812,
                        "Percent99": 1889,
                        "StdDev": 433,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 0.0,
                        "MaxDataKb": 0.0,
                        "AllDataMB": 0.0,
                        "ErrorStats": [
                            {
                                "ErrorCode": 500,
                                "Exception": {
                                    "TargetSite": null,
                                    "StackTrace": null,
                                    "Message": "Internal Server Error",
                                    "Data": {},
                                    "InnerException": null,
                                    "HelpLink": null,
                                    "Source": null,
                                    "HResult": -2146233088
                                },
                                "Count": 20
                            }
                        ]
                    },
                    {
                        "StepName": "fail step 2",
                        "RequestCount": 27,
                        "OkCount": 0,
                        "FailCount": 27,
                        "Min": 0,
                        "Mean": 0,
                        "Max": 0,
                        "RPS": 0,
                        "Percent50": 0,
                        "Percent75": 0,
                        "Percent95": 0,
                        "Percent99": 0,
                        "StdDev": 0,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 0.0,
                        "MaxDataKb": 0.0,
                        "AllDataMB": 0.0,
                        "ErrorStats": [
                            {
                                "ErrorCode": 400,
                                "Exception": {
                                    "TargetSite": null,
                                    "StackTrace": null,
                                    "Message": "Bad Request",
                                    "Data": {},
                                    "InnerException": null,
                                    "HelpLink": null,
                                    "Source": null,
                                    "HResult": -2146233088
                                },
                                "Count": 27
                            }
                        ]
                    }
                ],
                "LatencyCount": {
                    "Less800": 40,
                    "More800Less1200": 22,
                    "More1200": 14
                },
                "LoadSimulationStats": {
                    "SimulationName": "keep_constant",
                    "Value": 2
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 500,
                        "Exception": {
                            "TargetSite": null,
                            "StackTrace": null,
                            "Message": "Internal Server Error",
                            "Data": {},
                            "InnerException": null,
                            "HelpLink": null,
                            "Source": null,
                            "HResult": -2146233088
                        },
                        "Count": 20
                    },
                    {
                        "ErrorCode": 400,
                        "Exception": {
                            "TargetSite": null,
                            "StackTrace": null,
                            "Message": "Bad Request",
                            "Data": {},
                            "InnerException": null,
                            "HelpLink": null,
                            "Source": null,
                            "HResult": -2146233088
                        },
                        "Count": 27
                    }
                ],
                "Duration": "00:01:00"
            }
        ],
        "PluginStats": [
            {
                "TableName": "NBomber.Plugins.Network.PingPlugin",
                "Columns": [
                    "Host",
                    "Status",
                    "Address",
                    "Round Trip Time",
                    "Time to Live",
                    "Don't Fragment",
                    "Buffer Size"
                ],
                "Rows": [
                    [
                        "google.com",
                        "Success",
                        "172.217.16.46",
                        "30 ms",
                        "128",
                        "False",
                        "32 bytes"
                    ]
                ]
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
                "Version": {
                    "Major": 10,
                    "Minor": 0,
                    "Build": 18363,
                    "Revision": 0,
                    "MajorRevision": 0,
                    "MinorRevision": 0
                },
                "VersionString": "Microsoft Windows NT 10.0.18363.0"
            },
            "DotNetVersion": ".NETCoreApp,Version=v1.0",
            "Processor": "Intel64 Family 6 Model 94 Stepping 3, GenuineIntel",
            "CoresCount": 8,
            "NBomberVersion": "1.1.0"
        }
    }, timeLineStatsData: {
        "TimeStamps": [
            "00:00:05",
            "00:00:10",
            "00:00:15",
            "00:00:20",
            "00:00:25",
            "00:00:30",
            "00:00:35",
            "00:00:39",
            "00:00:45",
            "00:00:50",
            "00:00:55",
            "00:01:00"
        ],
        "ScenarioStats": [
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 9,
                    "OkCount": 5,
                    "FailCount": 4,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 5,
                            "OkCount": 5,
                            "FailCount": 0,
                            "Min": 205,
                            "Mean": 422,
                            "Max": 668,
                            "RPS": 1,
                            "Percent50": 365,
                            "Percent75": 593,
                            "Percent95": 668,
                            "Percent99": 668,
                            "StdDev": 179,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 4,
                            "OkCount": 0,
                            "FailCount": 4,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 4
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 5,
                        "More800Less1200": 0,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 4
                        }
                    ],
                    "Duration": "00:00:05.0128846"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 8,
                    "OkCount": 4,
                    "FailCount": 4,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 4,
                            "OkCount": 4,
                            "FailCount": 0,
                            "Min": 343,
                            "Mean": 594,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 384,
                            "Percent75": 657,
                            "Percent95": 993,
                            "Percent99": 993,
                            "StdDev": 260,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 4,
                            "OkCount": 0,
                            "FailCount": 4,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 4
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 3,
                        "More800Less1200": 1,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 4
                        }
                    ],
                    "Duration": "00:00:05.0128846"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 20,
                    "OkCount": 11,
                    "FailCount": 9,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 11,
                            "OkCount": 11,
                            "FailCount": 0,
                            "Min": 205,
                            "Mean": 606,
                            "Max": 978,
                            "RPS": 1,
                            "Percent50": 668,
                            "Percent75": 746,
                            "Percent95": 974,
                            "Percent99": 978,
                            "StdDev": 259,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 9,
                            "OkCount": 0,
                            "FailCount": 9,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 9
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 9,
                        "More800Less1200": 2,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 9
                        }
                    ],
                    "Duration": "00:00:10.0120337"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 19,
                    "OkCount": 10,
                    "FailCount": 9,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 10,
                            "OkCount": 10,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 693,
                            "Max": 993,
                            "RPS": 1,
                            "Percent50": 811,
                            "Percent75": 904,
                            "Percent95": 993,
                            "Percent99": 993,
                            "StdDev": 272,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 9,
                            "OkCount": 0,
                            "FailCount": 9,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 9
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 4,
                        "More800Less1200": 6,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 9
                        }
                    ],
                    "Duration": "00:00:10.0120337"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 32,
                    "OkCount": 16,
                    "FailCount": 16,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 16,
                            "OkCount": 16,
                            "FailCount": 0,
                            "Min": 138,
                            "Mean": 517,
                            "Max": 978,
                            "RPS": 1,
                            "Percent50": 407,
                            "Percent75": 745,
                            "Percent95": 974,
                            "Percent99": 978,
                            "StdDev": 265,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 16,
                            "OkCount": 0,
                            "FailCount": 16,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 16
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 14,
                        "More800Less1200": 2,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 16
                        }
                    ],
                    "Duration": "00:00:15.0042152"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 31,
                    "OkCount": 16,
                    "FailCount": 15,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 16,
                            "OkCount": 16,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 647,
                            "Max": 993,
                            "RPS": 1,
                            "Percent50": 722,
                            "Percent75": 835,
                            "Percent95": 950,
                            "Percent99": 993,
                            "StdDev": 264,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 15,
                            "OkCount": 0,
                            "FailCount": 15,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 15
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 8,
                        "More800Less1200": 8,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 15
                        }
                    ],
                    "Duration": "00:00:15.0042152"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 42,
                    "OkCount": 22,
                    "FailCount": 20,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 21,
                            "OkCount": 21,
                            "FailCount": 0,
                            "Min": 138,
                            "Mean": 538,
                            "Max": 978,
                            "RPS": 1,
                            "Percent50": 510,
                            "Percent75": 746,
                            "Percent95": 974,
                            "Percent99": 978,
                            "StdDev": 277,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 21,
                            "OkCount": 1,
                            "FailCount": 20,
                            "Min": 1246,
                            "Mean": 1246,
                            "Max": 1246,
                            "RPS": 0,
                            "Percent50": 1246,
                            "Percent75": 1246,
                            "Percent95": 1246,
                            "Percent99": 1246,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 17,
                        "More800Less1200": 4,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        }
                    ],
                    "Duration": "00:00:20.0127556"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 41,
                    "OkCount": 21,
                    "FailCount": 20,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 21,
                            "OkCount": 21,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 639,
                            "Max": 993,
                            "RPS": 1,
                            "Percent50": 722,
                            "Percent75": 835,
                            "Percent95": 950,
                            "Percent99": 993,
                            "StdDev": 252,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 20,
                            "OkCount": 0,
                            "FailCount": 20,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 0,
                            "OkCount": 0,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 12,
                        "More800Less1200": 9,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        }
                    ],
                    "Duration": "00:00:20.0127556"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 56,
                    "OkCount": 32,
                    "FailCount": 24,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 26,
                            "OkCount": 26,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 501,
                            "Max": 978,
                            "RPS": 1,
                            "Percent50": 390,
                            "Percent75": 746,
                            "Percent95": 974,
                            "Percent99": 978,
                            "StdDev": 283,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 26,
                            "OkCount": 6,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 855,
                            "Max": 1246,
                            "RPS": 0,
                            "Percent50": 750,
                            "Percent75": 1127,
                            "Percent95": 1246,
                            "Percent99": 1246,
                            "StdDev": 246,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 4,
                            "OkCount": 0,
                            "FailCount": 4,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 4
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 25,
                        "More800Less1200": 6,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 4
                        }
                    ],
                    "Duration": "00:00:25.0137593"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 51,
                    "OkCount": 28,
                    "FailCount": 23,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 24,
                            "OkCount": 24,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 650,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 722,
                            "Percent75": 873,
                            "Percent95": 987,
                            "Percent99": 993,
                            "StdDev": 259,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 24,
                            "OkCount": 4,
                            "FailCount": 20,
                            "Min": 602,
                            "Mean": 870,
                            "Max": 1437,
                            "RPS": 0,
                            "Percent50": 656,
                            "Percent75": 785,
                            "Percent95": 1437,
                            "Percent99": 1437,
                            "StdDev": 334,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 3,
                            "OkCount": 0,
                            "FailCount": 3,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 3
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 16,
                        "More800Less1200": 11,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 3
                        }
                    ],
                    "Duration": "00:00:25.0137593"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 63,
                    "OkCount": 36,
                    "FailCount": 27,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 28,
                            "OkCount": 28,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 510,
                            "Max": 978,
                            "RPS": 0,
                            "Percent50": 390,
                            "Percent75": 746,
                            "Percent95": 974,
                            "Percent99": 978,
                            "StdDev": 288,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 28,
                            "OkCount": 8,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 1027,
                            "Max": 1936,
                            "RPS": 0,
                            "Percent50": 787,
                            "Percent75": 1154,
                            "Percent95": 1936,
                            "Percent99": 1936,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 7,
                            "OkCount": 0,
                            "FailCount": 7,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 7
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 26,
                        "More800Less1200": 8,
                        "More1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 7
                        }
                    ],
                    "Duration": "00:00:30.0132978"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 60,
                    "OkCount": 34,
                    "FailCount": 26,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 28,
                            "OkCount": 28,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 634,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 722,
                            "Percent75": 873,
                            "Percent95": 987,
                            "Percent99": 993,
                            "StdDev": 267,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 26,
                            "OkCount": 6,
                            "FailCount": 20,
                            "Min": 602,
                            "Mean": 991,
                            "Max": 1548,
                            "RPS": 0,
                            "Percent50": 785,
                            "Percent75": 1437,
                            "Percent95": 1548,
                            "Percent99": 1548,
                            "StdDev": 370,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 6,
                            "OkCount": 0,
                            "FailCount": 6,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 6
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 19,
                        "More800Less1200": 13,
                        "More1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 6
                        }
                    ],
                    "Duration": "00:00:30.0132978"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 73,
                    "OkCount": 43,
                    "FailCount": 30,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 32,
                            "OkCount": 32,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 501,
                            "Max": 978,
                            "RPS": 0,
                            "Percent50": 383,
                            "Percent75": 746,
                            "Percent95": 964,
                            "Percent99": 978,
                            "StdDev": 279,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 31,
                            "OkCount": 11,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 1061,
                            "Max": 1936,
                            "RPS": 0,
                            "Percent50": 1127,
                            "Percent75": 1246,
                            "Percent95": 1466,
                            "Percent99": 1936,
                            "StdDev": 409,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 10,
                            "OkCount": 0,
                            "FailCount": 10,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 10
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 31,
                        "More800Less1200": 8,
                        "More1200": 4
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 10
                        }
                    ],
                    "Duration": "00:00:35.0118387"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 69,
                    "OkCount": 40,
                    "FailCount": 29,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 31,
                            "OkCount": 31,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 637,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 681,
                            "Percent75": 835,
                            "Percent95": 950,
                            "Percent99": 993,
                            "StdDev": 254,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 29,
                            "OkCount": 9,
                            "FailCount": 20,
                            "Min": 602,
                            "Mean": 1053,
                            "Max": 1797,
                            "RPS": 0,
                            "Percent50": 919,
                            "Percent75": 1437,
                            "Percent95": 1797,
                            "Percent99": 1797,
                            "StdDev": 404,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 9,
                            "OkCount": 0,
                            "FailCount": 9,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 9
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 22,
                        "More800Less1200": 15,
                        "More1200": 3
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 9
                        }
                    ],
                    "Duration": "00:00:35.0118387"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 81,
                    "OkCount": 48,
                    "FailCount": 33,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 34,
                            "OkCount": 34,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 514,
                            "Max": 978,
                            "RPS": 0,
                            "Percent50": 390,
                            "Percent75": 758,
                            "Percent95": 964,
                            "Percent99": 978,
                            "StdDev": 277,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 34,
                            "OkCount": 14,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 1117,
                            "Max": 1936,
                            "RPS": 0,
                            "Percent50": 1127,
                            "Percent75": 1466,
                            "Percent95": 1597,
                            "Percent99": 1936,
                            "StdDev": 414,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 13,
                            "OkCount": 0,
                            "FailCount": 13,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 13
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 32,
                        "More800Less1200": 10,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 13
                        }
                    ],
                    "Duration": "00:00:39.9994432"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 79,
                    "OkCount": 47,
                    "FailCount": 32,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 34,
                            "OkCount": 34,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 629,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 673,
                            "Percent75": 873,
                            "Percent95": 969,
                            "Percent99": 993,
                            "StdDev": 261,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 33,
                            "OkCount": 13,
                            "FailCount": 20,
                            "Min": 602,
                            "Mean": 1112,
                            "Max": 1797,
                            "RPS": 0,
                            "Percent50": 945,
                            "Percent75": 1437,
                            "Percent95": 1767,
                            "Percent99": 1797,
                            "StdDev": 389,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 12,
                            "OkCount": 0,
                            "FailCount": 12,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 12
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 24,
                        "More800Less1200": 18,
                        "More1200": 5
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 12
                        }
                    ],
                    "Duration": "00:00:39.9994432"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 92,
                    "OkCount": 55,
                    "FailCount": 37,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 38,
                            "OkCount": 38,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 500,
                            "Max": 978,
                            "RPS": 0,
                            "Percent50": 390,
                            "Percent75": 746,
                            "Percent95": 964,
                            "Percent99": 978,
                            "StdDev": 272,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 37,
                            "OkCount": 17,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 1127,
                            "Max": 1952,
                            "RPS": 0,
                            "Percent50": 1127,
                            "Percent75": 1466,
                            "Percent95": 1936,
                            "Percent99": 1952,
                            "StdDev": 442,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 17,
                            "OkCount": 0,
                            "FailCount": 17,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 17
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 37,
                        "More800Less1200": 11,
                        "More1200": 7
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 17
                        }
                    ],
                    "Duration": "00:00:45.0110076"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 90,
                    "OkCount": 54,
                    "FailCount": 36,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 37,
                            "OkCount": 37,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 628,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 673,
                            "Percent75": 857,
                            "Percent95": 969,
                            "Percent99": 993,
                            "StdDev": 255,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 37,
                            "OkCount": 17,
                            "FailCount": 20,
                            "Min": 559,
                            "Mean": 1109,
                            "Max": 1889,
                            "RPS": 0,
                            "Percent50": 945,
                            "Percent75": 1437,
                            "Percent95": 1797,
                            "Percent99": 1889,
                            "StdDev": 425,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 16,
                            "OkCount": 0,
                            "FailCount": 16,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 16
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 28,
                        "More800Less1200": 19,
                        "More1200": 7
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 16
                        }
                    ],
                    "Duration": "00:00:45.0110076"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 104,
                    "OkCount": 63,
                    "FailCount": 41,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 42,
                            "OkCount": 42,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 488,
                            "Max": 978,
                            "RPS": 0,
                            "Percent50": 383,
                            "Percent75": 746,
                            "Percent95": 964,
                            "Percent99": 978,
                            "StdDev": 279,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 41,
                            "OkCount": 21,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 1153,
                            "Max": 1952,
                            "RPS": 0,
                            "Percent50": 1131,
                            "Percent75": 1466,
                            "Percent95": 1936,
                            "Percent99": 1952,
                            "StdDev": 451,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 21,
                            "OkCount": 0,
                            "FailCount": 21,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 21
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 41,
                        "More800Less1200": 13,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 21
                        }
                    ],
                    "Duration": "00:00:50.0112838"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 100,
                    "OkCount": 61,
                    "FailCount": 39,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 41,
                            "OkCount": 41,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 619,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 673,
                            "Percent75": 835,
                            "Percent95": 969,
                            "Percent99": 993,
                            "StdDev": 255,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 40,
                            "OkCount": 20,
                            "FailCount": 20,
                            "Min": 559,
                            "Mean": 1129,
                            "Max": 1889,
                            "RPS": 0,
                            "Percent50": 1015,
                            "Percent75": 1437,
                            "Percent95": 1797,
                            "Percent99": 1889,
                            "StdDev": 414,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 19,
                            "OkCount": 0,
                            "FailCount": 19,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 19
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 31,
                        "More800Less1200": 22,
                        "More1200": 8
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 19
                        }
                    ],
                    "Duration": "00:00:50.0112838"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 112,
                    "OkCount": 69,
                    "FailCount": 43,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 45,
                            "OkCount": 45,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 493,
                            "Max": 978,
                            "RPS": 0,
                            "Percent50": 390,
                            "Percent75": 746,
                            "Percent95": 964,
                            "Percent99": 978,
                            "StdDev": 277,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 44,
                            "OkCount": 24,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 1200,
                            "Max": 1952,
                            "RPS": 0,
                            "Percent50": 1154,
                            "Percent75": 1566,
                            "Percent95": 1936,
                            "Percent99": 1952,
                            "StdDev": 447,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 23,
                            "OkCount": 0,
                            "FailCount": 23,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 23
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 43,
                        "More800Less1200": 14,
                        "More1200": 12
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 23
                        }
                    ],
                    "Duration": "00:00:55.0121476"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 110,
                    "OkCount": 67,
                    "FailCount": 43,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 44,
                            "OkCount": 44,
                            "FailCount": 0,
                            "Min": 187,
                            "Mean": 615,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 657,
                            "Percent75": 828,
                            "Percent95": 969,
                            "Percent99": 993,
                            "StdDev": 249,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 43,
                            "OkCount": 23,
                            "FailCount": 20,
                            "Min": 559,
                            "Mean": 1167,
                            "Max": 1889,
                            "RPS": 0,
                            "Percent50": 1020,
                            "Percent75": 1548,
                            "Percent95": 1812,
                            "Percent99": 1889,
                            "StdDev": 438,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 23,
                            "OkCount": 0,
                            "FailCount": 23,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 23
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 35,
                        "More800Less1200": 22,
                        "More1200": 10
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 23
                        }
                    ],
                    "Duration": "00:00:55.0121476"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 122,
                    "OkCount": 75,
                    "FailCount": 47,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 48,
                            "OkCount": 48,
                            "FailCount": 0,
                            "Min": 127,
                            "Mean": 506,
                            "Max": 978,
                            "RPS": 0,
                            "Percent50": 390,
                            "Percent75": 758,
                            "Percent95": 964,
                            "Percent99": 978,
                            "StdDev": 277,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 47,
                            "OkCount": 27,
                            "FailCount": 20,
                            "Min": 574,
                            "Mean": 1207,
                            "Max": 1952,
                            "RPS": 0,
                            "Percent50": 1191,
                            "Percent75": 1566,
                            "Percent95": 1936,
                            "Percent99": 1952,
                            "StdDev": 435,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 27,
                            "OkCount": 0,
                            "FailCount": 27,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 27
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 45,
                        "More800Less1200": 17,
                        "More1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 27
                        }
                    ],
                    "Duration": "00:01:00"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 119,
                    "OkCount": 74,
                    "FailCount": 45,
                    "AllDataMB": 0.0,
                    "StepStats": [
                        {
                            "StepName": "ok step",
                            "RequestCount": 47,
                            "OkCount": 47,
                            "FailCount": 0,
                            "Min": 158,
                            "Mean": 610,
                            "Max": 993,
                            "RPS": 0,
                            "Percent50": 672,
                            "Percent75": 811,
                            "Percent95": 969,
                            "Percent99": 993,
                            "StdDev": 251,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "fail step 1",
                            "RequestCount": 47,
                            "OkCount": 27,
                            "FailCount": 20,
                            "Min": 559,
                            "Mean": 1225,
                            "Max": 1889,
                            "RPS": 0,
                            "Percent50": 1209,
                            "Percent75": 1601,
                            "Percent95": 1812,
                            "Percent99": 1889,
                            "StdDev": 433,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 500,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Internal Server Error",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 20
                                }
                            ]
                        },
                        {
                            "StepName": "fail step 2",
                            "RequestCount": 25,
                            "OkCount": 0,
                            "FailCount": 25,
                            "Min": 0,
                            "Mean": 0,
                            "Max": 0,
                            "RPS": 0,
                            "Percent50": 0,
                            "Percent75": 0,
                            "Percent95": 0,
                            "Percent99": 0,
                            "StdDev": 0,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 0.0,
                            "MaxDataKb": 0.0,
                            "AllDataMB": 0.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 400,
                                    "Exception": {
                                        "TargetSite": null,
                                        "StackTrace": null,
                                        "Message": "Bad Request",
                                        "Data": {},
                                        "InnerException": null,
                                        "HelpLink": null,
                                        "Source": null,
                                        "HResult": -2146233088
                                    },
                                    "Count": 25
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 38,
                        "More800Less1200": 22,
                        "More1200": 14
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "keep_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 500,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Internal Server Error",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 20
                        },
                        {
                            "ErrorCode": 400,
                            "Exception": {
                                "TargetSite": null,
                                "StackTrace": null,
                                "Message": "Bad Request",
                                "Data": {},
                                "InnerException": null,
                                "HelpLink": null,
                                "Source": null,
                                "HResult": -2146233088
                            },
                            "Count": 25
                        }
                    ],
                    "Duration": "00:01:00"
                }
            ]
        ]
    }
};
