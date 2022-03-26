import Vue from "vue"
import Router from "@scripts/Configurations/Routes";
import Application from "@scripts/Application.vue";

new Vue({
    components:{ Application },
    router: Router,
    el: "#application",
});

