import Vue from "vue";
import Router from "vue-router";
import PublicLayout from "@scripts/Layouts/PublicLayout.vue";
import Login from "@scripts/Pages/Login.vue";
import Admin from "@scripts/Pages/Admin.vue";
import Customer from "@scripts/Pages/Customer.vue";
import Registration from "@scripts/Pages/Registration.vue";
import ForgotPassword from "@scripts/Pages/ForgotPassword.vue";
import AfterPassReset from "@scripts/Pages/AfterPassReset.vue";
import ResetPassword from "@scripts/Pages/ResetPassword.vue";
import VerifyEmail from "@scripts/Pages/VerifyEmail.vue";
import ApplayBankAccount from "@scripts/Pages/ApplayBankAccount.vue";
import Deposite from "@scripts/Pages/Deposite.vue";
import Widthdrown from "@scripts/Pages/Widthdrown.vue";
import ChangePassword from "@scripts/Pages/ChangePassword.vue";
import TransactionStatement from "@scripts/Pages/TransactionStatement.vue";
import DataGraph from "@scripts/Pages/DataGraph.vue";

import UserAcceptInvitation from "@scripts/Pages/UserAcceptInvitation.vue";



import vuexExample from "@scripts/Pages/RnD/vuexExample.vue";





Vue.use(Router);

let router = new Router({
    mode: "history",
    routes: [
        {
            path: "/", name: "public-layout", component:PublicLayout,
            children: [
                { path: "", name:"login", component:Login },
                { path: "registration", name:"registration", component:Registration },
                { path: "forgotPassword", name:"forgotPassword", component:ForgotPassword },
                { path: "resetPassword", name:"resetPassword", component:ResetPassword },
                { path: "afterPassReset", name:"afterPassReset", component:AfterPassReset },
                { path: "verifyEmail", name:"verifyEmail", component:VerifyEmail },
                { path: "admin", name:"admin", component:Admin },
                { path: "accept-invitation", name:"accept-invitation", component:UserAcceptInvitation },
                { path: "dataGraph", name:"dataGraph", component:DataGraph },
                { path: "customer", name:"customer", component:Customer, 
                redirect: { name: "transactionStatement" },
                    children: [
                        { path: "applayBankAccount", name:"applayBankAccount", component:ApplayBankAccount },
                        { path: "deposite", name:"deposite", component:Deposite },
                        { path: "widthdrown", name:"widthdrown", component:Widthdrown },
                        { path: "transactionStatement", name:"transactionStatement", component:TransactionStatement },
                        { path: "changePassword", name:"changePassword", component:ChangePassword },
                        { path: "vuexExample", name:"vuexExample", component:vuexExample },
                    ]
                },
            ]
        }
    ]
});

router.beforeEach((to, from, next) =>{
    console.log("router to from :",to,from);
    next();
})
export default router;