const viewModel = { nBomberInfo: {
        "NBomberVersion": "1.1.0"
    }, testInfo: {
        "TestSuite": "nbomber_default_test_suite_name",
        "TestName": "nbomber_default_test_name"
    }, statsData: {
        "RequestCount": 3226,
        "OkCount": 2910,
        "FailCount": 316,
        "AllDataMB": 1511111.67,
        "ScenarioStats": [
            {
                "ScenarioName": "scenario 1",
                "RequestCount": 695,
                "OkCount": 695,
                "FailCount": 0,
                "AllDataMB": 358894.19,
                "StepStats": [
                    {
                        "StepName": "pull html 1",
                        "RequestCount": 695,
                        "OkCount": 695,
                        "FailCount": 0,
                        "Min": 5,
                        "Mean": 774,
                        "Max": 1510,
                        "RPS": 4,
                        "Percent50": 796,
                        "Percent75": 1141,
                        "Percent95": 1433,
                        "Percent99": 1485,
                        "StdDev": 434,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 882167.09,
                        "MaxDataKb": 2094145.19,
                        "AllDataMB": 358894.19,
                        "ErrorStats": []
                    }
                ],
                "LatencyCount": {
                    "Less800": 353,
                    "More800Less1200": 195,
                    "More1200": 146
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
                "RequestCount": 799,
                "OkCount": 735,
                "FailCount": 64,
                "AllDataMB": 372455.87,
                "StepStats": [
                    {
                        "StepName": "pull html 1",
                        "RequestCount": 400,
                        "OkCount": 400,
                        "FailCount": 0,
                        "Min": 0,
                        "Mean": 783,
                        "Max": 1513,
                        "RPS": 2,
                        "Percent50": 767,
                        "Percent75": 1156,
                        "Percent95": 1445,
                        "Percent99": 1499,
                        "StdDev": 432,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 1011036.71,
                        "MaxDataKb": 2095457.07,
                        "AllDataMB": 216943.61,
                        "ErrorStats": []
                    },
                    {
                        "StepName": "pull html 2",
                        "RequestCount": 399,
                        "OkCount": 335,
                        "FailCount": 64,
                        "Min": 0,
                        "Mean": 724,
                        "Max": 1500,
                        "RPS": 2,
                        "Percent50": 700,
                        "Percent75": 1123,
                        "Percent95": 1417,
                        "Percent99": 1475,
                        "StdDev": 446,
                        "MinDataKb": 3991.62,
                        "MeanDataKb": 967320.03,
                        "MaxDataKb": 2091092.82,
                        "AllDataMB": 155512.26,
                        "ErrorStats": [
                            {
                                "ErrorCode": 0,
                                "Message": "System.Exception: unknown client's error",
                                "Count": 64
                            }
                        ]
                    }
                ],
                "LatencyCount": {
                    "Less800": 395,
                    "More800Less1200": 177,
                    "More1200": 161
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 5
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 0,
                        "Message": "System.Exception: unknown client's error",
                        "Count": 64
                    }
                ],
                "Duration": "00:01:30"
            },
            {
                "ScenarioName": "scenario 3",
                "RequestCount": 1732,
                "OkCount": 1480,
                "FailCount": 252,
                "AllDataMB": 779761.61,
                "StepStats": [
                    {
                        "StepName": "pull html 3",
                        "RequestCount": 679,
                        "OkCount": 582,
                        "FailCount": 97,
                        "Min": 13,
                        "Mean": 753,
                        "Max": 1513,
                        "RPS": 4,
                        "Percent50": 728,
                        "Percent75": 1122,
                        "Percent95": 1434,
                        "Percent99": 1484,
                        "StdDev": 431,
                        "MinDataKb": 799.62,
                        "MeanDataKb": 1103604.68,
                        "MaxDataKb": 2094047.18,
                        "AllDataMB": 309319.21,
                        "ErrorStats": [
                            {
                                "ErrorCode": 410,
                                "Message": "System.Exception: Gone",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 416,
                                "Message": "System.Exception: Requested Range Not Satisfiable",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 426,
                                "Message": "System.Exception: Upgrade Required",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 402,
                                "Message": "System.Exception: Payment Required",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 451,
                                "Message": "System.Exception: Unavailable For Legal Reasons",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 411,
                                "Message": "System.Exception: Length Required",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 418,
                                "Message": "System.Exception: I'm a teapot",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 501,
                                "Message": "System.Exception: Not Implemented",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 404,
                                "Message": "System.Exception: Not Found",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 413,
                                "Message": "System.Exception: Payload Too Large",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 423,
                                "Message": "System.Exception: Locked (WebDAV)",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 412,
                                "Message": "System.Exception: Precondition Failed",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 422,
                                "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 401,
                                "Message": "System.Exception: Unauthorized",
                                "Count": 5
                            },
                            {
                                "ErrorCode": 431,
                                "Message": "System.Exception: Request Header Fields Too Large",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 408,
                                "Message": "System.Exception: Request Timeout",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 500,
                                "Message": "System.Exception: Internal Server Error",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 504,
                                "Message": "System.Exception: Gateway Timeout",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 510,
                                "Message": "System.Exception: Not Extended",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 414,
                                "Message": "System.Exception: URI Too Long",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 511,
                                "Message": "System.Exception: Network Authentication Required",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 508,
                                "Message": "System.Exception: Loop Detected (WebDAV)",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 405,
                                "Message": "System.Exception: Method Not Allowed",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 506,
                                "Message": "System.Exception: Variant Also Negotiates",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 417,
                                "Message": "System.Exception: Expectation Failed",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 407,
                                "Message": "System.Exception: Proxy Authentication Required",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 503,
                                "Message": "System.Exception: Service Unavailable",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 507,
                                "Message": "System.Exception: Insufficient Storage",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 429,
                                "Message": "System.Exception: Too Many Requests",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 403,
                                "Message": "System.Exception: Forbidden",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 400,
                                "Message": "System.Exception: Bad Request",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 502,
                                "Message": "System.Exception: Bad Gateway",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 424,
                                "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                                "Count": 2
                            }
                        ]
                    },
                    {
                        "StepName": "pull html 4",
                        "RequestCount": 576,
                        "OkCount": 483,
                        "FailCount": 93,
                        "Min": 11,
                        "Mean": 710,
                        "Max": 1509,
                        "RPS": 3,
                        "Percent50": 707,
                        "Percent75": 1047,
                        "Percent95": 1367,
                        "Percent99": 1488,
                        "StdDev": 405,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 966511.73,
                        "MaxDataKb": 2094941.13,
                        "AllDataMB": 265330.51,
                        "ErrorStats": [
                            {
                                "ErrorCode": 401,
                                "Message": "System.Exception: Unauthorized",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 504,
                                "Message": "System.Exception: Gateway Timeout",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 416,
                                "Message": "System.Exception: Requested Range Not Satisfiable",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 411,
                                "Message": "System.Exception: Length Required",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 406,
                                "Message": "System.Exception: Not Acceptable",
                                "Count": 6
                            },
                            {
                                "ErrorCode": 503,
                                "Message": "System.Exception: Service Unavailable",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 506,
                                "Message": "System.Exception: Variant Also Negotiates",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 402,
                                "Message": "System.Exception: Payment Required",
                                "Count": 6
                            },
                            {
                                "ErrorCode": 501,
                                "Message": "System.Exception: Not Implemented",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 413,
                                "Message": "System.Exception: Payload Too Large",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 410,
                                "Message": "System.Exception: Gone",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 409,
                                "Message": "System.Exception: Conflict",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 415,
                                "Message": "System.Exception: Unsupported Media Type",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 408,
                                "Message": "System.Exception: Request Timeout",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 404,
                                "Message": "System.Exception: Not Found",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 510,
                                "Message": "System.Exception: Not Extended",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 423,
                                "Message": "System.Exception: Locked (WebDAV)",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 507,
                                "Message": "System.Exception: Insufficient Storage",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 400,
                                "Message": "System.Exception: Bad Request",
                                "Count": 4
                            },
                            {
                                "ErrorCode": 417,
                                "Message": "System.Exception: Expectation Failed",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 405,
                                "Message": "System.Exception: Method Not Allowed",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 505,
                                "Message": "System.Exception: HTTP Version Not Supported",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 403,
                                "Message": "System.Exception: Forbidden",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 412,
                                "Message": "System.Exception: Precondition Failed",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 418,
                                "Message": "System.Exception: I'm a teapot",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 422,
                                "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                "Count": 4
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
                                "ErrorCode": 502,
                                "Message": "System.Exception: Bad Gateway",
                                "Count": 1
                            },
                            {
                                "ErrorCode": 451,
                                "Message": "System.Exception: Unavailable For Legal Reasons",
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
                                "Count": 4
                            },
                            {
                                "ErrorCode": 429,
                                "Message": "System.Exception: Too Many Requests",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 424,
                                "Message": "System.Exception: Failed Dependency (WebDAV)",
                                "Count": 2
                            },
                            {
                                "ErrorCode": 508,
                                "Message": "System.Exception: Loop Detected (WebDAV)",
                                "Count": 3
                            },
                            {
                                "ErrorCode": 421,
                                "Message": "System.Exception: Misdirected Request",
                                "Count": 1
                            }
                        ]
                    },
                    {
                        "StepName": "pull html 5",
                        "RequestCount": 477,
                        "OkCount": 415,
                        "FailCount": 62,
                        "Min": 13,
                        "Mean": 750,
                        "Max": 1509,
                        "RPS": 3,
                        "Percent50": 755,
                        "Percent75": 1090,
                        "Percent95": 1392,
                        "Percent99": 1490,
                        "StdDev": 413,
                        "MinDataKb": 0.0,
                        "MeanDataKb": 984694.67,
                        "MaxDataKb": 2093625.12,
                        "AllDataMB": 205111.89,
                        "ErrorStats": [
                            {
                                "ErrorCode": 0,
                                "Message": "System.Exception: unknown client's error",
                                "Count": 62
                            }
                        ]
                    }
                ],
                "LatencyCount": {
                    "Less800": 822,
                    "More800Less1200": 391,
                    "More1200": 263
                },
                "LoadSimulationStats": {
                    "SimulationName": "inject_per_sec",
                    "Value": 10
                },
                "ErrorStats": [
                    {
                        "ErrorCode": 410,
                        "Message": "System.Exception: Gone",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 416,
                        "Message": "System.Exception: Requested Range Not Satisfiable",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 426,
                        "Message": "System.Exception: Upgrade Required",
                        "Count": 9
                    },
                    {
                        "ErrorCode": 402,
                        "Message": "System.Exception: Payment Required",
                        "Count": 10
                    },
                    {
                        "ErrorCode": 451,
                        "Message": "System.Exception: Unavailable For Legal Reasons",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 411,
                        "Message": "System.Exception: Length Required",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 418,
                        "Message": "System.Exception: I'm a teapot",
                        "Count": 7
                    },
                    {
                        "ErrorCode": 501,
                        "Message": "System.Exception: Not Implemented",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 404,
                        "Message": "System.Exception: Not Found",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 413,
                        "Message": "System.Exception: Payload Too Large",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 423,
                        "Message": "System.Exception: Locked (WebDAV)",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 412,
                        "Message": "System.Exception: Precondition Failed",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 422,
                        "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                        "Count": 9
                    },
                    {
                        "ErrorCode": 401,
                        "Message": "System.Exception: Unauthorized",
                        "Count": 8
                    },
                    {
                        "ErrorCode": 431,
                        "Message": "System.Exception: Request Header Fields Too Large",
                        "Count": 2
                    },
                    {
                        "ErrorCode": 408,
                        "Message": "System.Exception: Request Timeout",
                        "Count": 8
                    },
                    {
                        "ErrorCode": 500,
                        "Message": "System.Exception: Internal Server Error",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 504,
                        "Message": "System.Exception: Gateway Timeout",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 510,
                        "Message": "System.Exception: Not Extended",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 414,
                        "Message": "System.Exception: URI Too Long",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 511,
                        "Message": "System.Exception: Network Authentication Required",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 508,
                        "Message": "System.Exception: Loop Detected (WebDAV)",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 405,
                        "Message": "System.Exception: Method Not Allowed",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 506,
                        "Message": "System.Exception: Variant Also Negotiates",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 417,
                        "Message": "System.Exception: Expectation Failed",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 407,
                        "Message": "System.Exception: Proxy Authentication Required",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 503,
                        "Message": "System.Exception: Service Unavailable",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 507,
                        "Message": "System.Exception: Insufficient Storage",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 429,
                        "Message": "System.Exception: Too Many Requests",
                        "Count": 5
                    },
                    {
                        "ErrorCode": 403,
                        "Message": "System.Exception: Forbidden",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 400,
                        "Message": "System.Exception: Bad Request",
                        "Count": 7
                    },
                    {
                        "ErrorCode": 502,
                        "Message": "System.Exception: Bad Gateway",
                        "Count": 2
                    },
                    {
                        "ErrorCode": 424,
                        "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                        "Count": 3
                    },
                    {
                        "ErrorCode": 406,
                        "Message": "System.Exception: Not Acceptable",
                        "Count": 6
                    },
                    {
                        "ErrorCode": 409,
                        "Message": "System.Exception: Conflict",
                        "Count": 4
                    },
                    {
                        "ErrorCode": 415,
                        "Message": "System.Exception: Unsupported Media Type",
                        "Count": 3
                    },
                    {
                        "ErrorCode": 505,
                        "Message": "System.Exception: HTTP Version Not Supported",
                        "Count": 2
                    },
                    {
                        "ErrorCode": 0,
                        "Message": "System.Exception: unknown client's error",
                        "Count": 62
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
                        "885521363"
                    ],
                    [
                        "Property2",
                        "88987456"
                    ],
                    [
                        "Property3",
                        "5243608"
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
                        "1158019032"
                    ],
                    [
                        "Property 2",
                        "1569424029"
                    ],
                    [
                        "Property 3",
                        "2023438098"
                    ]
                ]
            }
        ],
        "CustomPluginData": [
            {
                "PluginName": "CustomHtmlPlugin",
                "Title": "Custom HTML",
                "ViewModel": { message: "Hello, World" }
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
            "00:01:24",
            "00:01:30"
        ],
        "ScenarioStats": [
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 4,
                    "OkCount": 4,
                    "FailCount": 0,
                    "AllDataMB": 2770.67,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 4,
                            "OkCount": 4,
                            "FailCount": 0,
                            "Min": 217,
                            "Mean": 489,
                            "Max": 1037,
                            "RPS": 0,
                            "Percent50": 323,
                            "Percent75": 380,
                            "Percent95": 1037,
                            "Percent99": 1037,
                            "StdDev": 322,
                            "MinDataKb": 1214269.93,
                            "MeanDataKb": 1418583.26,
                            "MaxDataKb": 1622896.6,
                            "AllDataMB": 2770.67,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 3,
                        "More800Less1200": 1,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 1
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:05.0145028"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 3,
                    "OkCount": 3,
                    "FailCount": 0,
                    "AllDataMB": 2139.19,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 2,
                            "OkCount": 2,
                            "FailCount": 0,
                            "Min": 1007,
                            "Mean": 1190,
                            "Max": 1373,
                            "RPS": 0,
                            "Percent50": 1007,
                            "Percent75": 1373,
                            "Percent95": 1373,
                            "Percent99": 1373,
                            "StdDev": 183,
                            "MinDataKb": 1395758.57,
                            "MeanDataKb": 1395758.57,
                            "MaxDataKb": 1395758.57,
                            "AllDataMB": 1363.05,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 1,
                            "OkCount": 1,
                            "FailCount": 0,
                            "Min": 995,
                            "Mean": 995,
                            "Max": 995,
                            "RPS": 0,
                            "Percent50": 995,
                            "Percent75": 995,
                            "Percent95": 995,
                            "Percent99": 995,
                            "StdDev": 0,
                            "MinDataKb": 794762.77,
                            "MeanDataKb": 794762.77,
                            "MaxDataKb": 794762.77,
                            "AllDataMB": 776.14,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 0,
                        "More800Less1200": 2,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:05.0145028"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 6,
                    "OkCount": 6,
                    "FailCount": 0,
                    "AllDataMB": 3731.27,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 2,
                            "OkCount": 2,
                            "FailCount": 0,
                            "Min": 314,
                            "Mean": 843,
                            "Max": 1373,
                            "RPS": 0,
                            "Percent50": 314,
                            "Percent75": 1373,
                            "Percent95": 1373,
                            "Percent99": 1373,
                            "StdDev": 530,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 621789.84,
                            "MaxDataKb": 1243579.68,
                            "AllDataMB": 1214.43,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 2,
                            "OkCount": 2,
                            "FailCount": 0,
                            "Min": 560,
                            "Mean": 624,
                            "Max": 689,
                            "RPS": 0,
                            "Percent50": 560,
                            "Percent75": 689,
                            "Percent95": 689,
                            "Percent99": 689,
                            "StdDev": 64,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 440706.85,
                            "MaxDataKb": 1345531.43,
                            "AllDataMB": 1721.51,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 2,
                            "OkCount": 2,
                            "FailCount": 0,
                            "Min": 383,
                            "Mean": 499,
                            "Max": 616,
                            "RPS": 0,
                            "Percent50": 383,
                            "Percent75": 616,
                            "Percent95": 616,
                            "Percent99": 616,
                            "StdDev": 116,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 203604.92,
                            "MaxDataKb": 695787.46,
                            "AllDataMB": 795.33,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 5,
                        "More800Less1200": 0,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:05.0145028"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 6,
                    "OkCount": 6,
                    "FailCount": 0,
                    "AllDataMB": 1243.6,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 10,
                            "OkCount": 10,
                            "FailCount": 0,
                            "Min": 217,
                            "Mean": 681,
                            "Max": 1411,
                            "RPS": 0,
                            "Percent50": 404,
                            "Percent75": 1037,
                            "Percent95": 1411,
                            "Percent99": 1411,
                            "StdDev": 399,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 513826.76,
                            "MaxDataKb": 1622896.6,
                            "AllDataMB": 4014.27,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 2,
                        "More800Less1200": 3,
                        "More1200": 1
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 2
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:10.0067495"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 13,
                    "OkCount": 9,
                    "FailCount": 4,
                    "AllDataMB": 6505.6,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 9,
                            "OkCount": 9,
                            "FailCount": 0,
                            "Min": 390,
                            "Mean": 945,
                            "Max": 1373,
                            "RPS": 0,
                            "Percent50": 1007,
                            "Percent75": 1049,
                            "Percent95": 1373,
                            "Percent99": 1373,
                            "StdDev": 255,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 651801.62,
                            "MaxDataKb": 1888754.21,
                            "AllDataMB": 6301.91,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 7,
                            "OkCount": 3,
                            "FailCount": 4,
                            "Min": 109,
                            "Mean": 747,
                            "Max": 1137,
                            "RPS": 0,
                            "Percent50": 995,
                            "Percent75": 995,
                            "Percent95": 1137,
                            "Percent99": 1137,
                            "StdDev": 455,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 599778.16,
                            "MaxDataKb": 1604349.87,
                            "AllDataMB": 2342.88,
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
                        "Less800": 3,
                        "More800Less1200": 6,
                        "More1200": 0
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 4
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 4
                        }
                    ],
                    "Duration": "00:00:10.0067495"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 14,
                    "OkCount": 12,
                    "FailCount": 2,
                    "AllDataMB": 6513.4,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 8,
                            "OkCount": 8,
                            "FailCount": 0,
                            "Min": 23,
                            "Mean": 746,
                            "Max": 1405,
                            "RPS": 0,
                            "Percent50": 615,
                            "Percent75": 1092,
                            "Percent95": 1405,
                            "Percent99": 1405,
                            "StdDev": 472,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1012400.2,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 5926.81,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 6,
                            "OkCount": 6,
                            "FailCount": 0,
                            "Min": 531,
                            "Mean": 811,
                            "Max": 1509,
                            "RPS": 0,
                            "Percent50": 689,
                            "Percent75": 793,
                            "Percent95": 1509,
                            "Percent99": 1509,
                            "StdDev": 328,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 400785.23,
                            "MaxDataKb": 1844239.68,
                            "AllDataMB": 3522.53,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 6,
                            "OkCount": 4,
                            "FailCount": 2,
                            "Min": 383,
                            "Mean": 562,
                            "Max": 796,
                            "RPS": 0,
                            "Percent50": 455,
                            "Percent75": 616,
                            "Percent95": 796,
                            "Percent99": 796,
                            "StdDev": 159,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 135736.61,
                            "MaxDataKb": 695787.46,
                            "AllDataMB": 795.33,
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
                        "Less800": 9,
                        "More800Less1200": 1,
                        "More1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 4
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 2
                        }
                    ],
                    "Duration": "00:00:10.0067495"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 13,
                    "OkCount": 13,
                    "FailCount": 0,
                    "AllDataMB": 4202.51,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 23,
                            "OkCount": 23,
                            "FailCount": 0,
                            "Min": 48,
                            "Mean": 700,
                            "Max": 1498,
                            "RPS": 1,
                            "Percent50": 578,
                            "Percent75": 1019,
                            "Percent95": 1411,
                            "Percent99": 1498,
                            "StdDev": 412,
                            "MinDataKb": 15328.59,
                            "MeanDataKb": 927214.79,
                            "MaxDataKb": 1700492.98,
                            "AllDataMB": 8216.78,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 8,
                        "More800Less1200": 3,
                        "More1200": 2
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:15.0123497"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 28,
                    "OkCount": 25,
                    "FailCount": 3,
                    "AllDataMB": 10201.809999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 23,
                            "OkCount": 23,
                            "FailCount": 0,
                            "Min": 76,
                            "Mean": 913,
                            "Max": 1495,
                            "RPS": 0,
                            "Percent50": 982,
                            "Percent75": 1091,
                            "Percent95": 1485,
                            "Percent99": 1495,
                            "StdDev": 399,
                            "MinDataKb": 73232.13,
                            "MeanDataKb": 819375.35,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 11663.17,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 21,
                            "OkCount": 14,
                            "FailCount": 7,
                            "Min": 45,
                            "Mean": 669,
                            "Max": 1267,
                            "RPS": 0,
                            "Percent50": 649,
                            "Percent75": 995,
                            "Percent95": 1187,
                            "Percent99": 1267,
                            "StdDev": 381,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 846422.66,
                            "MaxDataKb": 1744030.59,
                            "AllDataMB": 7183.43,
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
                        "Less800": 11,
                        "More800Less1200": 9,
                        "More1200": 5
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 7
                        }
                    ],
                    "Duration": "00:00:15.0123497"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 24,
                    "OkCount": 18,
                    "FailCount": 6,
                    "AllDataMB": 12187.74,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 19,
                            "OkCount": 17,
                            "FailCount": 2,
                            "Min": 23,
                            "Mean": 956,
                            "Max": 1486,
                            "RPS": 0,
                            "Percent50": 1092,
                            "Percent75": 1392,
                            "Percent95": 1417,
                            "Percent99": 1486,
                            "StdDev": 451,
                            "MinDataKb": 285746.29,
                            "MeanDataKb": 983967.07,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 11296.07,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 14,
                            "OkCount": 12,
                            "FailCount": 2,
                            "Min": 31,
                            "Mean": 650,
                            "Max": 1509,
                            "RPS": 0,
                            "Percent50": 560,
                            "Percent75": 793,
                            "Percent95": 1101,
                            "Percent99": 1509,
                            "StdDev": 384,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 796351.41,
                            "MaxDataKb": 1844239.68,
                            "AllDataMB": 7273.1,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 11,
                            "OkCount": 7,
                            "FailCount": 4,
                            "Min": 275,
                            "Mean": 583,
                            "Max": 1016,
                            "RPS": 0,
                            "Percent50": 545,
                            "Percent75": 616,
                            "Percent95": 1016,
                            "Percent99": 1016,
                            "StdDev": 235,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 436782.49,
                            "MaxDataKb": 1843648.88,
                            "AllDataMB": 3863.24,
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
                        "Less800": 8,
                        "More800Less1200": 4,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 4
                        }
                    ],
                    "Duration": "00:00:15.0123497"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 17,
                    "OkCount": 17,
                    "FailCount": 0,
                    "AllDataMB": 6635.639999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 40,
                            "OkCount": 40,
                            "FailCount": 0,
                            "Min": 48,
                            "Mean": 767,
                            "Max": 1498,
                            "RPS": 1,
                            "Percent50": 768,
                            "Percent75": 1118,
                            "Percent95": 1440,
                            "Percent99": 1498,
                            "StdDev": 441,
                            "MinDataKb": 15328.59,
                            "MeanDataKb": 1045209.78,
                            "MaxDataKb": 1700492.98,
                            "AllDataMB": 14852.42,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 8,
                        "More800Less1200": 3,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 3
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:20.0102224"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 34,
                    "OkCount": 32,
                    "FailCount": 2,
                    "AllDataMB": 16720.230000000003,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 41,
                            "OkCount": 41,
                            "FailCount": 0,
                            "Min": 53,
                            "Mean": 862,
                            "Max": 1513,
                            "RPS": 1,
                            "Percent50": 921,
                            "Percent75": 1112,
                            "Percent95": 1485,
                            "Percent99": 1513,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 948978.27,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 22862.13,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 38,
                            "OkCount": 29,
                            "FailCount": 9,
                            "Min": 45,
                            "Mean": 750,
                            "Max": 1397,
                            "RPS": 0,
                            "Percent50": 687,
                            "Percent75": 1139,
                            "Percent95": 1397,
                            "Percent99": 1397,
                            "StdDev": 426,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 578585.94,
                            "MaxDataKb": 2062840.04,
                            "AllDataMB": 12704.7,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 9
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 17,
                        "More800Less1200": 7,
                        "More1200": 8
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 9
                        }
                    ],
                    "Duration": "00:00:20.0102224"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 39,
                    "OkCount": 35,
                    "FailCount": 4,
                    "AllDataMB": 13404.119999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 32,
                            "OkCount": 30,
                            "FailCount": 2,
                            "Min": 15,
                            "Mean": 849,
                            "Max": 1486,
                            "RPS": 0,
                            "Percent50": 973,
                            "Percent75": 1314,
                            "Percent95": 1417,
                            "Percent99": 1486,
                            "StdDev": 487,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 731333.77,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 16925.51,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 28,
                            "OkCount": 23,
                            "FailCount": 5,
                            "Min": 25,
                            "Mean": 685,
                            "Max": 1509,
                            "RPS": 0,
                            "Percent50": 689,
                            "Percent75": 858,
                            "Percent95": 1299,
                            "Percent99": 1509,
                            "StdDev": 374,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 726991.22,
                            "MaxDataKb": 1844239.68,
                            "AllDataMB": 9367.17,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
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
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 23,
                            "OkCount": 18,
                            "FailCount": 5,
                            "Min": 275,
                            "Mean": 668,
                            "Max": 1488,
                            "RPS": 0,
                            "Percent50": 578,
                            "Percent75": 885,
                            "Percent95": 1264,
                            "Percent99": 1488,
                            "StdDev": 336,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 460128.86,
                            "MaxDataKb": 1880566.96,
                            "AllDataMB": 9543.85,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 5
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 19,
                        "More800Less1200": 10,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 7
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 5
                        }
                    ],
                    "Duration": "00:00:20.0102224"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 22,
                    "OkCount": 22,
                    "FailCount": 0,
                    "AllDataMB": 8687.769999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 62,
                            "OkCount": 62,
                            "FailCount": 0,
                            "Min": 48,
                            "Mean": 796,
                            "Max": 1499,
                            "RPS": 1,
                            "Percent50": 829,
                            "Percent75": 1168,
                            "Percent95": 1444,
                            "Percent99": 1498,
                            "StdDev": 440,
                            "MinDataKb": 15328.59,
                            "MeanDataKb": 840437.4,
                            "MaxDataKb": 1748534.39,
                            "AllDataMB": 23540.19,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 9,
                        "More800Less1200": 8,
                        "More1200": 5
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 4
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:25.0119649"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 43,
                    "OkCount": 41,
                    "FailCount": 2,
                    "AllDataMB": 20836.059999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 63,
                            "OkCount": 63,
                            "FailCount": 0,
                            "Min": 36,
                            "Mean": 865,
                            "Max": 1513,
                            "RPS": 1,
                            "Percent50": 923,
                            "Percent75": 1156,
                            "Percent95": 1485,
                            "Percent99": 1499,
                            "StdDev": 435,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 824597.19,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 33878.14,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 58,
                            "OkCount": 47,
                            "FailCount": 11,
                            "Min": 37,
                            "Mean": 824,
                            "Max": 1423,
                            "RPS": 1,
                            "Percent50": 847,
                            "Percent75": 1187,
                            "Percent95": 1397,
                            "Percent99": 1423,
                            "StdDev": 446,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 895549.16,
                            "MaxDataKb": 2062840.04,
                            "AllDataMB": 22524.75,
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
                        "Less800": 18,
                        "More800Less1200": 10,
                        "More1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 9
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 11
                        }
                    ],
                    "Duration": "00:00:25.0119649"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 51,
                    "OkCount": 45,
                    "FailCount": 6,
                    "AllDataMB": 21594.020000000004,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 53,
                            "OkCount": 50,
                            "FailCount": 3,
                            "Min": 15,
                            "Mean": 812,
                            "Max": 1486,
                            "RPS": 1,
                            "Percent50": 728,
                            "Percent75": 1293,
                            "Percent95": 1417,
                            "Percent99": 1486,
                            "StdDev": 455,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 984691.69,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 26141.52,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 45,
                            "OkCount": 37,
                            "FailCount": 8,
                            "Min": 25,
                            "Mean": 679,
                            "Max": 1509,
                            "RPS": 1,
                            "Percent50": 628,
                            "Percent75": 1077,
                            "Percent95": 1259,
                            "Percent99": 1509,
                            "StdDev": 411,
                            "MinDataKb": 115301.7,
                            "MeanDataKb": 1065613.53,
                            "MaxDataKb": 1844239.68,
                            "AllDataMB": 16628.67,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
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
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 36,
                            "OkCount": 29,
                            "FailCount": 7,
                            "Min": 201,
                            "Mean": 697,
                            "Max": 1488,
                            "RPS": 0,
                            "Percent50": 603,
                            "Percent75": 1000,
                            "Percent95": 1435,
                            "Percent99": 1488,
                            "StdDev": 383,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 825982.99,
                            "MaxDataKb": 1880566.96,
                            "AllDataMB": 14660.36,
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
                        "Less800": 27,
                        "More800Less1200": 9,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 9
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 7
                        }
                    ],
                    "Duration": "00:00:25.0119649"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 29,
                    "OkCount": 29,
                    "FailCount": 0,
                    "AllDataMB": 17696.470000000005,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 91,
                            "OkCount": 91,
                            "FailCount": 0,
                            "Min": 19,
                            "Mean": 779,
                            "Max": 1499,
                            "RPS": 1,
                            "Percent50": 870,
                            "Percent75": 1168,
                            "Percent95": 1411,
                            "Percent99": 1498,
                            "StdDev": 455,
                            "MinDataKb": 15328.59,
                            "MeanDataKb": 854138.18,
                            "MaxDataKb": 2010774.66,
                            "AllDataMB": 41236.66,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 13,
                        "More800Less1200": 9,
                        "More1200": 7
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 5
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:30.0128014"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 53,
                    "OkCount": 47,
                    "FailCount": 6,
                    "AllDataMB": 27187.86,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 89,
                            "OkCount": 89,
                            "FailCount": 0,
                            "Min": 36,
                            "Mean": 873,
                            "Max": 1513,
                            "RPS": 1,
                            "Percent50": 923,
                            "Percent75": 1218,
                            "Percent95": 1485,
                            "Percent99": 1499,
                            "StdDev": 425,
                            "MinDataKb": 73232.13,
                            "MeanDataKb": 1182977.02,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 49126.25,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 85,
                            "OkCount": 68,
                            "FailCount": 17,
                            "Min": 16,
                            "Mean": 842,
                            "Max": 1423,
                            "RPS": 1,
                            "Percent50": 932,
                            "Percent75": 1266,
                            "Percent95": 1397,
                            "Percent99": 1422,
                            "StdDev": 448,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 788467.52,
                            "MaxDataKb": 2062840.04,
                            "AllDataMB": 34464.5,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 17
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 19,
                        "More800Less1200": 12,
                        "More1200": 15
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 17
                        }
                    ],
                    "Duration": "00:00:30.0128014"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 64,
                    "OkCount": 53,
                    "FailCount": 11,
                    "AllDataMB": 33236.31,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 79,
                            "OkCount": 73,
                            "FailCount": 6,
                            "Min": 15,
                            "Mean": 776,
                            "Max": 1486,
                            "RPS": 1,
                            "Percent50": 728,
                            "Percent75": 1214,
                            "Percent95": 1407,
                            "Percent99": 1449,
                            "StdDev": 453,
                            "MinDataKb": 83377.5,
                            "MeanDataKb": 1112937.08,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 40960.32,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
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
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 67,
                            "OkCount": 52,
                            "FailCount": 15,
                            "Min": 25,
                            "Mean": 678,
                            "Max": 1509,
                            "RPS": 1,
                            "Percent50": 628,
                            "Percent75": 968,
                            "Percent95": 1259,
                            "Percent99": 1327,
                            "StdDev": 383,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1033405.65,
                            "MaxDataKb": 1844239.68,
                            "AllDataMB": 25720.92,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 52,
                            "OkCount": 44,
                            "FailCount": 8,
                            "Min": 68,
                            "Mean": 726,
                            "Max": 1488,
                            "RPS": 1,
                            "Percent50": 616,
                            "Percent75": 1016,
                            "Percent95": 1418,
                            "Percent99": 1488,
                            "StdDev": 403,
                            "MinDataKb": 10939.2,
                            "MeanDataKb": 1044135.49,
                            "MaxDataKb": 1880566.96,
                            "AllDataMB": 23985.62,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 8
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 30,
                        "More800Less1200": 14,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "ramp_constant",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 8
                        }
                    ],
                    "Duration": "00:00:30.0128014"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 45,
                    "OkCount": 45,
                    "FailCount": 0,
                    "AllDataMB": 23159.829999999994,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 136,
                            "OkCount": 136,
                            "FailCount": 0,
                            "Min": 19,
                            "Mean": 782,
                            "Max": 1499,
                            "RPS": 2,
                            "Percent50": 829,
                            "Percent75": 1141,
                            "Percent95": 1428,
                            "Percent99": 1498,
                            "StdDev": 442,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 643879.46,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 64396.49,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 23,
                        "More800Less1200": 12,
                        "More1200": 10
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:35.0108641"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 72,
                    "OkCount": 67,
                    "FailCount": 5,
                    "AllDataMB": 29789.0,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 123,
                            "OkCount": 123,
                            "FailCount": 0,
                            "Min": 25,
                            "Mean": 825,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 813,
                            "Percent75": 1156,
                            "Percent95": 1466,
                            "Percent99": 1499,
                            "StdDev": 417,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 764477.19,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 61578.68,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 123,
                            "OkCount": 101,
                            "FailCount": 22,
                            "Min": 16,
                            "Mean": 823,
                            "Max": 1500,
                            "RPS": 1,
                            "Percent50": 870,
                            "Percent75": 1266,
                            "Percent95": 1407,
                            "Percent99": 1448,
                            "StdDev": 450,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 804005.58,
                            "MaxDataKb": 2084214.55,
                            "AllDataMB": 51801.07,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 22
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 38,
                        "More800Less1200": 16,
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
                            "Count": 22
                        }
                    ],
                    "Duration": "00:00:35.0108641"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 105,
                    "OkCount": 84,
                    "FailCount": 21,
                    "AllDataMB": 50533.71999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 122,
                            "OkCount": 109,
                            "FailCount": 13,
                            "Min": 15,
                            "Mean": 778,
                            "Max": 1513,
                            "RPS": 1,
                            "Percent50": 793,
                            "Percent75": 1124,
                            "Percent95": 1439,
                            "Percent99": 1486,
                            "StdDev": 442,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 505235.35,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 59168.24,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
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
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
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
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 103,
                            "OkCount": 81,
                            "FailCount": 22,
                            "Min": 11,
                            "Mean": 663,
                            "Max": 1509,
                            "RPS": 1,
                            "Percent50": 583,
                            "Percent75": 1020,
                            "Percent95": 1327,
                            "Percent99": 1469,
                            "StdDev": 411,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 539501.24,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 46831.76,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 78,
                            "OkCount": 63,
                            "FailCount": 15,
                            "Min": 30,
                            "Mean": 756,
                            "Max": 1488,
                            "RPS": 1,
                            "Percent50": 741,
                            "Percent75": 1126,
                            "Percent95": 1388,
                            "Percent99": 1435,
                            "StdDev": 418,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 390597.1,
                            "MaxDataKb": 2026122.21,
                            "AllDataMB": 35200.58,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 15
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 42,
                        "More800Less1200": 26,
                        "More1200": 16
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
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
                            "Count": 1
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 15
                        }
                    ],
                    "Duration": "00:00:35.0108641"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 52,
                    "OkCount": 52,
                    "FailCount": 0,
                    "AllDataMB": 27926.299999999996,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 188,
                            "OkCount": 188,
                            "FailCount": 0,
                            "Min": 19,
                            "Mean": 769,
                            "Max": 1499,
                            "RPS": 3,
                            "Percent50": 798,
                            "Percent75": 1118,
                            "Percent95": 1428,
                            "Percent99": 1484,
                            "StdDev": 432,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 831154.12,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 92322.79,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 28,
                        "More800Less1200": 17,
                        "More1200": 7
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:40.0108166"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 48,
                    "OkCount": 47,
                    "FailCount": 1,
                    "AllDataMB": 18407.359999999986,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 151,
                            "OkCount": 151,
                            "FailCount": 0,
                            "Min": 25,
                            "Mean": 823,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 843,
                            "Percent75": 1155,
                            "Percent95": 1458,
                            "Percent99": 1499,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 801934.04,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 73167.71,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 145,
                            "OkCount": 120,
                            "FailCount": 25,
                            "Min": 15,
                            "Mean": 771,
                            "Max": 1500,
                            "RPS": 1,
                            "Percent50": 796,
                            "Percent75": 1142,
                            "Percent95": 1397,
                            "Percent99": 1448,
                            "StdDev": 454,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 721264.82,
                            "MaxDataKb": 2084214.55,
                            "AllDataMB": 58619.4,
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
                        "Less800": 25,
                        "More800Less1200": 17,
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
                            "Count": 23
                        }
                    ],
                    "Duration": "00:00:40.0108166"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 134,
                    "OkCount": 116,
                    "FailCount": 18,
                    "AllDataMB": 72498.37000000002,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 174,
                            "OkCount": 154,
                            "FailCount": 20,
                            "Min": 13,
                            "Mean": 745,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 713,
                            "Percent75": 1117,
                            "Percent95": 1417,
                            "Percent99": 1473,
                            "StdDev": 446,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 949867.79,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 89643.76,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 149,
                            "OkCount": 118,
                            "FailCount": 31,
                            "Min": 11,
                            "Mean": 691,
                            "Max": 1509,
                            "RPS": 2,
                            "Percent50": 636,
                            "Percent75": 1061,
                            "Percent95": 1299,
                            "Percent99": 1469,
                            "StdDev": 405,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 892443.41,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 71478.53,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 115,
                            "OkCount": 98,
                            "FailCount": 17,
                            "Min": 15,
                            "Mean": 696,
                            "Max": 1488,
                            "RPS": 1,
                            "Percent50": 689,
                            "Percent75": 1016,
                            "Percent95": 1386,
                            "Percent99": 1435,
                            "StdDev": 427,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 730404.48,
                            "MaxDataKb": 2026122.21,
                            "AllDataMB": 52576.66,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 17
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 71,
                        "More800Less1200": 26,
                        "More1200": 18
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 17
                        }
                    ],
                    "Duration": "00:00:40.0108166"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 50,
                    "OkCount": 50,
                    "FailCount": 0,
                    "AllDataMB": 19841.20000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 239,
                            "OkCount": 239,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 776,
                            "Max": 1499,
                            "RPS": 3,
                            "Percent50": 828,
                            "Percent75": 1143,
                            "Percent95": 1440,
                            "Percent99": 1497,
                            "StdDev": 446,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 830489.95,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 112163.99,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 24,
                        "More800Less1200": 10,
                        "More1200": 16
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:45.0058724"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 51,
                    "OkCount": 45,
                    "FailCount": 6,
                    "AllDataMB": 24192.620000000024,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 175,
                            "OkCount": 175,
                            "FailCount": 0,
                            "Min": 25,
                            "Mean": 817,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 839,
                            "Percent75": 1156,
                            "Percent95": 1450,
                            "Percent99": 1499,
                            "StdDev": 414,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 988719.4,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 87190.91,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 171,
                            "OkCount": 142,
                            "FailCount": 29,
                            "Min": 15,
                            "Mean": 765,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 768,
                            "Percent75": 1142,
                            "Percent95": 1407,
                            "Percent99": 1457,
                            "StdDev": 449,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 940255.02,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 68788.82,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 29
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 26,
                        "More800Less1200": 8,
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
                            "Count": 29
                        }
                    ],
                    "Duration": "00:00:45.0058724"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 134,
                    "OkCount": 118,
                    "FailCount": 16,
                    "AllDataMB": 48171.56999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 224,
                            "OkCount": 200,
                            "FailCount": 24,
                            "Min": 13,
                            "Mean": 757,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 715,
                            "Percent75": 1112,
                            "Percent95": 1417,
                            "Percent99": 1473,
                            "StdDev": 431,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 976997.82,
                            "MaxDataKb": 2050049.58,
                            "AllDataMB": 110970.15,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
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
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 197,
                            "OkCount": 157,
                            "FailCount": 40,
                            "Min": 11,
                            "Mean": 712,
                            "Max": 1509,
                            "RPS": 2,
                            "Percent50": 685,
                            "Percent75": 1061,
                            "Percent95": 1413,
                            "Percent99": 1500,
                            "StdDev": 423,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 871755.29,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 83854.65,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
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
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 151,
                            "OkCount": 131,
                            "FailCount": 20,
                            "Min": 15,
                            "Mean": 711,
                            "Max": 1488,
                            "RPS": 2,
                            "Percent50": 725,
                            "Percent75": 1016,
                            "Percent95": 1372,
                            "Percent99": 1435,
                            "StdDev": 412,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 842619.55,
                            "MaxDataKb": 2026122.21,
                            "AllDataMB": 67045.72,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 20
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 60,
                        "More800Less1200": 35,
                        "More1200": 22
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 3
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
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                            "Count": 2
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 20
                        }
                    ],
                    "Duration": "00:00:45.0058724"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 50,
                    "OkCount": 50,
                    "FailCount": 0,
                    "AllDataMB": 34789.05,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 289,
                            "OkCount": 289,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 757,
                            "Max": 1499,
                            "RPS": 3,
                            "Percent50": 768,
                            "Percent75": 1129,
                            "Percent95": 1433,
                            "Percent99": 1484,
                            "StdDev": 440,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 858044.82,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 146953.04,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 31,
                        "More800Less1200": 14,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:50.0111953"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 52,
                    "OkCount": 51,
                    "FailCount": 1,
                    "AllDataMB": 30784.03999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 200,
                            "OkCount": 200,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 800,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 809,
                            "Percent75": 1151,
                            "Percent95": 1445,
                            "Percent99": 1499,
                            "StdDev": 415,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1006107.61,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 103178.37,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 197,
                            "OkCount": 167,
                            "FailCount": 30,
                            "Min": 15,
                            "Mean": 770,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 780,
                            "Percent75": 1142,
                            "Percent95": 1417,
                            "Percent99": 1457,
                            "StdDev": 449,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 994167.18,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 83585.4,
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
                        "Less800": 28,
                        "More800Less1200": 13,
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
                            "Count": 30
                        }
                    ],
                    "Duration": "00:00:50.0111953"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 129,
                    "OkCount": 114,
                    "FailCount": 15,
                    "AllDataMB": 49503.66,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 276,
                            "OkCount": 245,
                            "FailCount": 31,
                            "Min": 13,
                            "Mean": 772,
                            "Max": 1513,
                            "RPS": 3,
                            "Percent50": 748,
                            "Percent75": 1124,
                            "Percent95": 1420,
                            "Percent99": 1479,
                            "StdDev": 431,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1046980.88,
                            "MaxDataKb": 2066635.13,
                            "AllDataMB": 130934.26,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 236,
                            "OkCount": 192,
                            "FailCount": 44,
                            "Min": 11,
                            "Mean": 705,
                            "Max": 1509,
                            "RPS": 2,
                            "Percent50": 672,
                            "Percent75": 1058,
                            "Percent95": 1405,
                            "Percent99": 1500,
                            "StdDev": 410,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 877342.3,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 97303.92,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
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
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 189,
                            "OkCount": 165,
                            "FailCount": 24,
                            "Min": 15,
                            "Mean": 727,
                            "Max": 1490,
                            "RPS": 2,
                            "Percent50": 733,
                            "Percent75": 1059,
                            "Percent95": 1409,
                            "Percent99": 1463,
                            "StdDev": 421,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 881171.13,
                            "MaxDataKb": 2026122.21,
                            "AllDataMB": 83136.0,
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
                        "Less800": 61,
                        "More800Less1200": 29,
                        "More1200": 23
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 1
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
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 24
                        }
                    ],
                    "Duration": "00:00:50.0111953"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 48,
                    "OkCount": 48,
                    "FailCount": 0,
                    "AllDataMB": 21605.389999999985,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 337,
                            "OkCount": 337,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 773,
                            "Max": 1503,
                            "RPS": 3,
                            "Percent50": 794,
                            "Percent75": 1141,
                            "Percent95": 1433,
                            "Percent99": 1497,
                            "StdDev": 439,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 834160.54,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 168558.43,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 21,
                        "More800Less1200": 11,
                        "More1200": 14
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:00:55.0048391"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 50,
                    "OkCount": 47,
                    "FailCount": 3,
                    "AllDataMB": 15390.200000000012,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 224,
                            "OkCount": 224,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 793,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 793,
                            "Percent75": 1141,
                            "Percent95": 1450,
                            "Percent99": 1499,
                            "StdDev": 421,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1018172.69,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 112839.93,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 223,
                            "OkCount": 190,
                            "FailCount": 33,
                            "Min": 15,
                            "Mean": 747,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 765,
                            "Percent75": 1139,
                            "Percent95": 1412,
                            "Percent99": 1457,
                            "StdDev": 451,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 956639.76,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 89314.04,
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
                        "Less800": 27,
                        "More800Less1200": 13,
                        "More1200": 7
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
                    "Duration": "00:00:55.0048391"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 139,
                    "OkCount": 124,
                    "FailCount": 15,
                    "AllDataMB": 66949.70000000001,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 325,
                            "OkCount": 292,
                            "FailCount": 33,
                            "Min": 13,
                            "Mean": 760,
                            "Max": 1513,
                            "RPS": 3,
                            "Percent50": 728,
                            "Percent75": 1117,
                            "Percent95": 1420,
                            "Percent99": 1479,
                            "StdDev": 432,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1090386.71,
                            "MaxDataKb": 2066635.13,
                            "AllDataMB": 154599.4,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 284,
                            "OkCount": 236,
                            "FailCount": 48,
                            "Min": 11,
                            "Mean": 718,
                            "Max": 1509,
                            "RPS": 2,
                            "Percent50": 687,
                            "Percent75": 1063,
                            "Percent95": 1380,
                            "Percent99": 1500,
                            "StdDev": 417,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 941128.84,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 123968.72,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
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
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 231,
                            "OkCount": 199,
                            "FailCount": 32,
                            "Min": 15,
                            "Mean": 727,
                            "Max": 1490,
                            "RPS": 2,
                            "Percent50": 706,
                            "Percent75": 1045,
                            "Percent95": 1409,
                            "Percent99": 1473,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 916156.27,
                            "MaxDataKb": 2026122.21,
                            "AllDataMB": 99755.76,
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
                        "Less800": 69,
                        "More800Less1200": 32,
                        "More1200": 23
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 2
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
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 32
                        }
                    ],
                    "Duration": "00:00:55.0048391"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 54,
                    "OkCount": 54,
                    "FailCount": 0,
                    "AllDataMB": 24946.830000000016,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 390,
                            "OkCount": 390,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 757,
                            "Max": 1503,
                            "RPS": 4,
                            "Percent50": 768,
                            "Percent75": 1134,
                            "Percent95": 1433,
                            "Percent99": 1484,
                            "StdDev": 441,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 855021.04,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 193505.26,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 32,
                        "More800Less1200": 14,
                        "More1200": 8
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:00.0114420"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 48,
                    "OkCount": 40,
                    "FailCount": 8,
                    "AllDataMB": 21175.119999999995,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 250,
                            "OkCount": 250,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 788,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 781,
                            "Percent75": 1151,
                            "Percent95": 1450,
                            "Percent99": 1499,
                            "StdDev": 424,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1002326.85,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 128430.34,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 245,
                            "OkCount": 204,
                            "FailCount": 41,
                            "Min": 15,
                            "Mean": 741,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 749,
                            "Percent75": 1137,
                            "Percent95": 1417,
                            "Percent99": 1475,
                            "StdDev": 455,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 943735.86,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 94898.75,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 41
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 23,
                        "More800Less1200": 6,
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
                            "Count": 41
                        }
                    ],
                    "Duration": "00:01:00.0114420"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 129,
                    "OkCount": 113,
                    "FailCount": 16,
                    "AllDataMB": 55310.40000000002,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 374,
                            "OkCount": 332,
                            "FailCount": 42,
                            "Min": 13,
                            "Mean": 767,
                            "Max": 1513,
                            "RPS": 3,
                            "Percent50": 743,
                            "Percent75": 1124,
                            "Percent95": 1434,
                            "Percent99": 1479,
                            "StdDev": 434,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1060923.7,
                            "MaxDataKb": 2066635.13,
                            "AllDataMB": 169770.82,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 324,
                            "OkCount": 273,
                            "FailCount": 51,
                            "Min": 11,
                            "Mean": 723,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 689,
                            "Percent75": 1069,
                            "Percent95": 1380,
                            "Percent99": 1499,
                            "StdDev": 413,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 957005.44,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 144734.4,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
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
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 270,
                            "OkCount": 233,
                            "FailCount": 37,
                            "Min": 15,
                            "Mean": 725,
                            "Max": 1490,
                            "RPS": 2,
                            "Percent50": 733,
                            "Percent75": 1045,
                            "Percent95": 1390,
                            "Percent99": 1485,
                            "StdDev": 412,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 991676.48,
                            "MaxDataKb": 2077749.28,
                            "AllDataMB": 119129.06,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 37
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 57,
                        "More800Less1200": 38,
                        "More1200": 17
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 37
                        }
                    ],
                    "Duration": "00:01:00.0114420"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 48,
                    "OkCount": 48,
                    "FailCount": 0,
                    "AllDataMB": 23992.839999999997,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 438,
                            "OkCount": 438,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 758,
                            "Max": 1503,
                            "RPS": 4,
                            "Percent50": 768,
                            "Percent75": 1124,
                            "Percent95": 1436,
                            "Percent99": 1496,
                            "StdDev": 438,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 874407.65,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 217498.1,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 24,
                        "More800Less1200": 18,
                        "More1200": 6
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:05.0148249"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 52,
                    "OkCount": 47,
                    "FailCount": 5,
                    "AllDataMB": 24116.309999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 275,
                            "OkCount": 275,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 797,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 809,
                            "Percent75": 1151,
                            "Percent95": 1445,
                            "Percent99": 1499,
                            "StdDev": 423,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 985356.67,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 141040.06,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 272,
                            "OkCount": 226,
                            "FailCount": 46,
                            "Min": 15,
                            "Mean": 747,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 765,
                            "Percent75": 1140,
                            "Percent95": 1417,
                            "Percent99": 1475,
                            "StdDev": 452,
                            "MinDataKb": 3991.62,
                            "MeanDataKb": 1014290.96,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 106405.34,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 46
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 20,
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
                            "Count": 46
                        }
                    ],
                    "Duration": "00:01:05.0148249"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 140,
                    "OkCount": 124,
                    "FailCount": 16,
                    "AllDataMB": 57642.859999999986,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 426,
                            "OkCount": 376,
                            "FailCount": 50,
                            "Min": 13,
                            "Mean": 767,
                            "Max": 1513,
                            "RPS": 3,
                            "Percent50": 743,
                            "Percent75": 1125,
                            "Percent95": 1434,
                            "Percent99": 1479,
                            "StdDev": 433,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1090180.46,
                            "MaxDataKb": 2090433.54,
                            "AllDataMB": 196089.48,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 373,
                            "OkCount": 317,
                            "FailCount": 56,
                            "Min": 11,
                            "Mean": 711,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 687,
                            "Percent75": 1061,
                            "Percent95": 1364,
                            "Percent99": 1499,
                            "StdDev": 412,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 938714.96,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 165417.42,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
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
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 310,
                            "OkCount": 269,
                            "FailCount": 41,
                            "Min": 15,
                            "Mean": 740,
                            "Max": 1507,
                            "RPS": 2,
                            "Percent50": 733,
                            "Percent75": 1080,
                            "Percent95": 1435,
                            "Percent99": 1490,
                            "StdDev": 420,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 960387.36,
                            "MaxDataKb": 2077749.28,
                            "AllDataMB": 129770.24,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 41
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 74,
                        "More800Less1200": 22,
                        "More1200": 28
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 41
                        }
                    ],
                    "Duration": "00:01:05.0148249"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 50,
                    "OkCount": 50,
                    "FailCount": 0,
                    "AllDataMB": 25880.26999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 488,
                            "OkCount": 488,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 770,
                            "Max": 1510,
                            "RPS": 4,
                            "Percent50": 798,
                            "Percent75": 1138,
                            "Percent95": 1440,
                            "Percent99": 1496,
                            "StdDev": 439,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 884578.61,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 243378.37,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 19,
                        "More800Less1200": 17,
                        "More1200": 14
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:10.0102160"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 50,
                    "OkCount": 46,
                    "FailCount": 5,
                    "AllDataMB": 28242.110000000015,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 301,
                            "OkCount": 301,
                            "FailCount": 0,
                            "Min": 7,
                            "Mean": 799,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 809,
                            "Percent75": 1173,
                            "Percent95": 1459,
                            "Percent99": 1499,
                            "StdDev": 430,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 998010.47,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 159039.41,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 297,
                            "OkCount": 246,
                            "FailCount": 51,
                            "Min": 13,
                            "Mean": 747,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 765,
                            "Percent75": 1139,
                            "Percent95": 1417,
                            "Percent99": 1475,
                            "StdDev": 450,
                            "MinDataKb": 3991.62,
                            "MeanDataKb": 996504.06,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 116648.1,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 51
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 24,
                        "More800Less1200": 9,
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
                            "Count": 51
                        }
                    ],
                    "Duration": "00:01:10.0102160"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 128,
                    "OkCount": 110,
                    "FailCount": 18,
                    "AllDataMB": 64062.16000000003,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 477,
                            "OkCount": 419,
                            "FailCount": 58,
                            "Min": 13,
                            "Mean": 757,
                            "Max": 1513,
                            "RPS": 3,
                            "Percent50": 726,
                            "Percent75": 1117,
                            "Percent95": 1434,
                            "Percent99": 1485,
                            "StdDev": 430,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1078918.49,
                            "MaxDataKb": 2090433.54,
                            "AllDataMB": 221217.17,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 413,
                            "OkCount": 352,
                            "FailCount": 61,
                            "Min": 11,
                            "Mean": 698,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 670,
                            "Percent75": 1047,
                            "Percent95": 1364,
                            "Percent99": 1497,
                            "StdDev": 411,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 958236.66,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 184846.42,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
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
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 347,
                            "OkCount": 302,
                            "FailCount": 45,
                            "Min": 15,
                            "Mean": 740,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 744,
                            "Percent75": 1080,
                            "Percent95": 1421,
                            "Percent99": 1493,
                            "StdDev": 421,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 976378.13,
                            "MaxDataKb": 2077749.28,
                            "AllDataMB": 149275.71,
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
                        "Less800": 71,
                        "More800Less1200": 26,
                        "More1200": 13
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 45
                        }
                    ],
                    "Duration": "00:01:10.0102160"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 54,
                    "OkCount": 54,
                    "FailCount": 0,
                    "AllDataMB": 33652.81,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 542,
                            "OkCount": 542,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 776,
                            "Max": 1510,
                            "RPS": 4,
                            "Percent50": 807,
                            "Percent75": 1142,
                            "Percent95": 1436,
                            "Percent99": 1497,
                            "StdDev": 437,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 888132.56,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 277031.18,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 23,
                        "More800Less1200": 20,
                        "More1200": 11
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:15.0113779"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 51,
                    "OkCount": 48,
                    "FailCount": 2,
                    "AllDataMB": 18995.669999999984,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 327,
                            "OkCount": 327,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 793,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 786,
                            "Percent75": 1180,
                            "Percent95": 1459,
                            "Percent99": 1499,
                            "StdDev": 431,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 996100.47,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 168650.35,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 321,
                            "OkCount": 268,
                            "FailCount": 53,
                            "Min": 13,
                            "Mean": 733,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 720,
                            "Percent75": 1125,
                            "Percent95": 1417,
                            "Percent99": 1463,
                            "StdDev": 448,
                            "MinDataKb": 3991.62,
                            "MeanDataKb": 984003.06,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 126032.83,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 53
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 32,
                        "More800Less1200": 7,
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
                            "Count": 53
                        }
                    ],
                    "Duration": "00:01:15.0113779"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 129,
                    "OkCount": 108,
                    "FailCount": 21,
                    "AllDataMB": 56422.19999999995,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 530,
                            "OkCount": 464,
                            "FailCount": 66,
                            "Min": 13,
                            "Mean": 746,
                            "Max": 1513,
                            "RPS": 4,
                            "Percent50": 714,
                            "Percent75": 1108,
                            "Percent95": 1424,
                            "Percent99": 1479,
                            "StdDev": 428,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1079469.55,
                            "MaxDataKb": 2090433.54,
                            "AllDataMB": 243320.65,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 455,
                            "OkCount": 388,
                            "FailCount": 67,
                            "Min": 11,
                            "Mean": 693,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 670,
                            "Percent75": 1044,
                            "Percent95": 1362,
                            "Percent99": 1497,
                            "StdDev": 410,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 948507.81,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 203782.23,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 384,
                            "OkCount": 332,
                            "FailCount": 52,
                            "Min": 15,
                            "Mean": 734,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 726,
                            "Percent75": 1059,
                            "Percent95": 1418,
                            "Percent99": 1493,
                            "StdDev": 415,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 972694.95,
                            "MaxDataKb": 2077749.28,
                            "AllDataMB": 164658.62,
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
                        "Less800": 70,
                        "More800Less1200": 23,
                        "More1200": 15
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 1
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
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
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 52
                        }
                    ],
                    "Duration": "00:01:15.0113779"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 47,
                    "OkCount": 47,
                    "FailCount": 0,
                    "AllDataMB": 23982.600000000035,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 589,
                            "OkCount": 589,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 776,
                            "Max": 1510,
                            "RPS": 4,
                            "Percent50": 798,
                            "Percent75": 1143,
                            "Percent95": 1433,
                            "Percent99": 1496,
                            "StdDev": 434,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 880340.28,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 301013.78,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 27,
                        "More800Less1200": 8,
                        "More1200": 12
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:20.0111759"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 48,
                    "OkCount": 44,
                    "FailCount": 4,
                    "AllDataMB": 26757.5,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 350,
                            "OkCount": 350,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 791,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 793,
                            "Percent75": 1200,
                            "Percent95": 1458,
                            "Percent99": 1499,
                            "StdDev": 436,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1002927.5,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 185602.82,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 347,
                            "OkCount": 290,
                            "FailCount": 57,
                            "Min": 13,
                            "Mean": 731,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 716,
                            "Percent75": 1126,
                            "Percent95": 1422,
                            "Percent99": 1475,
                            "StdDev": 452,
                            "MinDataKb": 3991.62,
                            "MeanDataKb": 979842.32,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 135837.86,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 57
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 24,
                        "More800Less1200": 8,
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
                            "Count": 57
                        }
                    ],
                    "Duration": "00:01:20.0111759"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 117,
                    "OkCount": 94,
                    "FailCount": 23,
                    "AllDataMB": 44154.57999999996,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 578,
                            "OkCount": 500,
                            "FailCount": 78,
                            "Min": 13,
                            "Mean": 754,
                            "Max": 1513,
                            "RPS": 4,
                            "Percent50": 721,
                            "Percent75": 1124,
                            "Percent95": 1424,
                            "Percent99": 1479,
                            "StdDev": 429,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1093719.3,
                            "MaxDataKb": 2090433.54,
                            "AllDataMB": 264853.74,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
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
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 428,
                                    "Message": "System.Exception: Precondition Required",
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 494,
                            "OkCount": 419,
                            "FailCount": 75,
                            "Min": 11,
                            "Mean": 690,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 672,
                            "Percent75": 1034,
                            "Percent95": 1362,
                            "Percent99": 1497,
                            "StdDev": 408,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 963520.59,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 219883.14,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 411,
                            "OkCount": 356,
                            "FailCount": 55,
                            "Min": 13,
                            "Mean": 736,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 733,
                            "Percent75": 1064,
                            "Percent95": 1409,
                            "Percent99": 1490,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 965332.45,
                            "MaxDataKb": 2077749.28,
                            "AllDataMB": 171179.2,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 55
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 48,
                        "More800Less1200": 29,
                        "More1200": 18
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 421,
                            "Message": "System.Exception: Misdirected Request",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 428,
                            "Message": "System.Exception: Precondition Required",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 55
                        }
                    ],
                    "Duration": "00:01:20.0111759"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 52,
                    "OkCount": 52,
                    "FailCount": 0,
                    "AllDataMB": 27614.909999999974,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 641,
                            "OkCount": 641,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 774,
                            "Max": 1510,
                            "RPS": 4,
                            "Percent50": 798,
                            "Percent75": 1141,
                            "Percent95": 1433,
                            "Percent99": 1496,
                            "StdDev": 434,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 882829.98,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 328628.69,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 27,
                        "More800Less1200": 16,
                        "More1200": 9
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [],
                    "Duration": "00:01:24.9980881"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 53,
                    "OkCount": 52,
                    "FailCount": 1,
                    "AllDataMB": 25753.27000000002,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 376,
                            "OkCount": 376,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 786,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 782,
                            "Percent75": 1180,
                            "Percent95": 1445,
                            "Percent99": 1499,
                            "StdDev": 434,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1006672.3,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 199984.16,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 373,
                            "OkCount": 315,
                            "FailCount": 58,
                            "Min": 0,
                            "Mean": 722,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 703,
                            "Percent75": 1123,
                            "Percent95": 1417,
                            "Percent99": 1475,
                            "StdDev": 447,
                            "MinDataKb": 3991.62,
                            "MeanDataKb": 978494.17,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 147209.79,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 58
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 33,
                        "More800Less1200": 11,
                        "More1200": 7
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 58
                        }
                    ],
                    "Duration": "00:01:24.9980881"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 121,
                    "OkCount": 99,
                    "FailCount": 22,
                    "AllDataMB": 56810.47000000009,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 629,
                            "OkCount": 540,
                            "FailCount": 89,
                            "Min": 13,
                            "Mean": 753,
                            "Max": 1513,
                            "RPS": 4,
                            "Percent50": 726,
                            "Percent75": 1117,
                            "Percent95": 1433,
                            "Percent99": 1484,
                            "StdDev": 431,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1096407.45,
                            "MaxDataKb": 2090433.54,
                            "AllDataMB": 285268.47,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 532,
                            "OkCount": 449,
                            "FailCount": 83,
                            "Min": 11,
                            "Mean": 695,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 685,
                            "Percent75": 1044,
                            "Percent95": 1367,
                            "Percent99": 1497,
                            "StdDev": 408,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 966062.8,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 238040.71,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
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
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 443,
                            "OkCount": 385,
                            "FailCount": 58,
                            "Min": 13,
                            "Mean": 739,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 733,
                            "Percent75": 1080,
                            "Percent95": 1405,
                            "Percent99": 1490,
                            "StdDev": 416,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 979201.45,
                            "MaxDataKb": 2077749.28,
                            "AllDataMB": 189417.37,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 58
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 54,
                        "More800Less1200": 27,
                        "More1200": 18
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 9
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 58
                        }
                    ],
                    "Duration": "00:01:24.9980881"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 48,
                    "OkCount": 48,
                    "FailCount": 0,
                    "AllDataMB": 27904.809999999998,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 689,
                            "OkCount": 689,
                            "FailCount": 0,
                            "Min": 5,
                            "Mean": 775,
                            "Max": 1510,
                            "RPS": 4,
                            "Percent50": 798,
                            "Percent75": 1143,
                            "Percent95": 1436,
                            "Percent99": 1485,
                            "StdDev": 436,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 883661.91,
                            "MaxDataKb": 2094145.19,
                            "AllDataMB": 356533.5,
                            "ErrorStats": []
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 25,
                        "More800Less1200": 11,
                        "More1200": 12
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
                    "RequestCount": 48,
                    "OkCount": 42,
                    "FailCount": 6,
                    "AllDataMB": 23412.73999999999,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 399,
                            "OkCount": 399,
                            "FailCount": 0,
                            "Min": 0,
                            "Mean": 783,
                            "Max": 1513,
                            "RPS": 2,
                            "Percent50": 765,
                            "Percent75": 1173,
                            "Percent95": 1445,
                            "Percent99": 1499,
                            "StdDev": 432,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1008223.78,
                            "MaxDataKb": 2095457.07,
                            "AllDataMB": 215094.43,
                            "ErrorStats": []
                        },
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 398,
                            "OkCount": 334,
                            "FailCount": 64,
                            "Min": 0,
                            "Mean": 723,
                            "Max": 1500,
                            "RPS": 2,
                            "Percent50": 700,
                            "Percent75": 1122,
                            "Percent95": 1417,
                            "Percent99": 1475,
                            "StdDev": 446,
                            "MinDataKb": 3991.62,
                            "MeanDataKb": 967320.03,
                            "MaxDataKb": 2091092.82,
                            "AllDataMB": 155512.26,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 64
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 26,
                        "More800Less1200": 8,
                        "More1200": 8
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 5
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 64
                        }
                    ],
                    "Duration": "00:01:30"
                },
                {
                    "ScenarioName": "scenario 3",
                    "RequestCount": 120,
                    "OkCount": 101,
                    "FailCount": 19,
                    "AllDataMB": 64092.58999999997,
                    "StepStats": [
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 677,
                            "OkCount": 581,
                            "FailCount": 96,
                            "Min": 13,
                            "Mean": 753,
                            "Max": 1513,
                            "RPS": 4,
                            "Percent50": 737,
                            "Percent75": 1122,
                            "Percent95": 1434,
                            "Percent99": 1484,
                            "StdDev": 431,
                            "MinDataKb": 799.62,
                            "MeanDataKb": 1100726.83,
                            "MaxDataKb": 2090433.54,
                            "AllDataMB": 307274.24,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 5
                                },
                                {
                                    "ErrorCode": 431,
                                    "Message": "System.Exception: Request Header Fields Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 500,
                                    "Message": "System.Exception: Internal Server Error",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 414,
                                    "Message": "System.Exception: URI Too Long",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 511,
                                    "Message": "System.Exception: Network Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 407,
                                    "Message": "System.Exception: Proxy Authentication Required",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                                    "Count": 2
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 4",
                            "RequestCount": 573,
                            "OkCount": 482,
                            "FailCount": 91,
                            "Min": 11,
                            "Mean": 708,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 704,
                            "Percent75": 1047,
                            "Percent95": 1367,
                            "Percent99": 1488,
                            "StdDev": 405,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 966511.73,
                            "MaxDataKb": 2094941.13,
                            "AllDataMB": 265330.51,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 401,
                                    "Message": "System.Exception: Unauthorized",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 504,
                                    "Message": "System.Exception: Gateway Timeout",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 416,
                                    "Message": "System.Exception: Requested Range Not Satisfiable",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 411,
                                    "Message": "System.Exception: Length Required",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 406,
                                    "Message": "System.Exception: Not Acceptable",
                                    "Count": 6
                                },
                                {
                                    "ErrorCode": 503,
                                    "Message": "System.Exception: Service Unavailable",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 506,
                                    "Message": "System.Exception: Variant Also Negotiates",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 402,
                                    "Message": "System.Exception: Payment Required",
                                    "Count": 6
                                },
                                {
                                    "ErrorCode": 501,
                                    "Message": "System.Exception: Not Implemented",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 413,
                                    "Message": "System.Exception: Payload Too Large",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 410,
                                    "Message": "System.Exception: Gone",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 409,
                                    "Message": "System.Exception: Conflict",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 415,
                                    "Message": "System.Exception: Unsupported Media Type",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 408,
                                    "Message": "System.Exception: Request Timeout",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 404,
                                    "Message": "System.Exception: Not Found",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 510,
                                    "Message": "System.Exception: Not Extended",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 423,
                                    "Message": "System.Exception: Locked (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 507,
                                    "Message": "System.Exception: Insufficient Storage",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 400,
                                    "Message": "System.Exception: Bad Request",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 417,
                                    "Message": "System.Exception: Expectation Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 405,
                                    "Message": "System.Exception: Method Not Allowed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 505,
                                    "Message": "System.Exception: HTTP Version Not Supported",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 403,
                                    "Message": "System.Exception: Forbidden",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 412,
                                    "Message": "System.Exception: Precondition Failed",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 418,
                                    "Message": "System.Exception: I'm a teapot",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 422,
                                    "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                                    "Count": 4
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
                                    "ErrorCode": 502,
                                    "Message": "System.Exception: Bad Gateway",
                                    "Count": 1
                                },
                                {
                                    "ErrorCode": 451,
                                    "Message": "System.Exception: Unavailable For Legal Reasons",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 426,
                                    "Message": "System.Exception: Upgrade Required",
                                    "Count": 4
                                },
                                {
                                    "ErrorCode": 429,
                                    "Message": "System.Exception: Too Many Requests",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 424,
                                    "Message": "System.Exception: Failed Dependency (WebDAV)",
                                    "Count": 2
                                },
                                {
                                    "ErrorCode": 508,
                                    "Message": "System.Exception: Loop Detected (WebDAV)",
                                    "Count": 3
                                },
                                {
                                    "ErrorCode": 421,
                                    "Message": "System.Exception: Misdirected Request",
                                    "Count": 1
                                }
                            ]
                        },
                        {
                            "StepName": "pull html 5",
                            "RequestCount": 474,
                            "OkCount": 412,
                            "FailCount": 62,
                            "Min": 13,
                            "Mean": 749,
                            "Max": 1509,
                            "RPS": 3,
                            "Percent50": 754,
                            "Percent75": 1090,
                            "Percent95": 1392,
                            "Percent99": 1490,
                            "StdDev": 414,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 986061.18,
                            "MaxDataKb": 2093625.12,
                            "AllDataMB": 204214.39,
                            "ErrorStats": [
                                {
                                    "ErrorCode": 0,
                                    "Message": "System.Exception: unknown client's error",
                                    "Count": 62
                                }
                            ]
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 44,
                        "More800Less1200": 38,
                        "More1200": 18
                    },
                    "LoadSimulationStats": {
                        "SimulationName": "inject_per_sec",
                        "Value": 10
                    },
                    "ErrorStats": [
                        {
                            "ErrorCode": 410,
                            "Message": "System.Exception: Gone",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 416,
                            "Message": "System.Exception: Requested Range Not Satisfiable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 426,
                            "Message": "System.Exception: Upgrade Required",
                            "Count": 9
                        },
                        {
                            "ErrorCode": 402,
                            "Message": "System.Exception: Payment Required",
                            "Count": 10
                        },
                        {
                            "ErrorCode": 451,
                            "Message": "System.Exception: Unavailable For Legal Reasons",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 411,
                            "Message": "System.Exception: Length Required",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 418,
                            "Message": "System.Exception: I'm a teapot",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 501,
                            "Message": "System.Exception: Not Implemented",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 404,
                            "Message": "System.Exception: Not Found",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 413,
                            "Message": "System.Exception: Payload Too Large",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 423,
                            "Message": "System.Exception: Locked (WebDAV)",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 412,
                            "Message": "System.Exception: Precondition Failed",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 422,
                            "Message": "System.Exception: Unprocessable Entity (WebDAV)",
                            "Count": 9
                        },
                        {
                            "ErrorCode": 401,
                            "Message": "System.Exception: Unauthorized",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 431,
                            "Message": "System.Exception: Request Header Fields Too Large",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 408,
                            "Message": "System.Exception: Request Timeout",
                            "Count": 8
                        },
                        {
                            "ErrorCode": 500,
                            "Message": "System.Exception: Internal Server Error",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 504,
                            "Message": "System.Exception: Gateway Timeout",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 510,
                            "Message": "System.Exception: Not Extended",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 414,
                            "Message": "System.Exception: URI Too Long",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 511,
                            "Message": "System.Exception: Network Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 508,
                            "Message": "System.Exception: Loop Detected (WebDAV)",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 405,
                            "Message": "System.Exception: Method Not Allowed",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 506,
                            "Message": "System.Exception: Variant Also Negotiates",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 417,
                            "Message": "System.Exception: Expectation Failed",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 407,
                            "Message": "System.Exception: Proxy Authentication Required",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 503,
                            "Message": "System.Exception: Service Unavailable",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 507,
                            "Message": "System.Exception: Insufficient Storage",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 429,
                            "Message": "System.Exception: Too Many Requests",
                            "Count": 5
                        },
                        {
                            "ErrorCode": 403,
                            "Message": "System.Exception: Forbidden",
                            "Count": 4
                        },
                        {
                            "ErrorCode": 400,
                            "Message": "System.Exception: Bad Request",
                            "Count": 7
                        },
                        {
                            "ErrorCode": 502,
                            "Message": "System.Exception: Bad Gateway",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 424,
                            "Message": "System.Exception: Failed Dependency (WebDAV)",
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
                            "Count": 2
                        },
                        {
                            "ErrorCode": 406,
                            "Message": "System.Exception: Not Acceptable",
                            "Count": 6
                        },
                        {
                            "ErrorCode": 409,
                            "Message": "System.Exception: Conflict",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 415,
                            "Message": "System.Exception: Unsupported Media Type",
                            "Count": 3
                        },
                        {
                            "ErrorCode": 505,
                            "Message": "System.Exception: HTTP Version Not Supported",
                            "Count": 2
                        },
                        {
                            "ErrorCode": 0,
                            "Message": "System.Exception: unknown client's error",
                            "Count": 62
                        }
                    ],
                    "Duration": "00:01:30"
                }
            ]
        ]
    } };
