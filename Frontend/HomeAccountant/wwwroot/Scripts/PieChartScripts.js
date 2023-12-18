let chart;

function CreateChart(chartId, config) {

    if (chart !== undefined && chart !== null) {
        chart.destroy();
    }

    const ctx = document.getElementById(chartId);
    let configObj = JSON.parse(config);
    chart = new Chart(ctx, {
        type: 'pie',
        data: configObj,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            aspectRatio: 1
        }
    });
}