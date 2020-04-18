var colors = {
	red: 'rgb(255, 99, 132)',
	redlight: 'rgba(255, 99, 132, 0.2)',
	orange: 'rgb(255, 159, 64)',
	yellow: 'rgb(255, 205, 86)',
	yellowlight: 'rgba(255, 206, 86, 0.2)',
	green: 'rgb(75, 192, 192)',
	greenlight: 'rgba(75, 192, 192, 0.2)',
	blue: 'rgb(54, 162, 235)',
	bluelight: 'rgba(54, 162, 235, 0.2)',
	purple: 'rgb(153, 102, 255)',
	grey: 'rgb(201, 203, 207)'
};

Vue.component('custom-card', {
   props: ['title'],
   template: "#custom-card-template"
});

Vue.component('claster-info-table', {
    props: ['machineInfo'],
    template: '#claster-info-table-template'
});

Vue.component('scenarios-stats-table', {
    props: ['scenariosStats'],
    template: '#scenarios-stats-table-template'
});

Vue.component('plugins-stats-table', {
    props: ['pluginsStats'],
    template: '#plugins-stats-table-template'
});

Vue.component('scenarios-stats-view', {
    props: ['scenariosStats', 'pluginsStats'],
    template: '#scenarios-stats-view-template'
});

Vue.component('num-req-chart', {
    extends: VueChartJs.Pie,
    props: ['okCount', 'failCount'],
    mounted () {
        var data = {
            labels: ['Ok', 'Failed'],
            datasets: [
                {
                    data: [this.okCount, this.failCount],
                    backgroundColor: [colors.green, colors.red]
                }
            ]
        };

        this.renderChart(data, {});
    }
});

Vue.component('indicators-chart', {
    extends: VueChartJs.Bar,
    props: ['label', 'latencyCount', 'failCount'],
    mounted () {
        var data = {
            labels: ["t < 800ms", "800ms < t < 1.2s", "t > 1.2s", "failed"],
            datasets: [{
                label: this.label,
                data: [
                    this.latencyCount.Less800,
                    this.latencyCount.More800Less1200,
                    this.latencyCount.More1200,
                    this.failCount
                ],
                backgroundColor: [
                    colors.greenlight,
                    colors.bluelight,
                    colors.yellowlight,
                    colors.redlight,
                ],
                borderColor: [
                    colors.green,
                    colors.blue,
                    colors.yellow,
                    colors.red,
                ],
                borderWidth: 1
            }]
        };

        var options = {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        };

        this.renderChart(data, options)
    }
});

var testStatsData = {
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
};
