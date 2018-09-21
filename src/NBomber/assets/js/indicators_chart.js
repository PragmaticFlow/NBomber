$(document).ready(function () {
    var ctx = document.getElementById("indicators_chart-%viewId%").getContext('2d');
    window.indicatorsChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ["t < 800ms", "800ms < t < 1.2s", "t > 1.2s", "failed"],
            datasets: [{
                label: "%label%",
                data: %dataArray%,
                backgroundColor: [
                    window.chartColors.greenlight,
                    window.chartColors.bluelight,
                    window.chartColors.yellowlight,
                    window.chartColors.redlight,
                ],
                borderColor: [
                    window.chartColors.green,
                    window.chartColors.blue,
                    window.chartColors.yellow,
                    window.chartColors.red,
                ],
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
});