const viewModel = { nBomberInfo: {
        "NBomberVersion": "1.1.0"
    }, testInfo: {
        "TestSuite": "nbomber_default_test_suite_name",
        "TestName": "nbomber_default_test_name"
    }, statsData: {
        "RequestCount": 3293,
        "OkCount": 2998,
        "FailCount": 295,
        "AllDataMB": 1473805.49,
        "ScenarioStats": [
            {
                "ScenarioName": "scenario 1",
                "RequestCount": 702,
                "OkCount": 702,
                "FailCount": 0,
                "AllDataMB": 347838.29,
                "StepStats": [
                    {
                        "StepName": "pull html 1",
                        "RequestCount": 702,
                        "OkCount": 702,
                        "FailCount": 0,
                        "Min": 0,
                        "Mean": 757,
                        "Max": 1512,
                        "RPS": 5,
                        "Percent50": 747,
                        "Percent75": 1119,
                        "Percent95": 1435,
                        "Percent99": 1489,
                        "StdDev": 419,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 946343.11,
                        "MaxDataKb": 2090797.29,
                        "AllDataMB": 347838.29,
                        "ErrorStats": []
                    }
                ],
                "LatencyCount": {
                    "Less800": 380,
                    "More800Less1200": 186,
                    "More1200": 136
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 10
                },
                "ErrorStats": [],
                "Duration": "00:01:30"
            },
            {
                "ScenarioName": "scenario 2",
                "RequestCount": 825,
                "OkCount": 783,
                "FailCount": 42,
                "AllDataMB": 384528.72,
                "StepStats": [
                    {
                        "StepName": "pull html 1",
                        "RequestCount": 414,
                        "OkCount": 414,
                        "FailCount": 0,
                        "Min": 0,
                        "Mean": 750,
                        "Max": 1501,
                        "RPS": 3,
                        "Percent50": 775,
                        "Percent75": 1109,
                        "Percent95": 1433,
                        "Percent99": 1494,
                        "StdDev": 426,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 931128.48,
                        "MaxDataKb": 2086343.09,
                        "AllDataMB": 207586.17,
                        "ErrorStats": []
                    },
                    {
                        "StepName": "pull html 2",
                        "RequestCount": 411,
                        "OkCount": 369,
                        "FailCount": 42,
                        "Min": 11,
                        "Mean": 786,
                        "Max": 1510,
                        "RPS": 2,
                        "Percent50": 815,
                        "Percent75": 1170,
                        "Percent95": 1436,
                        "Percent99": 1489,
                        "StdDev": 435,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 976636.93,
                        "MaxDataKb": 2059468.5,
                        "AllDataMB": 176942.55,
                        "ErrorStats": [
                            {
                                "ErrorCode": 0,
                                "Message": "System.Exception: unknown client's error",
                                "Count": 42
                            }
                        ]
                    }
                ],
                "LatencyCount": {
                    "Less800": 403,
                    "More800Less1200": 215,
                    "More1200": 164
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 5
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 0,
                        "Message": "System.Exception: unknown client's error",
                        "Count": 42
                    }
                ],
                "Duration": "00:01:30"
            },
            {
                "ScenarioName": "scenario 3",
                "RequestCount": 1766,
                "OkCount": 1513,
                "FailCount": 253,
                "AllDataMB": 741438.48,
                "StepStats": [
                    {
                        "StepName": "pull html 3",
                        "RequestCount": 688,
                        "OkCount": 586,
                        "FailCount": 102,
                        "Min": 9,
                        "Mean": 747,
                        "Max": 1509,
                        "RPS": 4,
                        "Percent50": 757,
                        "Percent75": 1107,
                        "Percent95": 1436,
                        "Percent99": 1494,
                        "StdDev": 432,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 935821.74,
                        "MaxDataKb": 2090156.63,
                        "AllDataMB": 297275.5,
                        "ErrorStats": [
                            {
                                "ErrorCode": 429,
                                "Message": "System.Exception: Too Many Requests",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 416,
                                "Message": "System.Exception: Requested Range Not Satisfiable",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 408,
                                "Message": "System.Exception: Request Timeout",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 415,
                                "Message": "System.Exception: Unsupported Media Type",
                                "Count": 6
                            },
                            {
                                "ErrorCode": 421,
                                "Message": "System.Exception: Misdirected Request",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 413,
                                "Message": "System.Exception: Payload Too Large",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 418,
                                "Message": "System.Exception: I'm a teapot",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 428,
                                "Message": "System.Exception: Precondition Required",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 426,
                                "Message": "System.Exception: Upgrade Required",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 403,
                                "Message": "System.Exception: Forbidden",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 409,
                                "Message": "System.Exception: Conflict",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 402,
                                "Message": "System.Exception: Payment Required",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 411,
                                "Message": "System.Exception: Length Required",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 451,
                                "Message": "System.Exception: Unavailable For Legal Reasons",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 404,
                                "Message": "System.Exception: Not Found",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 504,
                                "Message": "System.Exception: Gateway Timeout",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 412,
                                "Message": "System.Exception: Precondition Failed",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 431,
                                "Message": "System.Exception: Request Header Fields Too Large",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 501,
                                "Message": "System.Exception: Not Implemented",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 510,
                                "Message": "System.Exception: Not Extended",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 506,
                                "Message": "System.Exception: Variant Also Negotiates",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 424,
                                "Message": "System.Exception: Failed Dependency (WebDAV)",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 502,
                                "Message": "System.Exception: Bad Gateway",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 423,
                                "Message": "System.Exception: Locked (WebDAV)",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 511,
                                "Message": "System.Exception: Network Authentication Required",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 507,
                                "Message": "System.Exception: Insufficient Storage",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 407,
                                "Message": "System.Exception: Proxy Authentication Required",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 500,
                                "Message": "System.Exception: Internal Server Error",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 503,
                                "Message": "System.Exception: Service Unavailable",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 505,
                                "Message": "System.Exception: HTTP Version Not Supported",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 417,
                                "Message": "System.Exception: Expectation Failed",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 508,
                                "Message": "System.Exception: Loop Detected (WebDAV)",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 414,
                                "Message": "System.Exception: URI Too Long",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 410,
                                "Message": "System.Exception: Gone",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 401,
                                "Message": "System.Exception: Unauthorized",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 422,
                                "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                "Count": 4
                            }
                        ]
                    },
                    {
                        "StepName": "pull html 4",
                        "RequestCount": 579,
                        "OkCount": 507,
                        "FailCount": 72,
                        "Min": 0,
                        "Mean": 773,
                        "Max": 1501,
                        "RPS": 3,
                        "Percent50": 800,
                        "Percent75": 1153,
                        "Percent95": 1427,
                        "Percent99": 1486,
                        "StdDev": 438,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 1049974.47,
                        "MaxDataKb": 2089670.0,
                        "AllDataMB": 251228.67,
                        "ErrorStats": [
                            {
                                "ErrorCode": 501,
                                "Message": "System.Exception: Not Implemented",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 431,
                                "Message": "System.Exception: Request Header Fields Too Large",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 409,
                                "Message": "System.Exception: Conflict",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 411,
                                "Message": "System.Exception: Length Required",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 504,
                                "Message": "System.Exception: Gateway Timeout",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 404,
                                "Message": "System.Exception: Not Found",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 500,
                                "Message": "System.Exception: Internal Server Error",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 422,
                                "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 412,
                                "Message": "System.Exception: Precondition Failed",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 402,
                                "Message": "System.Exception: Payment Required",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 451,
                                "Message": "System.Exception: Unavailable For Legal Reasons",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 407,
                                "Message": "System.Exception: Proxy Authentication Required",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 413,
                                "Message": "System.Exception: Payload Too Large",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 503,
                                "Message": "System.Exception: Service Unavailable",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 410,
                                "Message": "System.Exception: Gone",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 510,
                                "Message": "System.Exception: Not Extended",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 400,
                                "Message": "System.Exception: Bad Request",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 506,
                                "Message": "System.Exception: Variant Also Negotiates",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 415,
                                "Message": "System.Exception: Unsupported Media Type",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 403,
                                "Message": "System.Exception: Forbidden",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 417,
                                "Message": "System.Exception: Expectation Failed",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 511,
                                "Message": "System.Exception: Network Authentication Required",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 421,
                                "Message": "System.Exception: Misdirected Request",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 428,
                                "Message": "System.Exception: Precondition Required",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 414,
                                "Message": "System.Exception: URI Too Long",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 418,
                                "Message": "System.Exception: I'm a teapot",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 405,
                                "Message": "System.Exception: Method Not Allowed",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 416,
                                "Message": "System.Exception: Requested Range Not Satisfiable",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 507,
                                "Message": "System.Exception: Insufficient Storage",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 406,
                                "Message": "System.Exception: Not Acceptable",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 505,
                                "Message": "System.Exception: HTTP Version Not Supported",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 424,
                                "Message": "System.Exception: Failed Dependency (WebDAV)",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 426,
                                "Message": "System.Exception: Upgrade Required",
                                "Count": 1
                            }
                        ]
                    },
                    {
                        "StepName": "pull html 5",
                        "RequestCount": 499,
                        "OkCount": 420,
                        "FailCount": 79,
                        "Min": 12,
                        "Mean": 740,
                        "Max": 1501,
                        "RPS": 3,
                        "Percent50": 724,
                        "Percent75": 1092,
                        "Percent95": 1406,
                        "Percent99": 1487,
                        "StdDev": 412,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 1013624.74,
                        "MaxDataKb": 2091246.99,
                        "AllDataMB": 192934.31,
                        "ErrorStats": [
                            {
                                "ErrorCode": 0,
                                "Message": "System.Exception: unknown client's error",
                                "Count": 79
                            }
                        ]
                    }
                ],
                "LatencyCount": {
                    "Less800": 792,
                    "More800Less1200": 417,
                    "More1200": 299
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 10
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 429,
                        "Message": "System.Exception: Too Many Requests",
                        "Count": 1
                    },
                    {
                        "ErrorCode": 416,
                        "Message": "System.Exception: Requested Range Not Satisfiable",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 408,
                        "Message": "System.Exception: Request Timeout",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 415,
                        "Message": "System.Exception: Unsupported Media Type",
                        "Count": 8
                    },
                    {
                        "ErrorCode": 421,
                        "Message": "System.Exception: Misdirected Request",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 413,
                        "Message": "System.Exception: Payload Too Large",
                        "Count": 7
                    },
                    {
                        "ErrorCode": 418,
                        "Message": "System.Exception: I'm a teapot",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 428,
                        "Message": "System.Exception: Precondition Required",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 426,
                        "Message": "System.Exception: Upgrade Required",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 403,
                        "Message": "System.Exception: Forbidden",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 409,
                        "Message": "System.Exception: Conflict",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 402,
                        "Message": "System.Exception: Payment Required",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 411,
                        "Message": "System.Exception: Length Required",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 451,
                        "Message": "System.Exception: Unavailable For Legal Reasons",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 404,
                        "Message": "System.Exception: Not Found",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 504,
                        "Message": "System.Exception: Gateway Timeout",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 412,
                        "Message": "System.Exception: Precondition Failed",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 431,
                        "Message": "System.Exception: Request Header Fields Too Large",
                        "Count": 7
                    },
                    {
                        "ErrorCode": 501,
                        "Message": "System.Exception: Not Implemented",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 510,
                        "Message": "System.Exception: Not Extended",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 506,
                        "Message": "System.Exception: Variant Also Negotiates",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 424,
                        "Message": "System.Exception: Failed Dependency (WebDAV)",
                        "Count": 2
                    },
                    {
                        "ErrorCode": 502,
                        "Message": "System.Exception: Bad Gateway",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 423,
                        "Message": "System.Exception: Locked (WebDAV)",
                        "Count": 1
                    },
                    {
                        "ErrorCode": 511,
                        "Message": "System.Exception: Network Authentication Required",
                        "Count": 8
                    },
                    {
                        "ErrorCode": 507,
                        "Message": "System.Exception: Insufficient Storage",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 407,
                        "Message": "System.Exception: Proxy Authentication Required",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 500,
                        "Message": "System.Exception: Internal Server Error",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 503,
                        "Message": "System.Exception: Service Unavailable",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 505,
                        "Message": "System.Exception: HTTP Version Not Supported",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 417,
                        "Message": "System.Exception: Expectation Failed",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 508,
                        "Message": "System.Exception: Loop Detected (WebDAV)",
                        "Count": 2
                    },
                    {
                        "ErrorCode": 414,
                        "Message": "System.Exception: URI Too Long",
                        "Count": 7
                    },
                    {
                        "ErrorCode": 410,
                        "Message": "System.Exception: Gone",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 401,
                        "Message": "System.Exception: Unauthorized",
                        "Count": 2
                    },
                    {
                        "ErrorCode": 422,
                        "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                        "Count": 7
                    },
                    {
                        "ErrorCode": 400,
                        "Message": "System.Exception: Bad Request",
                        "Count": 1
                    },
                    {
                        "ErrorCode": 405,
                        "Message": "System.Exception: Method Not Allowed",
                        "Count": 1
                    },
                    {
                        "ErrorCode": 406,
                        "Message": "System.Exception: Not Acceptable",
                        "Count": 1
                    },
                    {
                        "ErrorCode": 0,
                        "Message": "System.Exception: unknown client's error",
                        "Count": 79
                    }
                ],
                "Duration": "00:01:30"
            }
        ],
        "PluginStats": [
            {
                "TableName": "CustomPlugin1",
                "Columns": [
                    "Property",
                    "Value"
                ],
                "Rows": [
                    [
                        "Property1",
                        "1003438"
                    ],
                    [
                        "Property2",
                        "346989617"
                    ],
                    [
                        "Property3",
                        "1240103244"
                    ]
                ]
            },
            {
                "TableName": "CustomPlugin2",
                "Columns": [
                    "Property",
                    "Value"
                ],
                "Rows": [
                    [
                        "Property 1",
                        "2005721467"
                    ],
                    [
                        "Property 2",
                        "1038058159"
                    ],
                    [
                        "Property 3",
                        "235808509"
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
                "Version": "10.0.18363.0",
                "VersionString": "Microsoft Windows NT 10.0.18363.0"
            },
            "DotNetVersion": ".NETCoreApp,Version=v3.1",
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
            "00:00:40",
            "00:00:45",
            "00:00:50",
            "00:00:55",
            "00:01:00",
            "00:01:05",
            "00:01:10",
            "00:01:15",
            "00:01:20",
            "00:01:25",
            "00:01:30"
        ],
        "ScenarioStats": [
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 2,
                    "OkCount": 2,
                    "FailCount": 0,
                    "AllDataMB": 1750.44,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 2,
                            "OkCount": 2,
                            "FailCount": 0,
                            "Min": 868,
                            "Mean": 932,
                            "Max": 997,
                            "RPS": 0,
                            "Percent50": 868,
                            "Percent75": 997,
                            "Percent95": 997,
                            "Percent99": 997,
                            "StdDev": 64,
                            "MinDataKb": 1792449.36,
                            "MeanDataKb": 1792449.36,
                            "MaxDataKb": 1792449.36,
                            "AllDataMB": 1750.44,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 0,
                        "More800Less1200": 2,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 1
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:05.0031568"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 4,
                    "OkCount": 4,
                    "FailCount": 0,
                    "AllDataMB": 3112.89,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 2,
                            "OkCount": 2,
                            "FailCount": 0,
                            "Min": 123,
                            "Mean": 778,
                            "Max": 1434,
                            "RPS": 0,
                            "Percent50": 123,
                            "Percent75": 1434,
                            "Percent95": 1434,
                            "Percent99": 1434,
                            "StdDev": 656,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 330542.73,
                            "MaxDataKb": 661085.45,
                            "AllDataMB": 645.59,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 2,
                            "OkCount": 2,
                            "FailCount": 0,
                            "Min": 721,
                            "Mean": 1094,
                            "Max": 1467,
                            "RPS": 0,
                            "Percent50": 721,
                            "Percent75": 1467,
                            "Percent95": 1467,
                            "Percent99": 1467,
                            "StdDev": 373,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 631628.17,
                            "MaxDataKb": 1903656.12,
                            "AllDataMB": 2467.3,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 2,
                        "More800Less1200": 0,
                        "More1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:05.0031568"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 10,
                    "OkCount": 7,
                    "FailCount": 3,
                    "AllDataMB": 2066.56,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 6,
                            "OkCount": 4,
                            "FailCount": 2,
                            "Min": 74,
                            "Mean": 666,
                            "Max": 1433,
                            "RPS": 0,
                            "Percent50": 268,
                            "Percent75": 890,
                            "Percent95": 1433,
                            "Percent99": 1433,
                            "StdDev": 536,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 558149.85,
                            "MaxDataKb": 1116299.7,
                            "AllDataMB": 1090.14,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 3,
                            "OkCount": 2,
                            "FailCount": 1,
                            "Min": 42,
                            "Mean": 221,
                            "Max": 401,
                            "RPS": 0,
                            "Percent50": 42,
                            "Percent75": 401,
                            "Percent95": 401,
                            "Percent99": 401,
                            "StdDev": 180,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 499926.49,
                            "MaxDataKb": 999852.98,
                            "AllDataMB": 976.42,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 1,
                            "OkCount": 1,
                            "FailCount": 0,
                            "Min": 94,
                            "Mean": 94,
                            "Max": 94,
                            "RPS": 0,
                            "Percent50": 94,
                            "Percent75": 94,
                            "Percent95": 94,
                            "Percent99": 94,
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
                        "More800Less1200": 1,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        }
                    ],
                    "Duration": "00:00:05.0031568"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 5,
                    "OkCount": 5,
                    "FailCount": 0,
                    "AllDataMB": 2497.3700000000003,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 7,
                            "OkCount": 7,
                            "FailCount": 0,
                            "Min": 279,
                            "Mean": 982,
                            "Max": 1486,
                            "RPS": 0,
                            "Percent50": 967,
                            "Percent75": 997,
                            "Percent95": 1486,
                            "Percent99": 1486,
                            "StdDev": 374,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 543719.69,
                            "MaxDataKb": 1857811.07,
                            "AllDataMB": 4247.81,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 1,
                        "More800Less1200": 2,
                        "More1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:10.0080151"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 14,
                    "OkCount": 14,
                    "FailCount": 0,
                    "AllDataMB": 7475.51,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 9,
                            "OkCount": 9,
                            "FailCount": 0,
                            "Min": 123,
                            "Mean": 868,
                            "Max": 1434,
                            "RPS": 0,
                            "Percent50": 965,
                            "Percent75": 1088,
                            "Percent95": 1434,
                            "Percent99": 1434,
                            "StdDev": 449,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 494634.52,
                            "MaxDataKb": 871455.36,
                            "AllDataMB": 4149.54,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 9,
                            "OkCount": 9,
                            "FailCount": 0,
                            "Min": 250,
                            "Mean": 743,
                            "Max": 1467,
                            "RPS": 0,
                            "Percent50": 721,
                            "Percent75": 929,
                            "Percent95": 1467,
                            "Percent99": 1467,
                            "StdDev": 353,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 786228.03,
                            "MaxDataKb": 1922585.79,
                            "AllDataMB": 6438.86,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 7,
                        "More800Less1200": 6,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 4
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:10.0080151"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 17,
                    "OkCount": 15,
                    "FailCount": 2,
                    "AllDataMB": 11985.480000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 12,
                            "OkCount": 10,
                            "FailCount": 2,
                            "Min": 74,
                            "Mean": 691,
                            "Max": 1433,
                            "RPS": 0,
                            "Percent50": 504,
                            "Percent75": 890,
                            "Percent95": 1433,
                            "Percent99": 1433,
                            "StdDev": 421,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 792136.47,
                            "MaxDataKb": 1858473.94,
                            "AllDataMB": 4641.42,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 10,
                            "OkCount": 7,
                            "FailCount": 3,
                            "Min": 42,
                            "Mean": 480,
                            "Max": 1182,
                            "RPS": 0,
                            "Percent50": 325,
                            "Percent75": 401,
                            "Percent95": 1182,
                            "Percent99": 1182,
                            "StdDev": 432,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 908740.71,
                            "MaxDataKb": 1822096.55,
                            "AllDataMB": 4040.23,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 5,
                            "OkCount": 5,
                            "FailCount": 0,
                            "Min": 94,
                            "Mean": 724,
                            "Max": 1482,
                            "RPS": 0,
                            "Percent50": 559,
                            "Percent75": 1146,
                            "Percent95": 1482,
                            "Percent99": 1482,
                            "StdDev": 515,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1025520.89,
                            "MaxDataKb": 1865201.86,
                            "AllDataMB": 5370.39,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 9,
                        "More800Less1200": 4,
                        "More1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 4
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 1
                        }
                    ],
                    "Duration": "00:00:10.0080151"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 14,
                    "OkCount": 14,
                    "FailCount": 0,
                    "AllDataMB": 8887.36,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 21,
                            "OkCount": 21,
                            "FailCount": 0,
                            "Min": 47,
                            "Mean": 811,
                            "Max": 1486,
                            "RPS": 1,
                            "Percent50": 851,
                            "Percent75": 1121,
                            "Percent95": 1432,
                            "Percent99": 1486,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 719295.36,
                            "MaxDataKb": 2015862.83,
                            "AllDataMB": 13135.17,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 9,
                        "More800Less1200": 2,
                        "More1200": 3
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:15.0051632"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 27,
                    "OkCount": 26,
                    "FailCount": 1,
                    "AllDataMB": 10194.35,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 24,
                            "OkCount": 24,
                            "FailCount": 0,
                            "Min": 100,
                            "Mean": 829,
                            "Max": 1489,
                            "RPS": 1,
                            "Percent50": 950,
                            "Percent75": 1211,
                            "Percent95": 1434,
                            "Percent99": 1489,
                            "StdDev": 470,
                            "MinDataKb": 115720.73,
                            "MeanDataKb": 834276.06,
                            "MaxDataKb": 1456169.34,
                            "AllDataMB": 9635.38,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 21,
                            "OkCount": 20,
                            "FailCount": 1,
                            "Min": 60,
                            "Mean": 759,
                            "Max": 1467,
                            "RPS": 0,
                            "Percent50": 695,
                            "Percent75": 1092,
                            "Percent95": 1462,
                            "Percent99": 1467,
                            "StdDev": 439,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 742170.91,
                            "MaxDataKb": 1955031.2,
                            "AllDataMB": 11147.37,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 1
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 14,
                        "More800Less1200": 3,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 1
                        }
                    ],
                    "Duration": "00:00:15.0051632"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 23,
                    "OkCount": 21,
                    "FailCount": 2,
                    "AllDataMB": 7111.379999999997,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 20,
                            "OkCount": 18,
                            "FailCount": 2,
                            "Min": 74,
                            "Mean": 698,
                            "Max": 1439,
                            "RPS": 0,
                            "Percent50": 592,
                            "Percent75": 890,
                            "Percent95": 1433,
                            "Percent99": 1439,
                            "StdDev": 420,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 599762.14,
                            "MaxDataKb": 1858473.94,
                            "AllDataMB": 7940.17,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 17,
                            "OkCount": 14,
                            "FailCount": 3,
                            "Min": 42,
                            "Mean": 662,
                            "Max": 1498,
                            "RPS": 0,
                            "Percent50": 411,
                            "Percent75": 1107,
                            "Percent95": 1182,
                            "Percent99": 1498,
                            "StdDev": 462,
                            "MinDataKb": 50247.91,
                            "MeanDataKb": 912672.9,
                            "MaxDataKb": 1822096.55,
                            "AllDataMB": 6501.06,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 13,
                            "OkCount": 11,
                            "FailCount": 2,
                            "Min": 94,
                            "Mean": 837,
                            "Max": 1482,
                            "RPS": 0,
                            "Percent50": 562,
                            "Percent75": 1347,
                            "Percent95": 1481,
                            "Percent99": 1482,
                            "StdDev": 513,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 567215.75,
                            "MaxDataKb": 1865201.86,
                            "AllDataMB": 6722.19,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 2
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 11,
                        "More800Less1200": 4,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 2
                        }
                    ],
                    "Duration": "00:00:15.0051632"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 18,
                    "OkCount": 18,
                    "FailCount": 0,
                    "AllDataMB": 7446.609999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 39,
                            "OkCount": 39,
                            "FailCount": 0,
                            "Min": 47,
                            "Mean": 797,
                            "Max": 1486,
                            "RPS": 1,
                            "Percent50": 814,
                            "Percent75": 1121,
                            "Percent95": 1432,
                            "Percent99": 1486,
                            "StdDev": 416,
                            "MinDataKb": 4153.13,
                            "MeanDataKb": 794867.54,
                            "MaxDataKb": 2015862.83,
                            "AllDataMB": 20581.78,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 9,
                        "More800Less1200": 6,
                        "More1200": 3
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:20.0084156"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 33,
                    "OkCount": 31,
                    "FailCount": 2,
                    "AllDataMB": 14919.11,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 40,
                            "OkCount": 40,
                            "FailCount": 0,
                            "Min": 78,
                            "Mean": 730,
                            "Max": 1489,
                            "RPS": 1,
                            "Percent50": 668,
                            "Percent75": 1169,
                            "Percent95": 1433,
                            "Percent99": 1489,
                            "StdDev": 469,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 789511.27,
                            "MaxDataKb": 2003524.94,
                            "AllDataMB": 16052.86,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 38,
                            "OkCount": 35,
                            "FailCount": 3,
                            "Min": 60,
                            "Mean": 900,
                            "Max": 1483,
                            "RPS": 1,
                            "Percent50": 906,
                            "Percent75": 1298,
                            "Percent95": 1462,
                            "Percent99": 1483,
                            "StdDev": 422,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 873336.84,
                            "MaxDataKb": 1955031.2,
                            "AllDataMB": 19649.0,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 3
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 16,
                        "More800Less1200": 4,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 3
                        }
                    ],
                    "Duration": "00:00:20.0084156"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 36,
                    "OkCount": 27,
                    "FailCount": 9,
                    "AllDataMB": 12751.490000000005,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 38,
                            "OkCount": 31,
                            "FailCount": 7,
                            "Min": 74,
                            "Mean": 760,
                            "Max": 1480,
                            "RPS": 1,
                            "Percent50": 705,
                            "Percent75": 1018,
                            "Percent95": 1433,
                            "Percent99": 1480,
                            "StdDev": 402,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 749512.76,
                            "MaxDataKb": 1858473.94,
                            "AllDataMB": 13878.19,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 27,
                            "OkCount": 22,
                            "FailCount": 5,
                            "Min": 42,
                            "Mean": 732,
                            "Max": 1498,
                            "RPS": 0,
                            "Percent50": 737,
                            "Percent75": 1107,
                            "Percent95": 1313,
                            "Percent99": 1498,
                            "StdDev": 428,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 856820.36,
                            "MaxDataKb": 2021426.24,
                            "AllDataMB": 12194.58,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 21,
                            "OkCount": 17,
                            "FailCount": 4,
                            "Min": 94,
                            "Mean": 923,
                            "Max": 1493,
                            "RPS": 0,
                            "Percent50": 955,
                            "Percent75": 1347,
                            "Percent95": 1482,
                            "Percent99": 1493,
                            "StdDev": 465,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 568988.12,
                            "MaxDataKb": 1865201.86,
                            "AllDataMB": 7842.14,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 4
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 11,
                        "More800Less1200": 9,
                        "More1200": 7
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 4
                        }
                    ],
                    "Duration": "00:00:20.0084156"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 23,
                    "OkCount": 23,
                    "FailCount": 0,
                    "AllDataMB": 10739.11,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 62,
                            "OkCount": 62,
                            "FailCount": 0,
                            "Min": 47,
                            "Mean": 788,
                            "Max": 1486,
                            "RPS": 1,
                            "Percent50": 714,
                            "Percent75": 1155,
                            "Percent95": 1456,
                            "Percent99": 1465,
                            "StdDev": 420,
                            "MinDataKb": 4153.13,
                            "MeanDataKb": 856718.17,
                            "MaxDataKb": 2015862.83,
                            "AllDataMB": 31320.89,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 14,
                        "More800Less1200": 3,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 4
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:25.0015869"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 54,
                    "OkCount": 50,
                    "FailCount": 4,
                    "AllDataMB": 22329.589999999997,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 68,
                            "OkCount": 68,
                            "FailCount": 0,
                            "Min": 31,
                            "Mean": 693,
                            "Max": 1489,
                            "RPS": 2,
                            "Percent50": 614,
                            "Percent75": 1002,
                            "Percent95": 1416,
                            "Percent99": 1434,
                            "StdDev": 432,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 837045.81,
                            "MaxDataKb": 2003524.94,
                            "AllDataMB": 30002.81,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 64,
                            "OkCount": 57,
                            "FailCount": 7,
                            "Min": 11,
                            "Mean": 860,
                            "Max": 1483,
                            "RPS": 1,
                            "Percent50": 875,
                            "Percent75": 1331,
                            "Percent95": 1447,
                            "Percent99": 1467,
                            "StdDev": 452,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 891633.75,
                            "MaxDataKb": 1955031.2,
                            "AllDataMB": 28028.64,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 7
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 29,
                        "More800Less1200": 10,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 9
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 7
                        }
                    ],
                    "Duration": "00:00:25.0015869"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 51,
                    "OkCount": 42,
                    "FailCount": 9,
                    "AllDataMB": 18531.079999999994,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 58,
                            "OkCount": 46,
                            "FailCount": 12,
                            "Min": 74,
                            "Mean": 773,
                            "Max": 1494,
                            "RPS": 1,
                            "Percent50": 705,
                            "Percent75": 1047,
                            "Percent95": 1460,
                            "Percent99": 1494,
                            "StdDev": 412,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 962482.19,
                            "MaxDataKb": 2083570.1,
                            "AllDataMB": 21724.99,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 44,
                            "OkCount": 37,
                            "FailCount": 7,
                            "Min": 16,
                            "Mean": 723,
                            "Max": 1498,
                            "RPS": 0,
                            "Percent50": 700,
                            "Percent75": 1109,
                            "Percent95": 1311,
                            "Percent99": 1498,
                            "StdDev": 428,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 892149.71,
                            "MaxDataKb": 2021426.24,
                            "AllDataMB": 19489.89,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 35,
                            "OkCount": 29,
                            "FailCount": 6,
                            "Min": 94,
                            "Mean": 763,
                            "Max": 1493,
                            "RPS": 0,
                            "Percent50": 640,
                            "Percent75": 1146,
                            "Percent95": 1482,
                            "Percent99": 1493,
                            "StdDev": 440,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 626471.28,
                            "MaxDataKb": 1865201.86,
                            "AllDataMB": 11231.11,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 6
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 27,
                        "More800Less1200": 8,
                        "More1200": 7
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 9
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 6
                        }
                    ],
                    "Duration": "00:00:25.0015869"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 31,
                    "OkCount": 31,
                    "FailCount": 0,
                    "AllDataMB": 12426.980000000003,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 93,
                            "OkCount": 93,
                            "FailCount": 0,
                            "Min": 18,
                            "Mean": 768,
                            "Max": 1501,
                            "RPS": 2,
                            "Percent50": 776,
                            "Percent75": 1090,
                            "Percent95": 1456,
                            "Percent99": 1496,
                            "StdDev": 422,
                            "MinDataKb": 4153.13,
                            "MeanDataKb": 951367.16,
                            "MaxDataKb": 2042880.69,
                            "AllDataMB": 43747.87,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 16,
                        "More800Less1200": 10,
                        "More1200": 5
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:30.0053147"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 65,
                    "OkCount": 61,
                    "FailCount": 4,
                    "AllDataMB": 34711.79000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 100,
                            "OkCount": 100,
                            "FailCount": 0,
                            "Min": 31,
                            "Mean": 700,
                            "Max": 1489,
                            "RPS": 2,
                            "Percent50": 661,
                            "Percent75": 1055,
                            "Percent95": 1398,
                            "Percent99": 1434,
                            "StdDev": 410,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 883576.78,
                            "MaxDataKb": 2033664.22,
                            "AllDataMB": 46456.49,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 97,
                            "OkCount": 86,
                            "FailCount": 11,
                            "Min": 11,
                            "Mean": 817,
                            "Max": 1483,
                            "RPS": 1,
                            "Percent50": 817,
                            "Percent75": 1281,
                            "Percent95": 1447,
                            "Percent99": 1467,
                            "StdDev": 452,
                            "MinDataKb": 101286.21,
                            "MeanDataKb": 1130962.56,
                            "MaxDataKb": 2028427.07,
                            "AllDataMB": 46286.75,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 11
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 36,
                        "More800Less1200": 16,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 11
                        }
                    ],
                    "Duration": "00:00:30.0053147"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 57,
                    "OkCount": 53,
                    "FailCount": 4,
                    "AllDataMB": 23803.43,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 81,
                            "OkCount": 66,
                            "FailCount": 15,
                            "Min": 74,
                            "Mean": 763,
                            "Max": 1494,
                            "RPS": 1,
                            "Percent50": 705,
                            "Percent75": 1047,
                            "Percent95": 1439,
                            "Percent99": 1480,
                            "StdDev": 405,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 691273.71,
                            "MaxDataKb": 2083570.1,
                            "AllDataMB": 27570.57,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 62,
                            "OkCount": 54,
                            "FailCount": 8,
                            "Min": 16,
                            "Mean": 750,
                            "Max": 1498,
                            "RPS": 1,
                            "Percent50": 700,
                            "Percent75": 1142,
                            "Percent95": 1367,
                            "Percent99": 1486,
                            "StdDev": 434,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 935101.58,
                            "MaxDataKb": 2021426.24,
                            "AllDataMB": 28406.9,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 51,
                            "OkCount": 45,
                            "FailCount": 6,
                            "Min": 15,
                            "Mean": 750,
                            "Max": 1493,
                            "RPS": 1,
                            "Percent50": 703,
                            "Percent75": 1039,
                            "Percent95": 1481,
                            "Percent99": 1493,
                            "StdDev": 422,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 829215.65,
                            "MaxDataKb": 1865201.86,
                            "AllDataMB": 20271.95,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 6
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 29,
                        "More800Less1200": 13,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 6
                        }
                    ],
                    "Duration": "00:00:30.0053147"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 51,
                    "OkCount": 51,
                    "FailCount": 0,
                    "AllDataMB": 23368.519999999997,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 144,
                            "OkCount": 144,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 793,
                            "Max": 1501,
                            "RPS": 2,
                            "Percent50": 780,
                            "Percent75": 1129,
                            "Percent95": 1444,
                            "Percent99": 1496,
                            "StdDev": 418,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 755449.13,
                            "MaxDataKb": 2054323.88,
                            "AllDataMB": 67116.39,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 25,
                        "More800Less1200": 13,
                        "More1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:35.0024224"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 70,
                    "OkCount": 65,
                    "FailCount": 5,
                    "AllDataMB": 22168.65999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 136,
                            "OkCount": 136,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 712,
                            "Max": 1489,
                            "RPS": 2,
                            "Percent50": 741,
                            "Percent75": 1055,
                            "Percent95": 1314,
                            "Percent99": 1434,
                            "StdDev": 408,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 759816.48,
                            "MaxDataKb": 2033664.22,
                            "AllDataMB": 62219.61,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 131,
                            "OkCount": 115,
                            "FailCount": 16,
                            "Min": 11,
                            "Mean": 768,
                            "Max": 1483,
                            "RPS": 2,
                            "Percent50": 724,
                            "Percent75": 1125,
                            "Percent95": 1434,
                            "Percent99": 1467,
                            "StdDev": 434,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 484895.75,
                            "MaxDataKb": 2028427.07,
                            "AllDataMB": 52692.29,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 16
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 35,
                        "More800Less1200": 25,
                        "More1200": 5
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 16
                        }
                    ],
                    "Duration": "00:00:35.0024224"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 116,
                    "OkCount": 95,
                    "FailCount": 21,
                    "AllDataMB": 43933.270000000004,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 133,
                            "OkCount": 108,
                            "FailCount": 25,
                            "Min": 29,
                            "Mean": 756,
                            "Max": 1495,
                            "RPS": 2,
                            "Percent50": 705,
                            "Percent75": 1076,
                            "Percent95": 1439,
                            "Percent99": 1494,
                            "StdDev": 409,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 548684.65,
                            "MaxDataKb": 2083570.1,
                            "AllDataMB": 44857.75,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 98,
                            "OkCount": 87,
                            "FailCount": 11,
                            "Min": 16,
                            "Mean": 731,
                            "Max": 1498,
                            "RPS": 1,
                            "Percent50": 699,
                            "Percent75": 1125,
                            "Percent95": 1367,
                            "Percent99": 1486,
                            "StdDev": 428,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 574928.19,
                            "MaxDataKb": 2021426.24,
                            "AllDataMB": 43620.18,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 80,
                            "OkCount": 66,
                            "FailCount": 14,
                            "Min": 15,
                            "Mean": 765,
                            "Max": 1493,
                            "RPS": 1,
                            "Percent50": 715,
                            "Percent75": 1108,
                            "Percent95": 1427,
                            "Percent99": 1482,
                            "StdDev": 414,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 405837.53,
                            "MaxDataKb": 1965363.85,
                            "AllDataMB": 31704.76,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 14
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 53,
                        "More800Less1200": 26,
                        "More1200": 15
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 14
                        }
                    ],
                    "Duration": "00:00:35.0024224"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 50,
                    "OkCount": 50,
                    "FailCount": 0,
                    "AllDataMB": 34690.57000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 194,
                            "OkCount": 194,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 777,
                            "Max": 1512,
                            "RPS": 3,
                            "Percent50": 761,
                            "Percent75": 1121,
                            "Percent95": 1432,
                            "Percent99": 1496,
                            "StdDev": 414,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1033152.27,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 101806.96,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 28,
                        "More800Less1200": 13,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:40.0116421"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 49,
                    "OkCount": 46,
                    "FailCount": 3,
                    "AllDataMB": 25154.72,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 160,
                            "OkCount": 160,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 738,
                            "Max": 1489,
                            "RPS": 2,
                            "Percent50": 781,
                            "Percent75": 1088,
                            "Percent95": 1398,
                            "Percent99": 1466,
                            "StdDev": 411,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 882181.07,
                            "MaxDataKb": 2033664.22,
                            "AllDataMB": 77897.51,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 156,
                            "OkCount": 137,
                            "FailCount": 19,
                            "Min": 11,
                            "Mean": 762,
                            "Max": 1486,
                            "RPS": 2,
                            "Percent50": 724,
                            "Percent75": 1140,
                            "Percent95": 1436,
                            "Percent99": 1483,
                            "StdDev": 439,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 757427.48,
                            "MaxDataKb": 2028427.07,
                            "AllDataMB": 62169.11,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 19
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 21,
                        "More800Less1200": 15,
                        "More1200": 10
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 19
                        }
                    ],
                    "Duration": "00:00:40.0116421"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 129,
                    "OkCount": 109,
                    "FailCount": 20,
                    "AllDataMB": 58266.45000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 182,
                            "OkCount": 151,
                            "FailCount": 31,
                            "Min": 14,
                            "Mean": 752,
                            "Max": 1497,
                            "RPS": 2,
                            "Percent50": 744,
                            "Percent75": 1091,
                            "Percent95": 1439,
                            "Percent99": 1495,
                            "StdDev": 424,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 866189.49,
                            "MaxDataKb": 2083570.1,
                            "AllDataMB": 66642.36,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 140,
                            "OkCount": 122,
                            "FailCount": 18,
                            "Min": 16,
                            "Mean": 733,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 690,
                            "Percent75": 1107,
                            "Percent95": 1367,
                            "Percent99": 1486,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 917861.34,
                            "MaxDataKb": 2021426.24,
                            "AllDataMB": 63959.88,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 117,
                            "OkCount": 96,
                            "FailCount": 21,
                            "Min": 15,
                            "Mean": 774,
                            "Max": 1493,
                            "RPS": 1,
                            "Percent50": 757,
                            "Percent75": 1140,
                            "Percent95": 1408,
                            "Percent99": 1482,
                            "StdDev": 415,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 657418.26,
                            "MaxDataKb": 1999657.65,
                            "AllDataMB": 47846.9,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 21
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 55,
                        "More800Less1200": 32,
                        "More1200": 22
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 21
                        }
                    ],
                    "Duration": "00:00:40.0116421"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 48,
                    "OkCount": 48,
                    "FailCount": 0,
                    "AllDataMB": 19171.949999999997,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 243,
                            "OkCount": 243,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 773,
                            "Max": 1512,
                            "RPS": 3,
                            "Percent50": 768,
                            "Percent75": 1119,
                            "Percent95": 1432,
                            "Percent99": 1498,
                            "StdDev": 418,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 927303.24,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 120978.91,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 25,
                        "More800Less1200": 15,
                        "More1200": 8
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:45.0020019"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 51,
                    "OkCount": 46,
                    "FailCount": 5,
                    "AllDataMB": 20783.95000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 186,
                            "OkCount": 186,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 751,
                            "Max": 1489,
                            "RPS": 2,
                            "Percent50": 789,
                            "Percent75": 1105,
                            "Percent95": 1414,
                            "Percent99": 1466,
                            "StdDev": 418,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 855000.67,
                            "MaxDataKb": 2033664.22,
                            "AllDataMB": 88561.46,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 181,
                            "OkCount": 157,
                            "FailCount": 24,
                            "Min": 11,
                            "Mean": 743,
                            "Max": 1486,
                            "RPS": 2,
                            "Percent50": 724,
                            "Percent75": 1111,
                            "Percent95": 1434,
                            "Percent99": 1467,
                            "StdDev": 433,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 883443.34,
                            "MaxDataKb": 2052788.17,
                            "AllDataMB": 72289.11,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 24
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 24,
                        "More800Less1200": 16,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 24
                        }
                    ],
                    "Duration": "00:00:45.0020019"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 135,
                    "OkCount": 121,
                    "FailCount": 14,
                    "AllDataMB": 53457.639999999985,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 233,
                            "OkCount": 194,
                            "FailCount": 39,
                            "Min": 14,
                            "Mean": 744,
                            "Max": 1497,
                            "RPS": 2,
                            "Percent50": 728,
                            "Percent75": 1078,
                            "Percent95": 1439,
                            "Percent99": 1495,
                            "StdDev": 425,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 893377.21,
                            "MaxDataKb": 2083570.1,
                            "AllDataMB": 92122.69,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 183,
                            "OkCount": 161,
                            "FailCount": 22,
                            "Min": 9,
                            "Mean": 756,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 796,
                            "Percent75": 1102,
                            "Percent95": 1391,
                            "Percent99": 1486,
                            "StdDev": 415,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1003433.69,
                            "MaxDataKb": 2021426.24,
                            "AllDataMB": 72982.41,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 158,
                            "OkCount": 135,
                            "FailCount": 23,
                            "Min": 15,
                            "Mean": 756,
                            "Max": 1493,
                            "RPS": 1,
                            "Percent50": 757,
                            "Percent75": 1093,
                            "Percent95": 1398,
                            "Percent99": 1482,
                            "StdDev": 404,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 927045.26,
                            "MaxDataKb": 2051994.38,
                            "AllDataMB": 66801.68,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 23
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 61,
                        "More800Less1200": 40,
                        "More1200": 20
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 23
                        }
                    ],
                    "Duration": "00:00:45.0020019"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 51,
                    "OkCount": 51,
                    "FailCount": 0,
                    "AllDataMB": 21535.100000000006,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 293,
                            "OkCount": 293,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 776,
                            "Max": 1512,
                            "RPS": 3,
                            "Percent50": 776,
                            "Percent75": 1121,
                            "Percent95": 1427,
                            "Percent99": 1496,
                            "StdDev": 412,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 919939.03,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 142514.01,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 26,
                        "More800Less1200": 14,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:50.0140005"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 48,
                    "OkCount": 47,
                    "FailCount": 1,
                    "AllDataMB": 22576.179999999993,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 211,
                            "OkCount": 211,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 772,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 798,
                            "Percent75": 1124,
                            "Percent95": 1416,
                            "Percent99": 1482,
                            "StdDev": 424,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 877806.41,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 100487.74,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 204,
                            "OkCount": 179,
                            "FailCount": 25,
                            "Min": 11,
                            "Mean": 755,
                            "Max": 1486,
                            "RPS": 2,
                            "Percent50": 724,
                            "Percent75": 1115,
                            "Percent95": 1434,
                            "Percent99": 1475,
                            "StdDev": 428,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 860077.5,
                            "MaxDataKb": 2052788.17,
                            "AllDataMB": 82939.01,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 25
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 18,
                        "More800Less1200": 15,
                        "More1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 25
                        }
                    ],
                    "Duration": "00:00:50.0140005"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 127,
                    "OkCount": 107,
                    "FailCount": 20,
                    "AllDataMB": 48495.25999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 279,
                            "OkCount": 230,
                            "FailCount": 49,
                            "Min": 14,
                            "Mean": 745,
                            "Max": 1497,
                            "RPS": 3,
                            "Percent50": 728,
                            "Percent75": 1091,
                            "Percent95": 1460,
                            "Percent99": 1495,
                            "StdDev": 427,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 876933.44,
                            "MaxDataKb": 2083570.1,
                            "AllDataMB": 109245.44,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 225,
                            "OkCount": 200,
                            "FailCount": 25,
                            "Min": 9,
                            "Mean": 745,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 755,
                            "Percent75": 1098,
                            "Percent95": 1410,
                            "Percent99": 1486,
                            "StdDev": 427,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1067675.7,
                            "MaxDataKb": 2060273.84,
                            "AllDataMB": 95962.23,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 197,
                            "OkCount": 167,
                            "FailCount": 30,
                            "Min": 12,
                            "Mean": 719,
                            "Max": 1493,
                            "RPS": 2,
                            "Percent50": 675,
                            "Percent75": 1063,
                            "Percent95": 1398,
                            "Percent99": 1482,
                            "StdDev": 417,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 937359.89,
                            "MaxDataKb": 2051994.38,
                            "AllDataMB": 75194.37,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 30
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 66,
                        "More800Less1200": 21,
                        "More1200": 19
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 30
                        }
                    ],
                    "Duration": "00:00:50.0140005"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 48,
                    "OkCount": 48,
                    "FailCount": 0,
                    "AllDataMB": 24980.22,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 341,
                            "OkCount": 341,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 767,
                            "Max": 1512,
                            "RPS": 4,
                            "Percent50": 761,
                            "Percent75": 1104,
                            "Percent95": 1427,
                            "Percent99": 1498,
                            "StdDev": 411,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 918554.65,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 167494.23,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 29,
                        "More800Less1200": 11,
                        "More1200": 8
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:55.0068646"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 53,
                    "OkCount": 52,
                    "FailCount": 1,
                    "AllDataMB": 24771.76000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 236,
                            "OkCount": 236,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 764,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 797,
                            "Percent75": 1118,
                            "Percent95": 1416,
                            "Percent99": 1485,
                            "StdDev": 417,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 882292.83,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 111132.36,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 232,
                            "OkCount": 206,
                            "FailCount": 26,
                            "Min": 11,
                            "Mean": 743,
                            "Max": 1499,
                            "RPS": 2,
                            "Percent50": 695,
                            "Percent75": 1110,
                            "Percent95": 1434,
                            "Percent99": 1483,
                            "StdDev": 426,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 909690.9,
                            "MaxDataKb": 2052788.17,
                            "AllDataMB": 97066.15,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 26
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 34,
                        "More800Less1200": 12,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 26
                        }
                    ],
                    "Duration": "00:00:55.0068646"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 131,
                    "OkCount": 118,
                    "FailCount": 13,
                    "AllDataMB": 56077.890000000014,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 333,
                            "OkCount": 280,
                            "FailCount": 53,
                            "Min": 14,
                            "Mean": 740,
                            "Max": 1497,
                            "RPS": 3,
                            "Percent50": 728,
                            "Percent75": 1090,
                            "Percent95": 1439,
                            "Percent99": 1495,
                            "StdDev": 428,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 880496.38,
                            "MaxDataKb": 2088145.66,
                            "AllDataMB": 130922.25,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 269,
                            "OkCount": 237,
                            "FailCount": 32,
                            "Min": 9,
                            "Mean": 763,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 811,
                            "Percent75": 1109,
                            "Percent95": 1410,
                            "Percent99": 1486,
                            "StdDev": 431,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1119934.74,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 120238.5,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 233,
                            "OkCount": 201,
                            "FailCount": 32,
                            "Min": 12,
                            "Mean": 717,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 703,
                            "Percent75": 1092,
                            "Percent95": 1403,
                            "Percent99": 1487,
                            "StdDev": 424,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 949357.29,
                            "MaxDataKb": 2051994.38,
                            "AllDataMB": 85319.18,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 32
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 62,
                        "More800Less1200": 31,
                        "More1200": 27
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 32
                        }
                    ],
                    "Duration": "00:00:55.0068646"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 52,
                    "OkCount": 52,
                    "FailCount": 0,
                    "AllDataMB": 23464.79999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 393,
                            "OkCount": 393,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 763,
                            "Max": 1512,
                            "RPS": 4,
                            "Percent50": 748,
                            "Percent75": 1110,
                            "Percent95": 1427,
                            "Percent99": 1496,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 921507.7,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 190959.03,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 29,
                        "More800Less1200": 12,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:00.0079488"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 53,
                    "OkCount": 51,
                    "FailCount": 2,
                    "AllDataMB": 28149.190000000002,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 262,
                            "OkCount": 262,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 759,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 794,
                            "Percent75": 1109,
                            "Percent95": 1416,
                            "Percent99": 1482,
                            "StdDev": 410,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 940790.41,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 125640.68,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 259,
                            "OkCount": 231,
                            "FailCount": 28,
                            "Min": 11,
                            "Mean": 763,
                            "Max": 1499,
                            "RPS": 2,
                            "Percent50": 779,
                            "Percent75": 1124,
                            "Percent95": 1414,
                            "Percent99": 1483,
                            "StdDev": 428,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1019564.41,
                            "MaxDataKb": 2052788.17,
                            "AllDataMB": 110707.02,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 28
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 21,
                        "More800Less1200": 21,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 28
                        }
                    ],
                    "Duration": "00:01:00.0079488"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 131,
                    "OkCount": 111,
                    "FailCount": 20,
                    "AllDataMB": 56101.100000000035,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 381,
                            "OkCount": 320,
                            "FailCount": 61,
                            "Min": 9,
                            "Mean": 725,
                            "Max": 1497,
                            "RPS": 3,
                            "Percent50": 703,
                            "Percent75": 1079,
                            "Percent95": 1435,
                            "Percent99": 1495,
                            "StdDev": 431,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 876483.89,
                            "MaxDataKb": 2088145.66,
                            "AllDataMB": 151807.22,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 314,
                            "OkCount": 276,
                            "FailCount": 38,
                            "Min": 9,
                            "Mean": 776,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 813,
                            "Percent75": 1125,
                            "Percent95": 1411,
                            "Percent99": 1481,
                            "StdDev": 424,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1115102.39,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 141500.82,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 268,
                            "OkCount": 230,
                            "FailCount": 38,
                            "Min": 12,
                            "Mean": 732,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 731,
                            "Percent75": 1092,
                            "Percent95": 1403,
                            "Percent99": 1487,
                            "StdDev": 417,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 951550.31,
                            "MaxDataKb": 2051994.38,
                            "AllDataMB": 99272.99,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 38
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 50,
                        "More800Less1200": 43,
                        "More1200": 16
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 38
                        }
                    ],
                    "Duration": "00:01:00.0079488"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 51,
                    "OkCount": 51,
                    "FailCount": 0,
                    "AllDataMB": 30235.940000000002,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 444,
                            "OkCount": 444,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 758,
                            "Max": 1512,
                            "RPS": 4,
                            "Percent50": 748,
                            "Percent75": 1104,
                            "Percent95": 1421,
                            "Percent99": 1496,
                            "StdDev": 415,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 927108.63,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 221194.97,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 28,
                        "More800Less1200": 14,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:05.0069616"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 45,
                    "OkCount": 42,
                    "FailCount": 3,
                    "AllDataMB": 21982.159999999974,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 285,
                            "OkCount": 285,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 756,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 786,
                            "Percent75": 1105,
                            "Percent95": 1420,
                            "Percent99": 1485,
                            "StdDev": 412,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 944774.82,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 138787.93,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 281,
                            "OkCount": 250,
                            "FailCount": 31,
                            "Min": 11,
                            "Mean": 762,
                            "Max": 1499,
                            "RPS": 2,
                            "Percent50": 765,
                            "Percent75": 1125,
                            "Percent95": 1434,
                            "Percent99": 1486,
                            "StdDev": 432,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1011941.47,
                            "MaxDataKb": 2052788.17,
                            "AllDataMB": 119541.93,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 31
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 25,
                        "More800Less1200": 8,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 31
                        }
                    ],
                    "Duration": "00:01:05.0069616"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 130,
                    "OkCount": 109,
                    "FailCount": 21,
                    "AllDataMB": 60342.82999999996,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 429,
                            "OkCount": 363,
                            "FailCount": 66,
                            "Min": 9,
                            "Mean": 739,
                            "Max": 1497,
                            "RPS": 3,
                            "Percent50": 747,
                            "Percent75": 1091,
                            "Percent95": 1436,
                            "Percent99": 1494,
                            "StdDev": 428,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 867751.31,
                            "MaxDataKb": 2088145.66,
                            "AllDataMB": 173768.89,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 358,
                            "OkCount": 311,
                            "FailCount": 47,
                            "Min": 9,
                            "Mean": 788,
                            "Max": 1498,
                            "RPS": 3,
                            "Percent50": 842,
                            "Percent75": 1142,
                            "Percent95": 1419,
                            "Percent99": 1481,
                            "StdDev": 426,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1069178.83,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 155605.21,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 307,
                            "OkCount": 262,
                            "FailCount": 45,
                            "Min": 12,
                            "Mean": 729,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 714,
                            "Percent75": 1063,
                            "Percent95": 1390,
                            "Percent99": 1482,
                            "StdDev": 407,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1055758.67,
                            "MaxDataKb": 2051994.38,
                            "AllDataMB": 123549.76,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 45
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 49,
                        "More800Less1200": 35,
                        "More1200": 26
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 45
                        }
                    ],
                    "Duration": "00:01:05.0069616"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 48,
                    "OkCount": 48,
                    "FailCount": 0,
                    "AllDataMB": 21172.48000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 492,
                            "OkCount": 492,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 763,
                            "Max": 1512,
                            "RPS": 4,
                            "Percent50": 752,
                            "Percent75": 1116,
                            "Percent95": 1421,
                            "Percent99": 1489,
                            "StdDev": 417,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 939186.77,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 242367.45,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 24,
                        "More800Less1200": 12,
                        "More1200": 12
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:10.0090099"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 54,
                    "OkCount": 52,
                    "FailCount": 2,
                    "AllDataMB": 32968.01000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 313,
                            "OkCount": 313,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 758,
                            "Max": 1498,
                            "RPS": 3,
                            "Percent50": 786,
                            "Percent75": 1105,
                            "Percent95": 1420,
                            "Percent99": 1489,
                            "StdDev": 411,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 923256.27,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 150321.7,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 307,
                            "OkCount": 274,
                            "FailCount": 33,
                            "Min": 11,
                            "Mean": 763,
                            "Max": 1499,
                            "RPS": 2,
                            "Percent50": 770,
                            "Percent75": 1127,
                            "Percent95": 1414,
                            "Percent99": 1483,
                            "StdDev": 430,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1035216.79,
                            "MaxDataKb": 2052788.17,
                            "AllDataMB": 140976.17,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 33
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 26,
                        "More800Less1200": 13,
                        "More1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 33
                        }
                    ],
                    "Duration": "00:01:10.0090099"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 127,
                    "OkCount": 111,
                    "FailCount": 16,
                    "AllDataMB": 50573.41000000003,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 478,
                            "OkCount": 408,
                            "FailCount": 70,
                            "Min": 9,
                            "Mean": 755,
                            "Max": 1497,
                            "RPS": 3,
                            "Percent50": 766,
                            "Percent75": 1094,
                            "Percent95": 1436,
                            "Percent99": 1494,
                            "StdDev": 426,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 760295.02,
                            "MaxDataKb": 2090156.63,
                            "AllDataMB": 196041.79,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 401,
                            "OkCount": 349,
                            "FailCount": 52,
                            "Min": 9,
                            "Mean": 778,
                            "Max": 1500,
                            "RPS": 3,
                            "Percent50": 814,
                            "Percent75": 1152,
                            "Percent95": 1421,
                            "Percent99": 1486,
                            "StdDev": 434,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 935996.66,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 171407.23,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 343,
                            "OkCount": 291,
                            "FailCount": 52,
                            "Min": 12,
                            "Mean": 733,
                            "Max": 1498,
                            "RPS": 2,
                            "Percent50": 714,
                            "Percent75": 1076,
                            "Percent95": 1388,
                            "Percent99": 1482,
                            "StdDev": 405,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 841952.43,
                            "MaxDataKb": 2051994.38,
                            "AllDataMB": 136048.25,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 52
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 55,
                        "More800Less1200": 30,
                        "More1200": 27
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 52
                        }
                    ],
                    "Duration": "00:01:10.0090099"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 52,
                    "OkCount": 52,
                    "FailCount": 0,
                    "AllDataMB": 21823.830000000016,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 544,
                            "OkCount": 544,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 760,
                            "Max": 1512,
                            "RPS": 4,
                            "Percent50": 748,
                            "Percent75": 1119,
                            "Percent95": 1420,
                            "Percent99": 1489,
                            "StdDev": 418,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 942562.34,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 264191.28,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 32,
                        "More800Less1200": 9,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:15.0029475"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 46,
                    "OkCount": 44,
                    "FailCount": 2,
                    "AllDataMB": 26932.01000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 336,
                            "OkCount": 336,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 760,
                            "Max": 1498,
                            "RPS": 3,
                            "Percent50": 786,
                            "Percent75": 1109,
                            "Percent95": 1425,
                            "Percent99": 1489,
                            "StdDev": 417,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 951275.94,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 164428.64,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 330,
                            "OkCount": 295,
                            "FailCount": 35,
                            "Min": 11,
                            "Mean": 776,
                            "Max": 1510,
                            "RPS": 2,
                            "Percent50": 795,
                            "Percent75": 1140,
                            "Percent95": 1425,
                            "Percent99": 1486,
                            "StdDev": 429,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 999866.25,
                            "MaxDataKb": 2059468.5,
                            "AllDataMB": 153801.24,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 35
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 17,
                        "More800Less1200": 15,
                        "More1200": 12
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 35
                        }
                    ],
                    "Duration": "00:01:15.0029475"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 143,
                    "OkCount": 122,
                    "FailCount": 21,
                    "AllDataMB": 55750.189999999944,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 533,
                            "OkCount": 456,
                            "FailCount": 77,
                            "Min": 9,
                            "Mean": 757,
                            "Max": 1497,
                            "RPS": 4,
                            "Percent50": 766,
                            "Percent75": 1117,
                            "Percent95": 1436,
                            "Percent99": 1489,
                            "StdDev": 430,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 880058.37,
                            "MaxDataKb": 2090156.63,
                            "AllDataMB": 230091.84,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 6
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 448,
                            "OkCount": 389,
                            "FailCount": 59,
                            "Min": 9,
                            "Mean": 788,
                            "Max": 1500,
                            "RPS": 3,
                            "Percent50": 816,
                            "Percent75": 1176,
                            "Percent95": 1427,
                            "Percent99": 1481,
                            "StdDev": 437,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 970935.93,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 184473.09,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 383,
                            "OkCount": 324,
                            "FailCount": 59,
                            "Min": 12,
                            "Mean": 736,
                            "Max": 1498,
                            "RPS": 3,
                            "Percent50": 714,
                            "Percent75": 1082,
                            "Percent95": 1390,
                            "Percent99": 1482,
                            "StdDev": 405,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 930258.49,
                            "MaxDataKb": 2051994.38,
                            "AllDataMB": 144682.53,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 59
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 60,
                        "More800Less1200": 29,
                        "More1200": 31
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 59
                        }
                    ],
                    "Duration": "00:01:15.0029475"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 52,
                    "OkCount": 52,
                    "FailCount": 0,
                    "AllDataMB": 23647.899999999965,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 596,
                            "OkCount": 596,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 754,
                            "Max": 1512,
                            "RPS": 5,
                            "Percent50": 733,
                            "Percent75": 1116,
                            "Percent95": 1416,
                            "Percent99": 1486,
                            "StdDev": 415,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 939622.62,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 287839.18,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 29,
                        "More800Less1200": 20,
                        "More1200": 3
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:20.0029519"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 53,
                    "OkCount": 50,
                    "FailCount": 3,
                    "AllDataMB": 20421.909999999974,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 363,
                            "OkCount": 363,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 751,
                            "Max": 1498,
                            "RPS": 3,
                            "Percent50": 776,
                            "Percent75": 1109,
                            "Percent95": 1432,
                            "Percent99": 1489,
                            "StdDev": 420,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 942761.48,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 178741.01,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 356,
                            "OkCount": 318,
                            "FailCount": 38,
                            "Min": 11,
                            "Mean": 786,
                            "Max": 1510,
                            "RPS": 2,
                            "Percent50": 815,
                            "Percent75": 1162,
                            "Percent95": 1427,
                            "Percent99": 1489,
                            "StdDev": 429,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 998004.99,
                            "MaxDataKb": 2059468.5,
                            "AllDataMB": 159910.78,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 38
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 25,
                        "More800Less1200": 14,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 38
                        }
                    ],
                    "Duration": "00:01:20.0029519"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 134,
                    "OkCount": 116,
                    "FailCount": 18,
                    "AllDataMB": 69783.42000000004,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 583,
                            "OkCount": 500,
                            "FailCount": 83,
                            "Min": 9,
                            "Mean": 751,
                            "Max": 1497,
                            "RPS": 4,
                            "Percent50": 757,
                            "Percent75": 1122,
                            "Percent95": 1435,
                            "Percent99": 1489,
                            "StdDev": 433,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 908875.4,
                            "MaxDataKb": 2090156.63,
                            "AllDataMB": 253535.86,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 6
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 494,
                            "OkCount": 429,
                            "FailCount": 65,
                            "Min": 0,
                            "Mean": 780,
                            "Max": 1500,
                            "RPS": 3,
                            "Percent50": 813,
                            "Percent75": 1153,
                            "Percent95": 1430,
                            "Percent99": 1481,
                            "StdDev": 436,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1056896.85,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 209842.79,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 421,
                            "OkCount": 355,
                            "FailCount": 66,
                            "Min": 12,
                            "Mean": 732,
                            "Max": 1501,
                            "RPS": 3,
                            "Percent50": 709,
                            "Percent75": 1076,
                            "Percent95": 1396,
                            "Percent99": 1487,
                            "StdDev": 409,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1019183.2,
                            "MaxDataKb": 2091246.99,
                            "AllDataMB": 165652.23,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 66
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 69,
                        "More800Less1200": 24,
                        "More1200": 20
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 66
                        }
                    ],
                    "Duration": "00:01:20.0029519"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 51,
                    "OkCount": 51,
                    "FailCount": 0,
                    "AllDataMB": 29690.72999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 647,
                            "OkCount": 647,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 753,
                            "Max": 1512,
                            "RPS": 5,
                            "Percent50": 733,
                            "Percent75": 1119,
                            "Percent95": 1432,
                            "Percent99": 1486,
                            "StdDev": 419,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 940150.0,
                            "MaxDataKb": 2086620.39,
                            "AllDataMB": 317529.91,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 27,
                        "More800Less1200": 13,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:25.0089803"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 48,
                    "OkCount": 46,
                    "FailCount": 2,
                    "AllDataMB": 18351.400000000023,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 386,
                            "OkCount": 386,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 756,
                            "Max": 1498,
                            "RPS": 3,
                            "Percent50": 780,
                            "Percent75": 1110,
                            "Percent95": 1433,
                            "Percent99": 1489,
                            "StdDev": 423,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 939786.45,
                            "MaxDataKb": 2083314.51,
                            "AllDataMB": 190777.5,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 381,
                            "OkCount": 341,
                            "FailCount": 40,
                            "Min": 11,
                            "Mean": 785,
                            "Max": 1510,
                            "RPS": 2,
                            "Percent50": 815,
                            "Percent75": 1162,
                            "Percent95": 1434,
                            "Percent99": 1499,
                            "StdDev": 432,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 986880.97,
                            "MaxDataKb": 2059468.5,
                            "AllDataMB": 166225.69,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 40
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 22,
                        "More800Less1200": 11,
                        "More1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 40
                        }
                    ],
                    "Duration": "00:01:25.0089803"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 125,
                    "OkCount": 103,
                    "FailCount": 22,
                    "AllDataMB": 45380.689999999944,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 632,
                            "OkCount": 539,
                            "FailCount": 93,
                            "Min": 9,
                            "Mean": 750,
                            "Max": 1509,
                            "RPS": 4,
                            "Percent50": 757,
                            "Percent75": 1122,
                            "Percent95": 1435,
                            "Percent99": 1494,
                            "StdDev": 432,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 934144.21,
                            "MaxDataKb": 2090156.63,
                            "AllDataMB": 277200.19,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 6
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 532,
                            "OkCount": 463,
                            "FailCount": 69,
                            "Min": 0,
                            "Mean": 778,
                            "Max": 1501,
                            "RPS": 3,
                            "Percent50": 811,
                            "Percent75": 1153,
                            "Percent95": 1427,
                            "Percent99": 1481,
                            "StdDev": 435,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1040444.08,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 221584.52,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 458,
                            "OkCount": 385,
                            "FailCount": 73,
                            "Min": 12,
                            "Mean": 743,
                            "Max": 1501,
                            "RPS": 3,
                            "Percent50": 724,
                            "Percent75": 1092,
                            "Percent95": 1406,
                            "Percent99": 1487,
                            "StdDev": 409,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1020086.28,
                            "MaxDataKb": 2091246.99,
                            "AllDataMB": 175626.86,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 73
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 49,
                        "More800Less1200": 33,
                        "More1200": 21
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 73
                        }
                    ],
                    "Duration": "00:01:25.0089803"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 48,
                    "OkCount": 48,
                    "FailCount": 0,
                    "AllDataMB": 25943.18000000005,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 695,
                            "OkCount": 695,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 758,
                            "Max": 1512,
                            "RPS": 5,
                            "Percent50": 748,
                            "Percent75": 1121,
                            "Percent95": 1432,
                            "Percent99": 1486,
                            "StdDev": 420,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 943883.1,
                            "MaxDataKb": 2090797.29,
                            "AllDataMB": 343473.09,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 23,
                        "More800Less1200": 15,
                        "More1200": 10
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:30"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 52,
                    "OkCount": 50,
                    "FailCount": 2,
                    "AllDataMB": 23940.570000000007,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 412,
                            "OkCount": 412,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 751,
                            "Max": 1501,
                            "RPS": 3,
                            "Percent50": 776,
                            "Percent75": 1109,
                            "Percent95": 1433,
                            "Percent99": 1494,
                            "StdDev": 426,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 931128.48,
                            "MaxDataKb": 2086343.09,
                            "AllDataMB": 207586.17,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 408,
                            "OkCount": 366,
                            "FailCount": 42,
                            "Min": 11,
                            "Mean": 785,
                            "Max": 1510,
                            "RPS": 2,
                            "Percent50": 815,
                            "Percent75": 1168,
                            "Percent95": 1434,
                            "Percent99": 1489,
                            "StdDev": 433,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 969561.94,
                            "MaxDataKb": 2059468.5,
                            "AllDataMB": 173357.59,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 42
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 28,
                        "More800Less1200": 11,
                        "More1200": 12
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 42
                        }
                    ],
                    "Duration": "00:01:30"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 126,
                    "OkCount": 109,
                    "FailCount": 17,
                    "AllDataMB": 57564.90000000002,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 681,
                            "OkCount": 580,
                            "FailCount": 101,
                            "Min": 9,
                            "Mean": 748,
                            "Max": 1509,
                            "RPS": 4,
                            "Percent50": 755,
                            "Percent75": 1109,
                            "Percent95": 1436,
                            "Percent99": 1494,
                            "StdDev": 434,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 939381.09,
                            "MaxDataKb": 2090156.63,
                            "AllDataMB": 294617.34,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 6
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 575,
                            "OkCount": 503,
                            "FailCount": 72,
                            "Min": 0,
                            "Mean": 774,
                            "Max": 1501,
                            "RPS": 3,
                            "Percent50": 801,
                            "Percent75": 1153,
                            "Percent95": 1427,
                            "Percent99": 1486,
                            "StdDev": 438,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1053206.1,
                            "MaxDataKb": 2089670.0,
                            "AllDataMB": 247426.05,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 495,
                            "OkCount": 416,
                            "FailCount": 79,
                            "Min": 12,
                            "Mean": 740,
                            "Max": 1501,
                            "RPS": 3,
                            "Percent50": 724,
                            "Percent75": 1092,
                            "Percent95": 1403,
                            "Percent99": 1487,
                            "StdDev": 411,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1013379.19,
                            "MaxDataKb": 2091246.99,
                            "AllDataMB": 189933.08,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 79
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 60,
                        "More800Less1200": 33,
                        "More1200": 17
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 79
                        }
                    ],
                    "Duration": "00:01:30"
                }
            ]
        ]
    } };
