import Vue from "vue";
import Router from "vue-router";
import PublicLayout from "@scripts/Layouts/PublicLayout.vue";
import Login from "@scripts/Pages/Login.vue";
import Admin from "@scripts/Pages/Admin.vue";
import Customer from "@scripts/Pages/Customer.vue";

Vue.use(Router);

let router = new Router({
    mode: "history",
    routes: [
        {
            path: "/", name: "public-layout", component:PublicLayout,
            children: [
                { path: "login", name:"login", component:Login },
                { path: "admin", name:"admin", component:Admin },
                { path: "customer", name:"customer", component:Customer },
            ]
        }
    ]
});

router.beforeEach((to, from, next) =>{
    console.log("router to from :",to,from);
    next();
})
export default router;