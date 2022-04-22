import Vue from "vue";
import InputField from "@scripts/Components/InputField.vue";

import VueModal from "@kouts/vue-modal";
import "@kouts/vue-modal/dist/vue-modal.css";
Vue.component("Modal", VueModal);

Vue.component("input-field", InputField)