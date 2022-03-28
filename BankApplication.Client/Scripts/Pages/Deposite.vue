<template>
  <div class="page page-deposite">
    <div class="row d-flex justify-content-center">
      <div class="col-4 text-center">
        <form>
          <h1 class="h3 mb-3 fw-normal">Deposite Balance</h1>

          <div class="form-floating">
             <select class="form-control mb-2" v-model="accNo" id="floatingSelect">
                <option v-for="account in acclist" :key="account" :value="account">
                    {{ account.accountNo }}
                </option>
            </select>
            <label for="floatingSelect">Choose an account no</label>
          </div>

          <div class="form-floating">
            <input
              type="number"
              v-model="amount"
              class="form-control mb-2"
              id="floatingAmount"
              placeholder="password"
            />
            <label for="floatingAmount">Amount</label>
          </div>
          <button @click.prevent="onDepositeAmount" class="w-100 btn btn-lg btn-primary mb-3"  type="button">
            Deposite
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
//import LoginModel from "@scripts/Models/Accounts/LoginModel";
import AccountService from "@scripts/Services/AccountServices";

export default {
  data() {
    return {
      accNo: '',
      amount: '',
      acclist: []
    };
  },
  mounted() {
    this.getActiveAccountList();
  },
  methods: {
    async onDepositeAmount() {
        let transactionModel = 
        {
            accountNo: this.accNo.accountNo,
            depositeAmount: this.amount,
            WidthrownAmount: 0,
            transactionType: "Deposite",
        };
        console.log("Request deposite data :", this.accNo.accountNo, this.amount);
        AccountService.transactionAmount(transactionModel)
          .then((response) => {
            console.log("Response data :", response.data);
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
            this.ClearInputField();
          });
    },
    getActiveAccountList() {
         AccountService.getActiveAccountList()
          .then((response) => {
              this.acclist = response.data.acclist;
            console.log("Response data :", response.data.acclist);
          })
          .catch((error) => {
            console.log(error);
          })
      },
    ClearInputField() {
        this.accNo ='',
        this.amount =''
      }
  },
};
</script>
