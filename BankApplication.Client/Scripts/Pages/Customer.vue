<template>
<body>
<div class="page page-customer container-fluid">
  <div class="row">
    <nav class="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse">
      <div class="position-sticky pt-3">
        <ul class="nav flex-column">
          <li class="nav-item">
            <a class="nav-link active" @click.prevent="OnClickApplayBankAccount" aria-current="page" href="#">
              <span data-feather="home"></span>
              Applay for Account
            </a>
          </li>
             <li class="nav-item">
            <a class="nav-link active" @click.prevent="OnClickDepositeAmount" aria-current="page" href="#">
              <span data-feather="home"></span>
              Deposite Balance
            </a>
          </li>
             <li class="nav-item">
            <a class="nav-link active" @click.prevent="OnClickWidthdrownAmount" aria-current="page" href="#">
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
      <!-- <div class="card text-white bg-primary mb-3" style="max-width: 18rem;">
        <div class="card-header">Header</div>
        <div class="card-body">
          <h5 class="card-title">Primary card title</h5>
          <p class="card-text">Total Balance</p>
        </div>
    </div> -->
        <h4>Customers account Transaction history</h4>
      <div class="table-responsive">
        <table class="table table-striped table-sm">
          <thead>
            <tr>
              <th scope="col">SL</th>
              <th scope="col">Account No</th>
              <th scope="col">A/C Type</th>
              <th scope="col">Balance</th>
              <th scope="col">Deposite</th>
              <th scope="col">widthdrown</th>
              <th scope="col">Date</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(account, index) of transaction" :key="index">
              <td>{{index}}</td>
              <td>{{account.accNo}}</td>
              <td>{{account.accType}}</td>
              <td>{{account.balance}}</td>
              <td>{{account.deposite}}</td>
              <td>{{account.widthdrown}}</td>
              <td>{{account.date}}</td>
   
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
      context: '',
      transaction:[],
    };
  },
  mounted(){
    this.getApplicationContext(),
    this.OnLoadTransactionHistoryView()
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
     OnClickApplayBankAccount(){
          this.$router.push({ name: "applayBankAccount"});
     },
    OnClickDepositeAmount(){
        this.$router.push({ name: "deposite"});
    },
    OnClickWidthdrownAmount() {
      this.$router.push({ name: "widthdrown"});
    },
    OnLoadTransactionHistoryView(){
           AccountService.customerTransactionHistory()
        .then((response) => {
          this.transaction = response.data.transactions;
          console.log("Transaction history :", this.transaction);

          //this.$router.push({ name: "login"});
        })
        .catch((error) => {
          console.log(error);
        })
    }
  },
};
</script>