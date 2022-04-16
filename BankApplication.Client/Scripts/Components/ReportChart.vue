<script>
import { Line, mixins } from "vue-chartjs";

export default {
    extends: Line,
    props: {
        chartData: {
            type: Object,
            default: null,
        },
    },
    mounted() {
        this.renderChart(this.chartData, {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                yAxes: [
                    {
                        ticks: {
                            suggestedMin: 50,
                            suggestedMax: 100,
                        },
                    },
                ],
            },
        });
    },
};
</script>
