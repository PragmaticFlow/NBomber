// const getPluginStats = rawPluginStats => {
//     const result = [];
//
//     rawPluginStats.map(tables => {
//         for (let [tableName, rawRows] of Object.entries(tables)) {
//             if (rawRows.length > 0) {
//                 const firstRow = rawRows[0];
//                 const columns = Object.keys(firstRow);
//                 const rows = rawRows.map(Object.values);
//                 const table = { PluginName: tableName, Columns: columns, Rows: rows };
//                 result.push(table);
//             }
//         }
//     });
//
//     return result;
// };

const colors = {
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

Vue.component('no-data', {
    template: "#no-data-template"
});

Vue.component('custom-card', {
    props: ['title'],
    template: "#custom-card-template"
});

Vue.component('cluster-info-table', {
    props: ['nodeInfo'],
    template: '#cluster-info-table-template'
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
        const data = {
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
        const data = {
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

        const options = {
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
