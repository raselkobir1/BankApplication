<template>
    <div>
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

    </div>
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
    OnLoadTransactionHistoryView(){
           AccountService.customerTransactionHistory()
        .then((response) => {
          this.transaction = response.data.transactions;
          console.log("Transaction history :", this.transaction);
        })
        .catch((error) => {
          console.log(error);
        })
    }
  },
};
</script>
