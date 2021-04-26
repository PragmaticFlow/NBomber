const toKb = bytes => +(bytes / 1024.0).toFixed(3);

const toMb = bytes => +(bytes / 1024.0 / 1024.0).toFixed(4);

const initApp = (appContainer, viewModel) => {

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
        props: ['statusCodes', 'okCount', 'failCount'],
        template: '#status-codes-template',
        data: function() {
            const okStatusCodes = this.statusCodes.filter(x => !x.IsError);
            const failStatusCodes = this.statusCodes.filter(x => x.IsError);
            const okStatusCodesCount = okStatusCodes.reduce((acc, val) => acc + val.Count, 0);
            const failStatusCodesCount = failStatusCodes.reduce((acc, val) => acc + val.Count, 0);
            const okNotAvailableStatusCodes = [];
            const failNotAvailableStatusCodes = [];

            if (okStatusCodesCount < this.okCount) {
                okNotAvailableStatusCodes.push({
                    StatusCode: 'ok (no status)',
                    IsError: false,
                    Message: '',
                    Count: this.okCount - okStatusCodesCount
                })
            }

            if (failStatusCodesCount < this.failCount) {
                failNotAvailableStatusCodes.push({
                    StatusCode: 'fail (no status)',
                    IsError: true,
                    Message: '',
                    Count: this.failCount - failStatusCodesCount
                })
            }

            const allStatusCodes =
                okNotAvailableStatusCodes
                    .concat(okStatusCodes)
                    .concat(failNotAvailableStatusCodes)
                    .concat(failStatusCodes);

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

    Vue.component('chart-scenario-requests', {
        props: ['scenarioStats'],
        template: '<div ref="container" class="chart chart-scenario-requests"></div>',
        mounted() {

        }
    });

    Vue.component('chart-status-codes', {
        props: ['status-codes'],
        template: '<div ref="container" class="chart chart-scenario-status-codes"></div>',
        mounted() {

        }
    });

    Vue.component('chart-scenario-latency-indicators', {
        props: ['scenarioStats'],
        template: '<div ref="container" class="chart chart-scenario-latency-col"></div>',
        mounted() {

        }
    });

    Vue.component('chart-scenario-latency-distribution', {
        props: ['timelineStats', 'scenarioName'],
        template: '<div ref="container" class="chart chart-timeline chart-timeline-scenario-load"></div>',
        mounted() {

        }
    });

    Vue.component('chart-timeline-step-throughput', {
        props: ['timelineStats', 'scenarioName', 'stepName'],
        template: '<div ref="container" class="chart chart-timeline chart-timeline-step-throughput"></div>',
        mounted() {

        }
    });

    Vue.component('chart-timeline-step-latency', {
        props: ['timelineStats', 'scenarioName', 'stepName'],
        template: '<div ref="container" class="chart chart-timeline chart-timeline-step-latency"></div>',
        mounted() {

        }
    });

    const createTimeLinesStatsForScenario = (timeLineStats, scenarioName) =>
        timeLineStats
            .flatMap(x => x.ScenarioStats)
            .filter(x => x.ScenarioName === scenarioName);

    const timeLineStats =
      viewModel.NodeStats.ScenarioStats
          .map(x => x.ScenarioName)
          .map(x => ({
              scenarioName: x,
              timeLineStats: createTimeLinesStatsForScenario(viewModel.TimeLineStats, x)
          }));

    // Vue Application
    const app = new Vue({
        el: appContainer,
        data: {
            viewModel,
            timeLineStats,
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
