import Vue from "vue"
import Router from "@scripts/Configurations/Routes";
import Application from "@scripts/Application.vue";
import "./Configurations/Axios";
import "./Store/Store";

new Vue({
    components:{ Application },
    router: Router,
    el: "#application",
});

