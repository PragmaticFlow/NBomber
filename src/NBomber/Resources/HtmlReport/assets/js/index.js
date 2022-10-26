const toKb = bytes => +(bytes / 1024.0).toFixed(3);

const toMb = bytes => +(bytes / 1024.0 / 1024.0).toFixed(4);

const getRequestCount = stats => stats.Ok.Request.Count + stats.Fail.Request.Count
const getAllBytes = stats => stats.Ok.DataTransfer.AllBytes + stats.Fail.DataTransfer.AllBytes
const isFailStatsExist = scenarioStats => scenarioStats.StepStats.some(x => x.Fail.Request.Count > 0)

const initApp = (appContainer, viewModel) => {
    // Chart data utilities
    const createScenarioData = (timeLineHistory, scenarioName) =>
        timeLineHistory
            .filter(t => t.ScenarioStats.findIndex(x => x.ScenarioName === scenarioName) >= 0)
            .map(t => ({
                Duration: t.Duration,
                ScenarioStats: t.ScenarioStats.find(x => x.ScenarioName === scenarioName)
            }));

    const createStepData = (timeLineHistory, scenarioName, stepName) =>
        createScenarioData(timeLineHistory, scenarioName)
            .filter(t => t.ScenarioStats.StepStats.findIndex(x => x.StepName === stepName) >= 0)
            .map(t => ({
                Duration: t.Duration,
                ScenarioStats: t.ScenarioStats,
                StepStats: t.ScenarioStats.StepStats.find(x => x.StepName === stepName)
            }));

    // Titles
    const titles = {
        charts: {
            scenarioRequests: 'requests number',
            scenarioStatusCodes: 'status codes',
            latencyIndicators: 'latency indicators',
            throughput: 'throughput',
            latency: 'latency',
            latencyDistribution: 'latency distribution',
            dataTransfer: 'data transfer'
        },
        axisX: {
            duration: 'duration',
            latency: 'latency'
        },
        axisY: {
            requests: 'requests',
            latency: 'latency, requests',
            loadSimulation: 'load simulation',
            rps: 'RPS',
            responseTime: 'response time, ms',
            dataKb: 'data, KB',
            dataMb: 'data, MB'
        },
        series: {
            ok: 'ok',
            failed: 'failed',
            loadSimulation: 'load',
            rps: 'RPS',
            latency: {
                low: 't < 800ms',
                medium: '800ms < t < 1.2s',
                high: 't > 1.2s',
                min: 'min',
                mean: 'mean',
                max: 'max',
                percentile50: '50%',
                percentile75: '75%',
                percentile95: '95%',
                percentile99: '99%',
                stdDev: 'stddev'
            },
            data: {
                min: 'min',
                mean: 'mean',
                max: 'max',
                percentile50: '50%',
                percentile75: '75%',
                percentile95: '95%',
                percentile99: '99%',
                stdDev: 'stddev'
            }

        }
    };

    // Theme
    const theme = {
        colors: {
            yAxis: '#212121', //elegant-color-dark
            xAxis: '#212121', //elegant-color-dark
            stats: {
                okCount: '#00b74a', // success
                failedCount: '#ff3547', // danger
                latency: {
                    low: '#00b74a', // success
                    medium: '#ffea00', // yellow accent-3
                    high: '#ffa900' // warning
                }
            }
        },
        chart: {
            title: {
                fontSize: 14
            },
            legend: {
                fontSize: 12
            }
        }
    };

    // Vue Components
    Vue.component('no-data', {
        template: "#no-data-template"
    });

    Vue.component('scenario-header', {
        props: ['scenarioIndex', 'scenarioName'],
        template: '#scenario-header-template'
    });

    Vue.component('node-info-table', {
        props: ['nodeInfo'],
        template: '#node-info-table-template'
    });

    Vue.component('scenario-stats', {
        props: ['scenarioStats'],
        template: '#scenario-stats-template'
    });

    Vue.component('scenario-stats-table', {
        props: ['scenarioStats', 'failStats'],
        template: '#scenario-stats-table-template',
        data: function () {
            return {
                statsName: this.failStats ? "Fail" : "Ok"
            }
        }
    });

    Vue.component('scenario-stats-requests-number-table', {
        props: ['scenarioStats', 'failStats'],
        template: '#scenario-stats-requests-number-table-template',
        data: function () {
            return {
                statsName: this.failStats ? "Fail" : "Ok"
            }
        }
    });

    Vue.component('scenario-stats-latency-table', {
        props: ['scenarioStats', 'failStats'],
        template: '#scenario-stats-latency-table-template',
        data: function () {
            return {
                statsName: this.failStats ? "Fail" : "Ok"
            }
        }
    });

    Vue.component('scenario-stats-data-transfer-table', {
        props: ['scenarioStats', 'failStats'],
        template: '#scenario-stats-data-transfer-table-template',
        data: function () {
            return {
                statsName: this.failStats ? "Fail" : "Ok"
            }
        }
    });

    const convertPluginStats = pluginsStats =>
        pluginsStats.reduce((acc, dataset) => {
            Object.entries(dataset.Tables).forEach(([key, value]) => {
                const name = key;
                const table = value;
                const cols = table.Columns.map(col => col.Caption)
                const colNames = table.Columns.map(col => col.ColumnName)
                const rows = table.Rows.map(row => colNames.map(colName => row[colName]));

                acc.push({name, cols, rows});
            });
            return acc;
        }, []);

    Vue.component('plugins-stats-table', {
        props: ['pluginsStats'],
        template: '#plugins-stats-table-template',
        data: function() {
            const tables = convertPluginStats(this.pluginsStats || [])

            return {
                tables
            };
        }
    });

    Vue.component('status-codes', {
        props: ['stepStats', 'showCharts'],
        template: '#status-codes-template',
        data: function() {
            const allOkStatuses = this.stepStats.flatMap(x => x.Ok.StatusCodes);
            const allFailStatuses = this.stepStats.flatMap(x => x.Fail.StatusCodes);

            const okReqCount = this.stepStats.reduce((acc, val) => acc + val.Ok.Request.Count, 0);
            const failReqCount = this.stepStats.reduce((acc, val) => acc + val.Fail.Request.Count, 0);

            const okStatusCodesCount = allOkStatuses.reduce((acc, val) => acc + val.Count, 0);
            const failStatusCodesCount = allFailStatuses.reduce((acc, val) => acc + val.Count, 0);

            const okNotAvailableStatusCodes = [];
            const failNotAvailableStatusCodes = [];

            if (okReqCount > okStatusCodesCount) {
                okNotAvailableStatusCodes.push({
                    StatusCode: 'ok (no status)',
                    IsError: false,
                    Message: '',
                    Count: okReqCount - okStatusCodesCount
                })
            }

            if (failReqCount > failStatusCodesCount) {
                failNotAvailableStatusCodes.push({
                    StatusCode: 'fail (no status)',
                    IsError: true,
                    Message: '',
                    Count: failReqCount - failStatusCodesCount
                })
            }

            const allStatusCodes =
                okNotAvailableStatusCodes
                    .concat(allOkStatuses)
                    .concat(failNotAvailableStatusCodes)
                    .concat(allFailStatuses);

            return {
                allStatusCodes
            }
        }
    });

    Vue.component('hints-table', {
        props: ['hints'],
        template: '#hints-table-template'
    });

    Vue.component('lazy-load', {
        props: ['loadedViews', 'loadForView'],
        template: '<div><slot v-if="loadedViews[loadForView]"></slot></div>'
    });

    Vue.component('chart', {
        props: ['type', 'data', 'options'],
        template: '<div ref="container"></div>',
        mounted() {
            google.charts.load('current', {'packages':['corechart', 'bar']});
            google.charts.setOnLoadCallback(() => {
                const chart = new google.visualization[this.type](this.$refs.container);
                const dataTable = google.visualization.arrayToDataTable(this.data);
                const draw = () => chart.draw(dataTable, this.options);
                window.addEventListener('resize', () => setTimeout(draw, 0));
                draw();
            });
        }
    });

    Vue.component('chart-scenario-requests', {
        props: ['scenarioStats'],
        template: '<chart class="chart chart-scenario-requests" :data="data" :options="options" type="PieChart"></chart>',
        data: function() {
            return {
                data: [
                    ['name', 'value'],
                    [titles.series.ok,  this.scenarioStats.Ok.Request.Count],
                    [titles.series.failed, this.scenarioStats.Fail.Request.Count]
                ],
                options: {
                    title : titles.charts.scenarioRequests,
                    titleTextStyle: theme.chart.title,
                    legend: { position: 'bottom', alignment: 'center', textStyle: theme.chart.legend },
                    colors: [theme.colors.stats.okCount, theme.colors.stats.failedCount]
                }
            }
        }
    });

    Vue.component('chart-status-codes', {
        props: ['status-codes'],
        template: '<chart class="chart chart-scenario-status-codes" :data="data" :options="options" type="PieChart"></chart>',
        data: function() {
            const header =  [['name', 'value']];
            const rows = this.statusCodes.map(statusCode => [statusCode.StatusCode.toString(), statusCode.Count]);
            const data = header.concat(rows);

            return {
                data,
                options: {
                    title : titles.charts.scenarioStatusCodes,
                    titleTextStyle: theme.chart.title,
                    legend: { position: 'bottom', alignment: 'center', textStyle: theme.chart.legend }
                }
            }
        }
    });

    Vue.component('chart-scenario-latency-indicators', {
        props: ['scenarioStats'],
        template: '<chart class="chart chart-scenario-latency-indicators" :data="data" :options="options" type="ColumnChart"></chart>',
        data: function() {
            const lessOrEq800 = this.scenarioStats.Ok.Latency.LatencyCount.LessOrEq800 + this.scenarioStats.Fail.Latency.LatencyCount.LessOrEq800
            const more800Less1200 = this.scenarioStats.Ok.Latency.LatencyCount.More800Less1200 + this.scenarioStats.Fail.Latency.LatencyCount.More800Less1200
            const moreOrEq1200 = this.scenarioStats.Ok.Latency.LatencyCount.MoreOrEq1200 + this.scenarioStats.Fail.Latency.LatencyCount.MoreOrEq1200
            const count = this.scenarioStats.Ok.Request.Count + this.scenarioStats.Fail.Request.Count

            return {
                data: [
                    ['name', 'value', { role: 'style' }],
                    [titles.series.latency.low, lessOrEq800, theme.colors.stats.latency.low],
                    [titles.series.latency.medium, more800Less1200, theme.colors.stats.latency.medium],
                    [titles.series.latency.high, moreOrEq1200, theme.colors.stats.latency.high],
                    [titles.series.failed, count, theme.colors.stats.failedCount]
                ],
                options: {
                    title : titles.charts.latencyIndicators,
                    titleTextStyle: theme.chart.title,
                    hAxis: {title: titles.axisX.latency},
                    vAxis: {title: titles.axisY.requests},
                    legend: { position: 'none' },
                    bar: {groupWidth: '25%'},
                    animation: {
                        startup: true,
                        duration: 500,
                        easing: 'out'
                    }
                }
            }
        }
    });

    Vue.component('chart-scenario-latency-distribution', {
        props: ['timeLineHistory', 'scenarioName'],
        template: '<chart class="chart chart-timeline chart-timeline-scenario-load" :data="data" :options="options" type="ComboChart"></chart>',
        data: function() {
            const header = [[titles.axisX.duration, titles.series.loadSimulation, titles.series.latency.low, titles.series.latency.medium, titles.series.latency.high]];
            const timeLineHistory = createScenarioData(this.timeLineHistory, this.scenarioName);
            const rows = timeLineHistory
                .map((t, i) => {
                    let latencyCount = t.ScenarioStats.Ok.Latency.LatencyCount;
                    latencyCount.LessOrEq800 += t.ScenarioStats.Fail.Latency.LatencyCount.LessOrEq800;
                    latencyCount.More800Less1200 += t.ScenarioStats.Fail.Latency.LatencyCount.More800Less1200;
                    latencyCount.MoreOrEq1200 += t.ScenarioStats.Fail.Latency.LatencyCount.MoreOrEq1200;

                    const result = [
                        t.Duration,
                        t.ScenarioStats.LoadSimulationStats.Value,
                        latencyCount.LessOrEq800,
                        latencyCount.More800Less1200,
                        latencyCount.MoreOrEq1200
                    ];

                    return result;
                });

            const data = header.concat(rows);

            return {
                data,
                options: {
                    title : titles.charts.latencyDistribution,
                    titleTextStyle: theme.chart.title,
                    hAxis: {title: titles.axisX.duration},
                    vAxes: {
                        0: {title: titles.axisY.latency},
                        1: {title: titles.axisY.loadSimulation}
                    },
                    legend: { position: 'bottom', alignment: 'center', textStyle: theme.chart.legend },
                    seriesType: 'bars',
                    isStacked: true,
                    series: {
                        1: {color: theme.colors.stats.latency.low},
                        2: {color: theme.colors.stats.latency.medium},
                        3: {color: theme.colors.stats.latency.high},
                    },
                    animation: {
                        startup: true,
                        duration: 500,
                        easing: 'out'
                    }
                }
            }
        }
    });

    Vue.component('chart-timeline-step-throughput', {
        props: ['timeLineHistory', 'scenarioName', 'stepName'],
        template: '<chart class="chart chart-timeline chart-timeline-step-throughput" :data="data" :options="options" type="LineChart"></chart>',
        data: function() {
            const header = [[titles.axisX.duration, titles.series.loadSimulation, titles.series.rps]];

            const rows =
                createStepData(this.timeLineHistory, this.scenarioName, this.stepName)
                    .map(x => [
                        x.Duration,
                        x.ScenarioStats.LoadSimulationStats.Value,
                        x.StepStats.Ok.Request.RPS
                    ]);

            const data = header.concat(rows);

            return {
                data,
                options: {
                    title : titles.charts.throughput,
                    hAxis: {title: titles.axisX.duration},
                    vAxes: {
                        0: {title: titles.axisY.rps},
                        1: {title: titles.axisY.loadSimulation}
                    },
                    legend: { position: 'bottom', alignment: 'center', textStyle: theme.chart.legend },
                    series: {
                        0: {type: 'line', targetAxisIndex: 1},
                    },
                    animation: {
                        startup: true,
                        duration: 500,
                        easing: 'out'
                    }
                }
            }
        }
    });

    Vue.component('chart-timeline-step-latency', {
        props: ['timeLineHistory', 'scenarioName', 'stepName'],
        template: '<chart class="chart chart-timeline chart-timeline-step-latency" :data="data" :options="options" type="ComboChart"></chart>',
        data: function() {
            const header = [[
                titles.axisX.duration,
                titles.series.loadSimulation,
                titles.series.latency.percentile50,
                titles.series.latency.percentile75,
                titles.series.latency.percentile99
            ]];

            const rows =
                createStepData(this.timeLineHistory, this.scenarioName, this.stepName)
                    .map(x => [
                        x.Duration,
                        x.ScenarioStats.LoadSimulationStats.Value,
                        x.StepStats.Ok.Latency.Percent50,
                        x.StepStats.Ok.Latency.Percent75,
                        x.StepStats.Ok.Latency.Percent99
                    ]);

            const data = header.concat(rows);

            return {
                data,
                options: {
                    title : titles.charts.latency,
                    titleTextStyle: theme.chart.title,
                    hAxis: {title: titles.axisX.duration},
                    vAxes: {
                        0: {title: titles.axisY.latency},
                        1: {title: titles.axisY.loadSimulation}
                    },
                    legend: { position: 'bottom', alignment: 'center', textStyle: theme.chart.legend },
                    series: {
                        0: {type: 'line', targetAxisIndex: 1},
                    },
                    animation: {
                        startup: true,
                        duration: 500,
                        easing: 'out'
                    }
                }
            }
        }
    });

    Vue.component('chart-timeline-step-data-transfer', {
        props: ['timeLineHistory', 'scenarioName', 'stepName'],
        template: '<chart class="chart chart-timeline chart-timeline-step-data-transfer" :data="data" :options="options" type="ComboChart"></chart>',
        data: function() {
            const header = [[
                titles.axisX.duration,
                titles.series.loadSimulation,
                titles.series.data.percentile50,
                titles.series.data.percentile75,
                titles.series.data.percentile99
            ]];

            const rows =
                createStepData(this.timeLineHistory, this.scenarioName, this.stepName)
                    .map(x => [
                        x.Duration,
                        x.ScenarioStats.LoadSimulationStats.Value,
                        toKb(x.StepStats.Ok.DataTransfer.Percent50),
                        toKb(x.StepStats.Ok.DataTransfer.Percent75),
                        toKb(x.StepStats.Ok.DataTransfer.Percent99)
                    ]);

            const data = header.concat(rows);

            return {
                data,
                options: {
                    title : titles.charts.dataTransfer,
                    titleTextStyle: theme.chart.title,
                    hAxis: {title: titles.axisX.duration},
                    vAxes: {
                        0: {title: titles.axisY.dataKb},
                        1: {title: titles.axisY.loadSimulation}
                    },
                    legend: { position: 'bottom', alignment: 'center', textStyle: theme.chart.legend },
                    series: {
                        0: {type: 'line', targetAxisIndex: 1},
                    },
                    animation: {
                        startup: true,
                        duration: 500,
                        easing: 'out'
                    }
                }
            }
        }
    });

    // Vue Application
    const app = new Vue({
        el: appContainer,
        data: {
            viewModel,
            sideBarActive: true,
            currentView: 'test-suite',
            loadedViews: {},
            showCharts: true
        },
        computed: {
            showChartsSwitcher: function () {
                return this.currentView === 'test-suite';
            }
        },
        methods: {
            toggleSideBar: function () {
                this.sideBarActive = !this.sideBarActive;
            },
            selectView: function (view) {
                this.currentView = view;

                if (!this.loadedViews[view])
                    this.$set(this.loadedViews, view, true);
            }
        }
    });
};
