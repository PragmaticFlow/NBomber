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

const getColor = index => {
    const colorsArr = Object.values(colors);
    return colorsArr[index % colorsArr.length];
};

Vue.component('no-data', {
    template: "#no-data-template"
});

Vue.component('scenario-header', {
    props: ['scenarioStats'],
    template: "#scenario-header-template"
});

Vue.component('node-info-table', {
    props: ['nodeInfo'],
    template: '#node-info-table-template'
});

Vue.component('scenarios-stats-table', {
    props: ['scenariosStats', 'stepProperties', 'hiddenStepProperties', 'xAxesLabel', 'yAxesLabel', 'viewId', 'collapsable'],
    template: '#scenarios-stats-table-template'
});

Vue.component('plugins-stats-table', {
    props: ['pluginsStats'],
    template: '#plugins-stats-table-template'
});

Vue.component('requests-chart', {
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

        const options = {
            responsive: true,
            maintainAspectRatio: false
        };

        this.renderChart(data, options);
    }
});

Vue.component('latency-chart', {
    extends: VueChartJs.Bar,
    props: ['latencyCount', 'failCount'],
    mounted () {
        const data = {
            labels: ['t < 800ms', '800ms < t < 1.2s', 't > 1.2s', 'Failed'],
            datasets: [{
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
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                xAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: 'Latency'
                    }
                }],
                yAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: 'Requests'
                    },
                    ticks: {
                        beginAtZero: true
                    }
                }]
            },
            legend: {
                display: false
            }
        };

        this.renderChart(data, options);
    }
});

Vue.component('time-line-stats-chart', {
    extends: VueChartJs.Line,
    props: ['stats', 'scenarioName', 'stepProperties', 'hiddenStepProperties', 'xAxesLabel', 'yAxesLabel'],
    mounted () {
        const stepProperties = this.stepProperties || ['RPS'];
        const hiddenStepProperties = this.hiddenStepProperties || [];

        const toFlatArrayReducer = (acc, current) => acc.concat(current);

        const propertyStepStats =
            this.stats.TimeStamps
                .map((timeStamp, i) =>
                    this.stats.ScenarioStats[i]
                        .filter(scnStats => scnStats.ScenarioName === this.scenarioName)
                        .map(scnStats => scnStats.StepStats)
                        .reduce(toFlatArrayReducer, [])
                        .map((stepStats, stepStatsIndex) =>
                            stepProperties.map((propertyName, propertyNameIndex) => ({
                                datasetIndex: `${stepStatsIndex}-${propertyNameIndex}`,
                                stepIndex: stepStatsIndex,
                                timeStamp,
                                propertyName,
                                stepStats
                        })))
                        .reduce(toFlatArrayReducer, []));

        const datasets =
            Object.values(propertyStepStats
                .reduce((acc, current) => {
                    current.map(propertyStep => {
                        const point = {
                            x: propertyStep.timeStamp,
                            y: propertyStep.stepStats[propertyStep.propertyName],
                            stepStats: propertyStep.stepStats
                        };

                        if (propertyStep.datasetIndex in acc) {
                            acc[propertyStep.datasetIndex].data.push(point);
                        } else {
                            acc[propertyStep.datasetIndex] = {
                                label: `${propertyStep.stepStats.StepName} | ${propertyStep.propertyName}`,
                                data: [point],
                                borderColor: [
                                    getColor(Object.keys(acc).length)
                                ],
                                fill: false,
                                borderWidth: 2,
                                hidden: hiddenStepProperties.includes(propertyStep.propertyName) || propertyStep.stepIndex > 0
                            }
                        }
                    });

                    return acc;
                }, {}));

        const data = {
            labels: this.stats.TimeStamps,
            datasets
        };

        const options = {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                xAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: this.xAxesLabel || ''
                    }
                }],
                yAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: this.yAxesLabel || ''
                    }
                }]
            },
            tooltips: {
                position: 'topOfChart',
                callbacks: {
                    label: (tooltipItem, data) => {
                        const dataset = data.datasets[tooltipItem.datasetIndex];
                        const { stepStats } = dataset.data[tooltipItem.index];
                        const lines = [dataset.label].concat(
                            Object.entries(stepStats).map(([key, value]) => `${key} = ${value}`));

                        return lines;
                    }
                }
            }
        };

        this.renderChart(data, options);
    }
});

Chart.Tooltip.positioners.topOfChart = (elements, eventPosition) => ({
    x: eventPosition.x,
    y: 0
});
