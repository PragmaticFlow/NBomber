const initApp = (appContainer, viewModel) => {
    // Utilities
    const createSeriesDataLatency = (scenarioStats, titles) => [
        { name: titles.series.latencyLow, y: scenarioStats.LatencyCount.Less800, color: theme.colors.stats.latency.low },
        { name: titles.series.latencyMedium, y: scenarioStats.LatencyCount.More800Less1200, color: theme.colors.stats.latency.medium },
        { name: titles.series.latencyHigh, y: scenarioStats.LatencyCount.More1200, color: theme.colors.stats.latency.high },
        { name: titles.series.failed, y: scenarioStats.FailCount, color: theme.colors.stats.failedCount }
    ];

    const createSeriesDataScenarioStats = (timelineStats, scenarioIndex, statsName) =>
        timelineStats.ScenarioStats.map(scenarios => scenarios[scenarioIndex][statsName]);

    const createSeriesDataScenarioStatsLatency = (timelineStats, scenarioIndex, latencyName) =>
        timelineStats.ScenarioStats.map(scenarios => scenarios[scenarioIndex].LatencyCount[latencyName]);

    const createSeriesDataScenarioStatsLoadSimulation = (timelineStats, scenarioIndex) =>
        timelineStats.ScenarioStats.map(scenarios => scenarios[scenarioIndex].LoadSimulationStats.Value);

    const createSeriesDataStepStats = (timelineStats, scenarioIndex, stepIndex, statsName) =>
        timelineStats.ScenarioStats.map(scenarios => scenarios[scenarioIndex].StepStats[stepIndex][statsName]);

    const createSeriesMarker = () => ({
        symbol: 'circle'
    });

    const createAxisXLatency = (title, color, titles) => ({
        title: {
            text: title,
            style: {
                fontSize: '12px',
                color
            }
        },
        labels: {
            style: {
                fontSize: '10px',
                color
            }
        },
        categories: [
            titles.series.latencyLow,
            titles.series.latencyMedium,
            titles.series.latencyHigh,
            titles.series.failed
        ],
        crosshair: true
    });

    const createAxisXTimeline = (title, color, timelineStats) => ({
        title: {
            text: title,
            style: {
                fontSize: '12px',
                color
            }
        },
        labels: {
            style: {
                fontSize: '12px',
                color
            }
        },
        categories: timelineStats.TimeStamps,
        crosshair: true
    });

    const createAxisY = (title, color, opposite, hidden) => ({
        title: {
            text: title,
            style: {
                fontSize: '14px',
                fontWeight: 'bold',
                color,
            }
        },
        labels: {
            style: {
                fontSize: '12px',
                fontWeight: 'bold',
                color
            }
        },
        opposite,
        allowDecimals: false,
        visible: !hidden
    });

    const createPlotOptions = () => ({
        series: {
            marker: {
                enabled: false
            },
            cursor: 'pointer',
        },
        area: {
            opacity: 1,
            fillOpacity: 0.2
        },
        pie: {
            cursor: 'pointer',
            dataLabels: {
                enabled: false
            },
            point: {
                events: {
                    legendItemClick: function () {
                        return false;
                    }
                }
            },
            showInLegend: true
        },
        column: {
            maxPointWidth: 25
        }
    });

    const createPlotOptionsLatencyDistribution = () => {
        const plotOptions = createPlotOptions();
        plotOptions.groupPadding = 0;
        plotOptions.series.stacking = 'normal';
        return plotOptions;
    };

    const createTooltip = () => ({
        shared: true
    });

    const createTooltipLoadSimulation = (timelineStats, scenarioIndex) => ({
        pointFormatter: function (tooltip) {
            const loadSimulationName = timelineStats.ScenarioStats[this.index][scenarioIndex].LoadSimulationStats.SimulationName;
            return `<span style="color:${this.color}">●</span> ${this.series.name}: <b>${loadSimulationName} ${this.y}</b>`;
        }
    });

    const createSettingsChartScenarioRequests = (theme, scenarioStats, titles) => ({
        credits: {
            enabled: false
        },
        title: {
            text: titles.charts.scenarioRequests
        },
        tooltip: {
            pointFormat: '<span style="color:{point.color}">●</span> {series.name}: <b>{point.y}</b> ({point.percentage:.1f}%)'
        },
        plotOptions: createPlotOptions(),
        legend: {
            labelFormat: '{name}: {y}',
            align: 'right',
            verticalAlign:'middle',
            padding: 0
        },
        series: [
            {
                type: 'pie',
                name: 'requests',
                colorByPoint: true,
                size: '100%',
                innerSize: '50%',
                data: [
                    {
                        name: titles.series.ok,
                        y: scenarioStats.OkCount,
                        color: theme.colors.stats.okCount
                    }, {
                        name: titles.series.failed,
                        y: scenarioStats.FailCount,
                        color: theme.colors.stats.failedCount
                    }
                ]
            }
        ]
    });

    const errorColors = [
        '#ff3547', '#ff1744', '#ff5252',
        '#c51162', '#f50057', '#ff4081',
        '#ff6d00', '#ff9100', '#ffab40',
        '#aa00ff', '#d500f9', '#e040fb'
    ];

    const createSettingsChartScenarioErrors = (theme, errorStats, titles) => ({
        credits: {
            enabled: false
        },
        title: {
            text: titles.charts.scenarioErrors
        },
        tooltip: {
            pointFormat: '<span style="color:{point.color}">●</span> {series.name}: <b>{point.y}</b> ({point.percentage:.1f}%)'
        },
        plotOptions: createPlotOptions(),
        legend: {
            labelFormat: '{name}: {y}',
            align: 'right',
            verticalAlign:'middle',
            padding: 0
        },
        series: [
            {
                type: 'pie',
                name: 'errors',
                colorByPoint: true,
                size: '100%',
                innerSize: '50%',
                data: errorStats.map((es, i) => ({
                    name: es.ErrorCode,
                    y: es.Count,
                    color: errorColors[i % errorColors.length]
                }))
            }
        ]
    });

    const createSettingsChartScenarioLatencyIndicators = (theme, scenarioStats, titles) => ({
        credits: {
            enabled: false
        },
        title: {
            text: titles.charts.latencyIndicators
        },
        xAxis: createAxisXLatency(titles.axisX.latency, theme.colors.xAxis, titles),
        yAxis: createAxisY(titles.axisY.requests, theme.colors.yAxis, false),
        tooltip: {
            pointFormatter: function () {
                if (this.index === 3) // failed
                    return `<span style="color:${this.color}">●</span> failed: <b>${this.y}</b> requests`;
                return `<span style="color:${this.color}">●</span> ${this.series.name}: <b>${this.y} requests</b>`
            }
        },
        plotOptions: createPlotOptions(),
        legend: {
            enabled: false
        },
        series: [
            {
                type: 'column',
                name: titles.series.latency,
                data: createSeriesDataLatency(scenarioStats, titles)
            }
        ]
    });

    const createSettingsChartScenarioLatencyDistribution = (theme, timelineStats, scenarioIndex, titles) => ({
        credits: {
            enabled: false
        },
        title: {
            text: 'latency distribution'
        },
        yAxis: [
            createAxisY(titles.axisY.latency, theme.colors.xAxis, false),
            createAxisY(titles.axisY.loadSimulation, theme.colors.stats.loadSimulation, true),
        ],
        xAxis: createAxisXTimeline(titles.axisX.duration, theme.colors.xAxis, timelineStats),
        tooltip: createTooltip(),
        plotOptions: createPlotOptionsLatencyDistribution(),
        series: [
            {
                type: 'column',
                name: titles.series.latencyLow,
                color: theme.colors.stats.latency.low,
                tooltip: {
                    valueSuffix: ' requests'
                },
                data: createSeriesDataScenarioStatsLatency(timelineStats, scenarioIndex, 'Less800')
            },
            {
                type: 'column',
                name: titles.series.latencyMedium,
                color: theme.colors.stats.latency.medium,
                tooltip: {
                    valueSuffix: ' requests'
                },
                data: createSeriesDataScenarioStatsLatency(timelineStats, scenarioIndex, 'More800Less1200')
            },
            {
                type: 'column',
                name: titles.series.latencyHigh,
                color: theme.colors.stats.latency.high,
                tooltip: {
                    valueSuffix: ' requests'
                },
                data: createSeriesDataScenarioStatsLatency(timelineStats, scenarioIndex, 'More1200')
            },
            {
                type: 'column',
                name: titles.series.failed,
                color: theme.colors.stats.failedCount,
                tooltip: {
                    valueSuffix: ' requests'
                },
                data: createSeriesDataScenarioStats(timelineStats, scenarioIndex, 'FailCount')
            },
            {
                name: titles.series.loadSimulation,
                yAxis: 1,
                zIndex: 1,
                marker: createSeriesMarker(),
                color: theme.colors.stats.loadSimulation,
                tooltip: createTooltipLoadSimulation(timelineStats, scenarioIndex),
                data: createSeriesDataScenarioStatsLoadSimulation(timelineStats, scenarioIndex)
            }
        ]
    });

    const createSettingsChartStepThroughput = (theme, timelineStats, scenarioIndex, stepIndex, titles) => ({
        credits: {
            enabled: false
        },
        title: {
            text: titles.charts.throughput
        },
        yAxis: [
            createAxisY(titles.axisY.rps, theme.colors.stats.rps, false),
            createAxisY(titles.axisY.loadSimulation, theme.colors.stats.loadSimulation, true)
        ],
        xAxis: createAxisXTimeline(titles.axisX.duration, theme.colors.xAxis, timelineStats),
        tooltip: createTooltip(),
        plotOptions: createPlotOptions(),
        series: [
            {
                name: titles.series.rps,
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.rps,
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'RPS')
            },
            {
                name: titles.series.loadSimulation,
                yAxis: 1,
                zIndex: 1,
                marker: createSeriesMarker(),
                color: theme.colors.stats.loadSimulation,
                tooltip: createTooltipLoadSimulation(timelineStats, scenarioIndex),
                data: createSeriesDataScenarioStatsLoadSimulation(timelineStats, scenarioIndex)
            }
        ]
    });

    const createSettingsChartStepLatency = (theme, timelineStats, scenarioIndex, stepIndex, titles) => ({
        credits: {
            enabled: false
        },
        title: {
            text: titles.charts.latency
        },
        yAxis: [
            createAxisY('response time, ms', theme.colors.yAxis, false),
            createAxisY('data, MB', theme.colors.stats.allDataMB, true, true),
            createAxisY(titles.axisY.loadSimulation, theme.colors.stats.loadSimulation, true)
        ],
        xAxis: createAxisXTimeline(titles.axisX.duration, theme.colors.xAxis, timelineStats),
        tooltip: createTooltip(),
        plotOptions: createPlotOptions(),
        series: [
            {
                name: 'min',
                type: 'area',
                color: theme.colors.stats.min,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'Min')
            }, {
                name: 'mean',
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.mean,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'Mean')
            }, {
                name: 'max',
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.max,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'Max')
            }, {
                name: 'stdDev',
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.stdDev,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'StdDev')
            }, {
                name: '50%',
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.percentile75,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'Percent50')
            }, {
                name: '75%',
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.percentile75,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'Percent75')
            }, {
                name: '95%',
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.percentile95,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'Percent95')
            }, {
                name: '99%',
                type: 'area',
                marker: createSeriesMarker(),
                color: theme.colors.stats.percentile99,
                tooltip: {
                    valueSuffix: ' ms'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'Percent99')
            },
            {
                name: 'data',
                yAxis: 1,
                zIndex: 1,
                marker: createSeriesMarker(),
                color: theme.colors.stats.allDataMB,
                tooltip: {
                    valueSuffix: ' MB'
                },
                data: createSeriesDataStepStats(timelineStats, scenarioIndex, stepIndex, 'AllDataMB')
            },
            {
                name: titles.series.loadSimulation,
                yAxis: 2,
                zIndex: 2,
                marker: createSeriesMarker(),
                color: theme.colors.stats.loadSimulation,
                tooltip: createTooltipLoadSimulation(timelineStats, scenarioIndex),
                data: createSeriesDataScenarioStatsLoadSimulation(timelineStats, scenarioIndex)
            }
        ]
    });

    const renderChart = (container, settings) => setTimeout(() => Highcharts.chart(container, settings), 0);

    // Titles
    const titles = {
        charts: {
            scenarioRequests: 'requests number',
            scenarioErrors: 'error codes',
            latencyIndicators: 'latency indicators',
            throughput: 'throughput',
            latency: 'latency'
        },
        axisX: {
            duration: 'duration',
            latency: 'latency'
        },
        axisY: {
            requests: 'requests',
            latency: 'latency, requests',
            loadSimulation: 'load simulation',
            rps: 'RPS'
        },
        series: {
            latencyLow: 't < 800ms',
            latencyMedium: '800ms < t < 1.2s',
            latencyHigh: 't > 1.2s',
            ok: 'ok',
            failed: 'failed',
            loadSimulation: 'load simulation',
            latency: 'latency',
            rps: 'RPS'
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
                rps: '#1565c0', // blue darken-3
                allDataMB: '#4e342e', // brown darken-3
                min: '#2e7d32', // green darken-3
                mean: '#9e9d24', // lime darken-3
                max: '#ff3d00', // deep-orange  darken-3
                stdDev: '#558b2f', // light-green darken-3
                percentile75: '#0277bd', // light-blue darken-3
                percentile95: '#283593', // indigo darken-3
                percentile99: '#6a1b9a', // purple darken-3
                loadSimulation: '#ff8f00', // amber darken-3
                latency: {
                    low: '#00b74a', // success
                    medium: '#ffea00', // yellow accent-3
                    high: '#ffa900' // warning
                }
            }
        }
    };

    // Vue Components
    Vue.component('plugin-card', {
        template: "#plugin-card-template"
    });

    Vue.component('no-data', {
        template: "#no-data-template"
    });

    Vue.component('scenario-header', {
        props: ['scenarioStats', 'scenarioIndex'],
        template: '#scenario-header-template'
    });

    Vue.component('node-info-table', {
        props: ['nodeInfo'],
        template: '#node-info-table-template'
    });

    Vue.component('scenario-stats-table', {
        props: ['scenarioStats'],
        template: '#scenario-stats-table-template'
    });

    Vue.component('scenario-stats-requests-number-table', {
        props: ['scenarioStats'],
        template: '#scenario-stats-requests-number-table-template'
    });

    Vue.component('scenario-stats-latency-table', {
        props: ['scenarioStats'],
        template: '#scenario-stats-latency-table-template'
    });

    Vue.component('scenario-stats-data-transfer-table', {
        props: ['scenarioStats'],
        template: '#scenario-stats-data-transfer-table-template'
    });

    Vue.component('plugins-stats-table', {
        props: ['pluginsStats'],
        template: '#plugins-stats-table-template'
    });


    Vue.component('error-stats-table', {
        props: ['errorStats', 'maxErrorsNumber'],
        template: '#error-stats-table-template',
        data: function() {
            return {
                showAll: false
            }
        },
        computed: {
            errors: function() {
                return this.showAll || this.errorStats.length <= this.maxErrorsNumber
                    ? this.errorStats
                    : this.errorStats.slice(0, this.maxErrorsNumber);
            },
            shouldBeLimited: function() {
                return this.errorStats.length > this.maxErrorsNumber;
            }
        },
        methods: {
            showAllErrors: function (showAll) {
                this.showAll = showAll;
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

    Vue.component('plugin-chart', {
        props: ['settings'],
        template: '<div ref="container" class="chart chart-plugin"></div>',
        mounted() {
            renderChart(this.$refs.container, this.settings);
        }
    });

    Vue.component('chart-scenario-requests', {
        props: ['scenarioStats'],
        template: '<div ref="container" class="chart chart-scenario-requests"></div>',
        mounted() {
            const settings = createSettingsChartScenarioRequests(theme, this.scenarioStats, titles);
            renderChart(this.$refs.container, settings);
        }
    });

    Vue.component('chart-scenario-errors', {
        props: ['errorStats'],
        template: '<div ref="container" class="chart chart-scenario-errors"></div>',
        mounted() {
            const settings = createSettingsChartScenarioErrors(theme, this.errorStats, titles);
            renderChart(this.$refs.container, settings);
        }
    });

    Vue.component('chart-scenario-latency-indicators', {
        props: ['scenarioStats'],
        template: '<div ref="container" class="chart chart-scenario-latency-col"></div>',
        mounted() {
            const settings = createSettingsChartScenarioLatencyIndicators(theme, this.scenarioStats, titles);
            renderChart(this.$refs.container, settings);
        }
    });

    Vue.component('chart-scenario-latency-distribution', {
        props: ['timelineStats', 'scenarioIndex'],
        template: '<div ref="container" class="chart chart-timeline chart-timeline-scenario-load"></div>',
        mounted() {
            const settings = createSettingsChartScenarioLatencyDistribution(theme, this.timelineStats, this.scenarioIndex, titles);
            renderChart(this.$refs.container, settings);
        }
    });

    Vue.component('chart-timeline-step-throughput', {
        props: ['timelineStats', 'scenarioIndex', 'stepIndex'],
        template: '<div ref="container" class="chart chart-timeline chart-timeline-step-throughput"></div>',
        mounted() {
            const settings = createSettingsChartStepThroughput(theme, this.timelineStats, this.scenarioIndex, this.stepIndex, titles);
            renderChart(this.$refs.container, settings);
        }
    });

    Vue.component('chart-timeline-step-latency', {
        props: ['timelineStats', 'scenarioIndex', 'stepIndex'],
        template: '<div ref="container" class="chart chart-timeline chart-timeline-step-latency"></div>',
        mounted() {
            const settings = createSettingsChartStepLatency(theme, this.timelineStats, this.scenarioIndex, this.stepIndex, titles);
            renderChart(this.$refs.container, settings);
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
        },
        watch: {
            currentView: function (newValue, oldValue) {
                document.querySelector('body')
                    .setAttribute('data-current-view', newValue);
            },
            sideBarActive: function (newValue, oldValue) {
                document.querySelector('body')
                    .setAttribute('data-sidebar-active', newValue);
            }
        }
    });
};
