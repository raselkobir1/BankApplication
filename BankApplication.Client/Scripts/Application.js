import Vue from "vue"
import Router from "@scripts/Configurations/Routes";
import Application from "@scripts/Application.vue";
import "./Configurations/Axios";
import "@scripts/Configurations/VeeValidate";
import "./Store/Store";
import "@scripts/Configurations/import"

new Vue({
    components:{ Application },
    router: Router,
    el: "#application",
});

