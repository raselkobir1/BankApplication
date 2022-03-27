<template>
  <div class="page page-applay-bankaccount">
    <div class="row d-flex justify-content-center">
      <div class="col-6 text-center mb-5">
        <form>
          <h1 class="h3 mb-3 fw-normal">Bank account Application Form</h1>
          <div class="form-floating mb-2">
            <input
              type="text"
              v-model="brance"
              class="form-control"
              id="floatingbrance"
              placeholder="0.00"
            />
             <label for="floatingbrance">Brance name</label>
            </div>

            <div class="form-floating mb-2">
            <input
              type="text"
              v-model="accholder"
              class="form-control"
              id="floatingaccholder"
              placeholder="0.00"
            />
             <label for="floatingaccholder">Account holder name</label>
            </div>
       
            <div class="form-floating mb-2">
            <input
              type="number"
              v-model="openingBalance"
              class="form-control"
              id="floatingInput"
              placeholder="0.00"
            />
            <label for="floatingInput">Opening Balance</label>
          </div>

          <div class="form-floating mb-2">
            <input
              type="text"
              v-model="accountType"
              class="form-control"
              id="floatingAccountType"
              placeholder="Account type"
            />
            <label for="floatingAccountType">Account Type</label>
          </div>
          <a class="btn btn-primary mb-5 mt-3" href="#" @click.prevent="onClickApplayAccount" >Applay</a >

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
        brance:'',
        accholder:'',
        openingBalance: '',
        accountType: '',
    };
  },
  mounted() {
    
  },
  methods: {
    async onClickApplayAccount() {
        let accApplayModel = 
            {
                brance: this.brance,
                accholder: this.accholder,
                openingBalance: this.openingBalance,
                accountType: this.accountType
            };
        AccountService.customerApplayForAccount(accApplayModel)
          .then((response) => {
            console.log("Acc applay model:", accApplayModel);
            console.log("Response data :", response.data);
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
            
          });
    },
  },
};
</script>