const getStatsData = () => (
    {
        "RequestCount": 2846,
        "OkCount": 2439,
        "FailCount": 407,
        "AllDataMB": 2432833.7500391006,
        "ScenarioStats": [
            {
                "ScenarioName": "scenario 1",
                "RequestCount": 1413,
                "OkCount": 1216,
                "FailCount": 197,
                "AllDataMB": 1203763.4423913956,
                "StepStats": [
                    {
                        "StepName": "pull html 1",
                        "RequestCount": 1413,
                        "OkCount": 1216,
                        "FailCount": 197,
                        "Min": 0,
                        "Mean": 497,
                        "Max": 1001,
                        "RPS": 15,
                        "Percent50": 500,
                        "Percent75": 736,
                        "Percent95": 940,
                        "Percent99": 950,
                        "Percent999": 999,
                        "StdDev": 283,
                        "MinDataKb": 664.73,
                        "MeanDataKb": 1013029.63,
                        "MaxDataKb": 2095897.12,
                        "AllDataMB": 1203763.4423913956
                    }
                ],
                "LatencyCount": {
                    "Less800": 991,
                    "More800Less1200": 223,
                    "More1200": 0
                },
                "Duration": "00:01:20"
            },
            {
                "ScenarioName": "scenario 2",
                "RequestCount": 1433,
                "OkCount": 1223,
                "FailCount": 210,
                "AllDataMB": 1229070.307647705,
                "StepStats": [
                    {
                        "StepName": "pull html 2",
                        "RequestCount": 780,
                        "OkCount": 658,
                        "FailCount": 122,
                        "Min": 4,
                        "Mean": 501,
                        "Max": 1000,
                        "RPS": 8,
                        "Percent50": 491,
                        "Percent75": 754,
                        "Percent95": 950,
                        "Percent99": 959,
                        "Percent999": 999,
                        "StdDev": 290,
                        "MinDataKb": 613.94,
                        "MeanDataKb": 1021460.47,
                        "MaxDataKb": 2093301.13,
                        "AllDataMB": 653823.8431634903
                    },
                    {
                        "StepName": "pull html 3",
                        "RequestCount": 653,
                        "OkCount": 565,
                        "FailCount": 88,
                        "Min": 4,
                        "Mean": 494,
                        "Max": 999,
                        "RPS": 7,
                        "Percent50": 506,
                        "Percent75": 759,
                        "Percent95": 956,
                        "Percent99": 959,
                        "Percent999": 999,
                        "StdDev": 293,
                        "MinDataKb": 878.28,
                        "MeanDataKb": 1038169.88,
                        "MaxDataKb": 2090447.38,
                        "AllDataMB": 575246.4644842148
                    }
                ],
                "LatencyCount": {
                    "Less800": 968,
                    "More800Less1200": 254,
                    "More1200": 0
                },
                "Duration": "00:01:20"
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
                        "1948778843"
                    ],
                    [
                        "Property2",
                        "306909376"
                    ],
                    [
                        "Property3",
                        "859223877"
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
                        "729625448"
                    ],
                    [
                        "Property 2",
                        "55506339"
                    ],
                    [
                        "Property 3",
                        "1913068507"
                    ]
                ]
            }
        ],
        "NodeInfo": {
            "MachineName": "vitalii-N752VX",
            "NodeType": {
                "Case": "SingleNode"
            },
            "CurrentOperation": 5,
            "OS": {
                "Platform": 4,
                "ServicePack": "",
                "Version": "5.3.0.51",
                "VersionString": "Unix 5.3.0.51"
            },
            "DotNetVersion": ".NETCoreApp,Version=v3.1",
            "Processor": "",
            "CoresCount": 8
        }
    }
);

const getTimeLineStatsData = () => (
    {
        "TimeStamps": [
            "00:00:10",
            "00:00:20",
            "00:00:30",
            "00:00:40",
            "00:00:49",
            "00:00:59",
            "00:01:09",
            "00:01:20"
        ],
        "ScenarioStats": [
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 49,
                    "OkCount": 43,
                    "FailCount": 6,
                    "AllDataMB": 51938.496582984924,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 49,
                            "OkCount": 43,
                            "FailCount": 6,
                            "Min": 12,
                            "Mean": 466,
                            "Max": 978,
                            "RPS": 4,
                            "Percent50": 383,
                            "Percent75": 691,
                            "Percent95": 921,
                            "Percent99": 950,
                            "Percent999": 970,
                            "StdDev": 281,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 1005348.91,
                            "MaxDataKb": 2030250.84,
                            "AllDataMB": 51938.496582984924
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 36,
                        "More800Less1200": 7,
                        "More1200": 0
                    },
                    "Duration": "00:00:10.0026785"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 52,
                    "OkCount": 46,
                    "FailCount": 6,
                    "AllDataMB": 40872.134103775024,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 30,
                            "OkCount": 25,
                            "FailCount": 5,
                            "Min": 60,
                            "Mean": 440,
                            "Max": 959,
                            "RPS": 2,
                            "Percent50": 451,
                            "Percent75": 636,
                            "Percent95": 873,
                            "Percent99": 950,
                            "Percent999": 959,
                            "StdDev": 264,
                            "MinDataKb": 19831.03,
                            "MeanDataKb": 1018622.48,
                            "MaxDataKb": 2035320.0,
                            "AllDataMB": 23311.41968345642
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 22,
                            "OkCount": 21,
                            "FailCount": 1,
                            "Min": 8,
                            "Mean": 449,
                            "Max": 998,
                            "RPS": 2,
                            "Percent50": 364,
                            "Percent75": 771,
                            "Percent95": 990,
                            "Percent99": 995,
                            "Percent999": 997,
                            "StdDev": 313,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 694837.03,
                            "MaxDataKb": 2090020.72,
                            "AllDataMB": 17560.714420318604
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 39,
                        "More800Less1200": 7,
                        "More1200": 0
                    },
                    "Duration": "00:00:10.0026785"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 190,
                    "OkCount": 167,
                    "FailCount": 23,
                    "AllDataMB": 184584.0932073593,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 190,
                            "OkCount": 167,
                            "FailCount": 23,
                            "Min": 3,
                            "Mean": 487,
                            "Max": 996,
                            "RPS": 8,
                            "Percent50": 477,
                            "Percent75": 713,
                            "Percent95": 929,
                            "Percent99": 950,
                            "Percent999": 995,
                            "StdDev": 282,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 992822.35,
                            "MaxDataKb": 2095465.38,
                            "AllDataMB": 184584.0932073593
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 137,
                        "More800Less1200": 30,
                        "More1200": 0
                    },
                    "Duration": "00:00:20.0006984"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 202,
                    "OkCount": 175,
                    "FailCount": 27,
                    "AllDataMB": 170395.02914524078,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 112,
                            "OkCount": 95,
                            "FailCount": 17,
                            "Min": 4,
                            "Mean": 483,
                            "Max": 999,
                            "RPS": 4,
                            "Percent50": 486,
                            "Percent75": 732,
                            "Percent95": 926,
                            "Percent99": 950,
                            "Percent999": 999,
                            "StdDev": 304,
                            "MinDataKb": 13521.2,
                            "MeanDataKb": 1069871.58,
                            "MaxDataKb": 2091858.31,
                            "AllDataMB": 88826.02756977081
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 90,
                            "OkCount": 80,
                            "FailCount": 10,
                            "Min": 4,
                            "Mean": 463,
                            "Max": 998,
                            "RPS": 4,
                            "Percent50": 459,
                            "Percent75": 698,
                            "Percent95": 971,
                            "Percent99": 980,
                            "Percent999": 997,
                            "StdDev": 305,
                            "MinDataKb": 0.0,
                            "MeanDataKb": 975383.76,
                            "MaxDataKb": 2090020.72,
                            "AllDataMB": 81569.00157546997
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 140,
                        "More800Less1200": 35,
                        "More1200": 0
                    },
                    "Duration": "00:00:20.0006984"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 384,
                    "OkCount": 335,
                    "FailCount": 49,
                    "AllDataMB": 345602.02653312683,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 384,
                            "OkCount": 335,
                            "FailCount": 49,
                            "Min": 3,
                            "Mean": 501,
                            "Max": 996,
                            "RPS": 11,
                            "Percent50": 523,
                            "Percent75": 740,
                            "Percent95": 934,
                            "Percent99": 950,
                            "Percent999": 994,
                            "StdDev": 287,
                            "MinDataKb": 19414.05,
                            "MeanDataKb": 1038176.41,
                            "MaxDataKb": 2095465.38,
                            "AllDataMB": 345602.02653312683
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 271,
                        "More800Less1200": 64,
                        "More1200": 0
                    },
                    "Duration": "00:00:30.0018655"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 397,
                    "OkCount": 349,
                    "FailCount": 48,
                    "AllDataMB": 343659.65815734863,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 214,
                            "OkCount": 187,
                            "FailCount": 27,
                            "Min": 4,
                            "Mean": 508,
                            "Max": 999,
                            "RPS": 6,
                            "Percent50": 495,
                            "Percent75": 764,
                            "Percent95": 947,
                            "Percent99": 950,
                            "Percent999": 999,
                            "StdDev": 296,
                            "MinDataKb": 13521.2,
                            "MeanDataKb": 1008050.03,
                            "MaxDataKb": 2091858.31,
                            "AllDataMB": 179134.01071548462
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 183,
                            "OkCount": 162,
                            "FailCount": 21,
                            "Min": 4,
                            "Mean": 485,
                            "Max": 998,
                            "RPS": 5,
                            "Percent50": 479,
                            "Percent75": 738,
                            "Percent95": 954,
                            "Percent99": 960,
                            "Percent999": 997,
                            "StdDev": 296,
                            "MinDataKb": 878.28,
                            "MeanDataKb": 1041129.14,
                            "MaxDataKb": 2090020.72,
                            "AllDataMB": 164525.647441864
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 274,
                        "More800Less1200": 75,
                        "More1200": 0
                    },
                    "Duration": "00:00:30.0018655"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 588,
                    "OkCount": 511,
                    "FailCount": 77,
                    "AllDataMB": 523119.5382680893,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 588,
                            "OkCount": 511,
                            "FailCount": 77,
                            "Min": 3,
                            "Mean": 495,
                            "Max": 996,
                            "RPS": 12,
                            "Percent50": 500,
                            "Percent75": 736,
                            "Percent95": 934,
                            "Percent99": 950,
                            "Percent999": 994,
                            "StdDev": 285,
                            "MinDataKb": 15525.48,
                            "MeanDataKb": 1043072.6,
                            "MaxDataKb": 2095465.38,
                            "AllDataMB": 523119.5382680893
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 417,
                        "More800Less1200": 93,
                        "More1200": 0
                    },
                    "Duration": "00:00:40.0018543"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 607,
                    "OkCount": 523,
                    "FailCount": 84,
                    "AllDataMB": 524335.5025720596,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 328,
                            "OkCount": 285,
                            "FailCount": 43,
                            "Min": 4,
                            "Mean": 489,
                            "Max": 1000,
                            "RPS": 7,
                            "Percent50": 486,
                            "Percent75": 737,
                            "Percent95": 959,
                            "Percent99": 980,
                            "Percent999": 999,
                            "StdDev": 293,
                            "MinDataKb": 8090.84,
                            "MeanDataKb": 998733.56,
                            "MaxDataKb": 2093301.13,
                            "AllDataMB": 273521.7368106842
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 279,
                            "OkCount": 238,
                            "FailCount": 41,
                            "Min": 4,
                            "Mean": 503,
                            "Max": 998,
                            "RPS": 5,
                            "Percent50": 520,
                            "Percent75": 771,
                            "Percent95": 971,
                            "Percent99": 980,
                            "Percent999": 995,
                            "StdDev": 299,
                            "MinDataKb": 878.28,
                            "MeanDataKb": 1072287.43,
                            "MaxDataKb": 2090020.72,
                            "AllDataMB": 250813.76576137543
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 413,
                        "More800Less1200": 110,
                        "More1200": 0
                    },
                    "Duration": "00:00:40.0018543"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 780,
                    "OkCount": 673,
                    "FailCount": 107,
                    "AllDataMB": 678963.3040590286,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 780,
                            "OkCount": 673,
                            "FailCount": 107,
                            "Min": 3,
                            "Mean": 503,
                            "Max": 996,
                            "RPS": 13,
                            "Percent50": 520,
                            "Percent75": 744,
                            "Percent95": 934,
                            "Percent99": 950,
                            "Percent999": 994,
                            "StdDev": 284,
                            "MinDataKb": 4289.88,
                            "MeanDataKb": 1029819.34,
                            "MaxDataKb": 2095465.38,
                            "AllDataMB": 678963.3040590286
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 547,
                        "More800Less1200": 125,
                        "More1200": 0
                    },
                    "Duration": "00:00:49.9997722"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 816,
                    "OkCount": 696,
                    "FailCount": 120,
                    "AllDataMB": 697184.8115587234,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 444,
                            "OkCount": 376,
                            "FailCount": 68,
                            "Min": 4,
                            "Mean": 493,
                            "Max": 1000,
                            "RPS": 7,
                            "Percent50": 488,
                            "Percent75": 754,
                            "Percent95": 957,
                            "Percent99": 980,
                            "Percent999": 999,
                            "StdDev": 296,
                            "MinDataKb": 8090.84,
                            "MeanDataKb": 997067.1,
                            "MaxDataKb": 2093301.13,
                            "AllDataMB": 362012.81656360626
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 372,
                            "OkCount": 320,
                            "FailCount": 52,
                            "Min": 4,
                            "Mean": 488,
                            "Max": 999,
                            "RPS": 6,
                            "Percent50": 491,
                            "Percent75": 738,
                            "Percent95": 971,
                            "Percent99": 980,
                            "Percent999": 999,
                            "StdDev": 295,
                            "MinDataKb": 878.28,
                            "MeanDataKb": 1070176.62,
                            "MaxDataKb": 2090447.38,
                            "AllDataMB": 335171.9949951172
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 552,
                        "More800Less1200": 143,
                        "More1200": 0
                    },
                    "Duration": "00:00:49.9997722"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 991,
                    "OkCount": 860,
                    "FailCount": 131,
                    "AllDataMB": 845964.782087326,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 991,
                            "OkCount": 860,
                            "FailCount": 131,
                            "Min": 3,
                            "Mean": 498,
                            "Max": 1000,
                            "RPS": 14,
                            "Percent50": 501,
                            "Percent75": 730,
                            "Percent95": 937,
                            "Percent99": 950,
                            "Percent999": 999,
                            "StdDev": 283,
                            "MinDataKb": 664.73,
                            "MeanDataKb": 1004923.32,
                            "MaxDataKb": 2095465.38,
                            "AllDataMB": 845964.782087326
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 704,
                        "More800Less1200": 154,
                        "More1200": 0
                    },
                    "Duration": "00:00:59.9996897"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 1011,
                    "OkCount": 860,
                    "FailCount": 151,
                    "AllDataMB": 862534.6515960693,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 554,
                            "OkCount": 463,
                            "FailCount": 91,
                            "Min": 4,
                            "Mean": 490,
                            "Max": 1000,
                            "RPS": 7,
                            "Percent50": 486,
                            "Percent75": 740,
                            "Percent95": 944,
                            "Percent99": 950,
                            "Percent999": 999,
                            "StdDev": 293,
                            "MinDataKb": 8090.84,
                            "MeanDataKb": 1018921.18,
                            "MaxDataKb": 2093301.13,
                            "AllDataMB": 457308.7902498245
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 457,
                            "OkCount": 397,
                            "FailCount": 60,
                            "Min": 4,
                            "Mean": 502,
                            "Max": 999,
                            "RPS": 6,
                            "Percent50": 509,
                            "Percent75": 764,
                            "Percent95": 968,
                            "Percent99": 980,
                            "Percent999": 999,
                            "StdDev": 295,
                            "MinDataKb": 878.28,
                            "MeanDataKb": 1040927.43,
                            "MaxDataKb": 2090447.38,
                            "AllDataMB": 405225.8613462448
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 681,
                        "More800Less1200": 178,
                        "More1200": 0
                    },
                    "Duration": "00:00:59.9996897"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 1192,
                    "OkCount": 1023,
                    "FailCount": 169,
                    "AllDataMB": 1004828.5732049942,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 1192,
                            "OkCount": 1023,
                            "FailCount": 169,
                            "Min": 0,
                            "Mean": 497,
                            "Max": 1001,
                            "RPS": 14,
                            "Percent50": 499,
                            "Percent75": 730,
                            "Percent95": 939,
                            "Percent99": 950,
                            "Percent999": 999,
                            "StdDev": 283,
                            "MinDataKb": 664.73,
                            "MeanDataKb": 1004440.06,
                            "MaxDataKb": 2095465.38,
                            "AllDataMB": 1004828.5732049942
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 833,
                        "More800Less1200": 188,
                        "More1200": 0
                    },
                    "Duration": "00:01:09.9995767"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 1218,
                    "OkCount": 1044,
                    "FailCount": 174,
                    "AllDataMB": 1036091.753276825,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 665,
                            "OkCount": 559,
                            "FailCount": 106,
                            "Min": 4,
                            "Mean": 497,
                            "Max": 1000,
                            "RPS": 8,
                            "Percent50": 488,
                            "Percent75": 753,
                            "Percent95": 947,
                            "Percent99": 950,
                            "Percent999": 999,
                            "StdDev": 293,
                            "MinDataKb": 613.94,
                            "MeanDataKb": 1003464.73,
                            "MaxDataKb": 2093301.13,
                            "AllDataMB": 544374.0530261993
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 553,
                            "OkCount": 485,
                            "FailCount": 68,
                            "Min": 4,
                            "Mean": 493,
                            "Max": 999,
                            "RPS": 7,
                            "Percent50": 508,
                            "Percent75": 748,
                            "Percent95": 966,
                            "Percent99": 950,
                            "Percent999": 999,
                            "StdDev": 292,
                            "MinDataKb": 878.28,
                            "MeanDataKb": 1033160.69,
                            "MaxDataKb": 2090447.38,
                            "AllDataMB": 491717.7002506256
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 834,
                        "More800Less1200": 209,
                        "More1200": 0
                    },
                    "Duration": "00:01:09.9995767"
                }
            ],
            [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 1393,
                    "OkCount": 1196,
                    "FailCount": 197,
                    "AllDataMB": 1184548.7666072845,
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 1393,
                            "OkCount": 1196,
                            "FailCount": 197,
                            "Min": 0,
                            "Mean": 497,
                            "Max": 1001,
                            "RPS": 14,
                            "Percent50": 500,
                            "Percent75": 735,
                            "Percent95": 940,
                            "Percent99": 960,
                            "Percent999": 999,
                            "StdDev": 282,
                            "MinDataKb": 664.73,
                            "MeanDataKb": 1013252.68,
                            "MaxDataKb": 2095897.12,
                            "AllDataMB": 1184548.7666072845
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 975,
                        "More800Less1200": 219,
                        "More1200": 0
                    },
                    "Duration": "00:01:20"
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 1413,
                    "OkCount": 1205,
                    "FailCount": 208,
                    "AllDataMB": 1210365.974931717,
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 768,
                            "OkCount": 648,
                            "FailCount": 120,
                            "Min": 4,
                            "Mean": 500,
                            "Max": 1000,
                            "RPS": 8,
                            "Percent50": 486,
                            "Percent75": 755,
                            "Percent95": 951,
                            "Percent99": 970,
                            "Percent999": 999,
                            "StdDev": 292,
                            "MinDataKb": 613.94,
                            "MeanDataKb": 1021557.82,
                            "MaxDataKb": 2093301.13,
                            "AllDataMB": 643995.0788402557
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 645,
                            "OkCount": 557,
                            "FailCount": 88,
                            "Min": 4,
                            "Mean": 494,
                            "Max": 999,
                            "RPS": 6,
                            "Percent50": 506,
                            "Percent75": 759,
                            "Percent95": 956,
                            "Percent99": 970,
                            "Percent999": 999,
                            "StdDev": 293,
                            "MinDataKb": 878.28,
                            "MeanDataKb": 1036609.94,
                            "MaxDataKb": 2090447.38,
                            "AllDataMB": 566370.8960914612
                        }
                    ],
                    "LatencyCount": {
                        "Less800": 952,
                        "More800Less1200": 252,
                        "More1200": 0
                    },
                    "Duration": "00:01:20"
                }
            ]
        ]
    }
);
