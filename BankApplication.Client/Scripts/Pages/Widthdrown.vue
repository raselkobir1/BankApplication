<template>
  <div class="page page-widthdrown">
    <div class="row d-flex justify-content-center">
      <div class="col-4 text-center">
        <form>
          <h1 class="h3 mb-3 fw-normal">Widthdrown Balance</h1>

          <div class="form-floating mb-2">
             <select class="form-control" v-model="accNo" id="floatingSelect">
                <option v-for="account in acclist" v-bind:key="account" :value="account">
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
          <button @click.prevent="onWidthdrownAmount" class="w-100 btn btn-lg btn-primary mb-3"  type="button">
            Widthdrown
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
    async onWidthdrownAmount() {
        let transactionModel = 
        {
            accountNo: this.accNo.accountNo,
            WidthrownAmount: this.amount,
            depositeAmount: 0,
            transactionType: "Widthdrown",
        };
        console.log("Request Widthdrown data :", this.accNo.accountNo, this.amount);
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
