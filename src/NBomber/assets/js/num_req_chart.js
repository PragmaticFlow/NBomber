$(document).ready(function () {
    var ctx = document.getElementById("num_req_chart").getContext('2d');
    window.numReqChart = new Chart(ctx, {
        type: 'pie',
        data: {
            datasets: [{
                data: %dataArray%,
                backgroundColor: [
                    window.chartColors.green,
                    window.chartColors.red
                ]
            }],
            labels: [
                'Ok',
                'Failed'
            ]
        },
        options: {
            responsive: true
        }
    });
});