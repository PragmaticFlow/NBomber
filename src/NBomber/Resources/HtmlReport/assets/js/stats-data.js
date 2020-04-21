function getStatsData() {
    return {
        "MachineInfo": {
            "MachineName": "Machine-Name",
            "Os": "Unix",
            "DotNetVersion": ".NETCoreApp,Version=v3.1",
            "Processor": "n/a",
            "CoresCount": 8
        },
        "NodeStats": {
            "RequestCount": 400,
            "OkCount": 310,
            "FailCount": 90,
            "AllDataMB": 0.0,
            "ScenarioStats": [
                {
                    "ScenarioName": "scenario 1",
                    "RequestCount": 100,
                    "OkCount": 70,
                    "FailCount": 30,
                    "AllDataMB": 19.92,
                    "LatencyCount": {
                        "Less800": 700,
                        "More800Less1200": 1000,
                        "More1200": 1300
                    },
                    "Duration": "00:01:00",
                    "StepStats": [
                        {
                            "StepName": "pull html 1",
                            "RequestCount": 100,
                            "OkCount": 70,
                            "FailCount": 30,
                            "Min": 10,
                            "Mean": 50,
                            "Max": 90,
                            "RPS": 150,
                            "Percent50": 50,
                            "Percent75": 75,
                            "Percent95": 95,
                            "StdDev": 40,
                            "MinDataKb": 17.92,
                            "MeanDataKb": 17.92,
                            "MaxDataKb": 17.92,
                            "AllDataMB": 157.5
                        }
                    ]
                },
                {
                    "ScenarioName": "scenario 2",
                    "RequestCount": 300,
                    "OkCount": 240,
                    "FailCount": 60,
                    "AllDataMB": 11.03,
                    "LatencyCount": {
                        "Less800": 500,
                        "More800Less1200": 1100,
                        "More1200": 1500
                    },
                    "Duration": "00:00:30",
                    "StepStats": [
                        {
                            "StepName": "pull html 2",
                            "RequestCount": 100,
                            "OkCount": 70,
                            "FailCount": 30,
                            "Min": 10,
                            "Mean": 50,
                            "Max": 90,
                            "RPS": 150,
                            "Percent50": 50,
                            "Percent75": 75,
                            "Percent95": 95,
                            "StdDev": 40,
                            "MinDataKb": 17.92,
                            "MeanDataKb": 17.92,
                            "MaxDataKb": 17.92,
                            "AllDataMB": 157.5
                        },
                        {
                            "StepName": "pull html 3",
                            "RequestCount": 200,
                            "OkCount": 170,
                            "FailCount": 30,
                            "Min": 10,
                            "Mean": 50,
                            "Max": 90,
                            "RPS": 150,
                            "Percent50": 50,
                            "Percent75": 75,
                            "Percent95": 95,
                            "StdDev": 40,
                            "MinDataKb": 17.92,
                            "MeanDataKb": 17.92,
                            "MaxDataKb": 17.92,
                            "AllDataMB": 157.5
                        }
                    ]
                }
            ],
            "PluginStats": [
                {
                    "PluginName": "Plugin Stats 1",
                    "Columns": ["Key", "Value"],
                    "Rows": [
                        ["Key 1", "Value 1"],
                        ["Key 2", "Value 2"]
                    ]
                },
                {
                    "PluginName": "Plugin Stats 2",
                    "Columns": ["Property", "Value"],
                    "Rows": [
                        ["Property 1", "Value 1"],
                        ["Property 2", "Value 2"]
                    ]
                }
            ],
            "NodeInfo": {
                "MachineName": "Machine Name",
                "Sender": "SingleNode",
                "CurrentOperation": "None"
            }
        }
    }
}
