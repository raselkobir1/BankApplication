<template>
<body>
<div class="page page-customer container-fluid">
  <div class="row">
    <nav class="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse">
      <div class="position-sticky pt-3">
        <ul class="nav flex-column">
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" href="#">
              <span data-feather="home"></span>
              Applay for Account
            </a>
          </li>
             <li class="nav-item">
            <a class="nav-link active" aria-current="page" href="#">
              <span data-feather="home"></span>
              Deposite Balance
            </a>
          </li>
             <li class="nav-item">
            <a class="nav-link active" aria-current="page" href="#">
              <span data-feather="home"></span>
              Widthdrown Balance
            </a>
          </li>
           <li class="nav-item">
            <a class="nav-link active" aria-current="page" href="#">
              <span data-feather="home"></span>
              View Statement
            </a>
          </li>
        </ul>
      </div>
    </nav>

    <main class="col-md-9 ms-sm-auto col-lg-10 px-md-4">
      <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <h1 class="h2">Customer Dashboard</h1>
        <div class="btn-toolbar mb-2 mb-md-0">
          <div class="btn-group me-2">
            <button type="button" @click="OnClickSignOut" class="btn btn-sm btn-outline-secondary">Sign out</button>
          </div>
        </div>
      </div>
    </main>
  </div>
</div>
</body>
</template>

<script>
//import LoginModel from "@scripts/Models/Accounts/LoginModel";
import AccountService from "@scripts/Services/AccountServices";

export default {
  data() {
    return {
      context: ''
    };
  },
  mounted(){
    this.getApplicationContext()
  },
  methods: {
    getApplicationContext() {
      AccountService.getApplicationContext()
        .then((response) => {
          this.context = response.data;
          console.log("App context data :",this.context)
        })
        .catch((error) => {
          this.serverError = error;
        });
      },
    OnClickSignOut() {
      AccountService.singOut(this.loginModel)
        .then((response) => {
          this.$router.push({ name: "login"});
        })
        .catch((error) => {
          console.log(error);
        })
     },

     
    },
};
</script>