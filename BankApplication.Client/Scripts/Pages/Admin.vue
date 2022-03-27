<template>
<body>
<div class="page page-admin container-fluid">
  <div class="row">
    <nav class="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse">
      <div class="position-sticky pt-3">
        <ul class="nav flex-column">
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" href="#">
              <span data-feather="home"></span>
              Admin Dashboard
            </a>
          </li>
        </ul>
      </div>
    </nav>

    <main class="col-md-9 ms-sm-auto col-lg-10 px-md-4">
      <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <h1 class="h2">Dashboard</h1>
        <div class="btn-toolbar mb-2 mb-md-0">
          <div class="btn-group me-2">
            <button type="button" @click="OnClickSignOut" class="btn btn-sm btn-outline-secondary">Sign out</button>
          </div>
        </div>
      </div>

      <h2>Customers account list</h2>
      <div class="table-responsive">
        <table class="table table-striped table-sm">
          <thead>
            <tr>
              <th scope="col">SL</th>
              <th scope="col">Account No</th>
              <th scope="col">OP Balance</th>
              <th scope="col">A/C Type</th>
              <th scope="col">Status</th>
              <th scope="col">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(account, index) of accountList" :key="index">
              <td>{{index}}</td>
              <td>{{account.accountNo}}</td>
              <td>{{account.openingBalance}}</td>
              <td>{{account.accountType}}</td>
              <td>{{account.accountStatus == true ? "Active":"Inactive"}}</td>
              <td>
                <button type="button" @click.prevent="OnClickActiveAccount(account.id)" class="btn btn-sm btn-primary">A/C Active</button>
                <button type="button" @click.prevent="OnClickInActiveAccount(account.id)" class="btn btn-sm btn-primary">A/C Inactive</button>
              </td>
            </tr>
          </tbody>
        </table>
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
      accountList: [],
      context: ''
    };
  },
  mounted(){
    this.getCustomerAccounts();
    this.getApplicationContext()
  },
  methods: {
    async getCustomerAccounts() {
        AccountService.getAccounts()
          .then((response) => {
            this.accountList = response.data.accounts
            console.log("Response data :", response.data.accounts);
            //this.$router.push({ name: "admin" });
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
            
          });
    },
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
    OnClickActiveAccount(id) {
      AccountService.accountActivationProcess(id)
        .then((response) => {
          console.log("id for activation account :",id);
          this.getCustomerAccounts();
        })
        .catch((error) => {
          console.log(error);
        })
    },
    OnClickInActiveAccount(id) {
      AccountService.accountInActivationProcess(id)
        .then((response) => {
          console.log("id for Inactivation account :",id);
          this.getCustomerAccounts();
        })
        .catch((error) => {
          console.log(error);
        })
    }
     
  },
};
</script>