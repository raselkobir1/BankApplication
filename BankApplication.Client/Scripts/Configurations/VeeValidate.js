import Vue from "vue";
import VeeValidate from "vee-validate";
import { Validator } from "vee-validate";

Vue.use(VeeValidate, {
    events: "change|blur",
});

Validator.extend(
    "all_property_filled",
    { 
        getMessage(field) {
            return "Please fillup all the fields";
        },
        validate(value) {
            let properties = Object.keys(value);
            let isValid = true;
            properties.forEach((p) => {
                if(value[p] == undefined || !value[p] || value[p]== "") {
                    isValid = false;
                }
            });
            return isValid;
        },
    },
    { 
        immediate: true,
    }
);
