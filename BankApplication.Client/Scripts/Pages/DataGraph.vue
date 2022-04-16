<template>
    <div class="graph">
        <div class="row d-flex justify-content-center mt-5">
            <div class="col-6">
                <h5>Last 7 days activation trends</h5>
                <ReportChart v-if="reportData" :chart-data="reportData" />
            </div>
        </div>
    </div>
</template>

<script>
import ReportChart from "@scripts/Components/ReportChart";
export default {
    components: { ReportChart},
    data() {
        return{
            reportData: null,
        }
    },
    mounted() {
        this.getChartData();
    },
    methods: {
        getChartData() {
                 var chartFormatedData = {
                labels: ['label1', 'label2', 'label3', 'label4', 'label5', 'label6','label7'],
                datasets: [{ label: "Daily Activation", data: [30, 59, 80, 81, 56, 55, 40], fill: false, borderColor: '#2263a1', }],
            };
            this.reportData = chartFormatedData
        }
    }
}
</script>




/** -------------------------------------Orginal example get data from DB----------------------------------------------
export default {
    components: { PhoneOrderSummary },
    data() {
        return {
            totalCustomers: 0,
            totalPhoneOrders: 0,
            phoneOrderData: null,
        };
    },
    mounted() {
        ReportsService.GetAllSummary().then((data) => {
            this.totalCustomers = data.totalCustomers;
            this.totalPhoneOrders = data.totalPhoneOrders;
        });
        ReportsService.GetPhoneOrderSummary().then((data) => {
            var reportData = {
                labels: [],
                datasets: [{ label: "Daily Activation", data: [], fill: false, borderColor: '#2263a1', }],
            };
            data.forEach((d) => reportData.labels.push(d.date));
            data.forEach((d) => reportData.datasets[0].data.push(d.count));
            this.phoneOrderData = reportData;
        });
    },
};
</script>
**/

