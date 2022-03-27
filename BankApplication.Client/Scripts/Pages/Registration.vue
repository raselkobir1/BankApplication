<template>
  <div class="page page-login">
    <div class="row d-flex justify-content-center">
      <div class="col-6 text-center mb-5">
        <form>
          <h1 class="h3 mb-3 fw-normal">User Registration Form</h1>
          <div class="form-floating">
            <input
              type="email"
              v-model="email"
              class="form-control"
              id="floatingInput"
              placeholder="name@example.com"
            />
            <label for="floatingInput">Email address</label>
          </div>

          <div class="form-floating">
            <input
              type="password"
              v-model="password"
              class="form-control"
              id="floatingPassword"
              placeholder="password"
            />
            <label for="floatingPassword">password</label>
          </div>
          <a class="btn btn-primary mb-5 mt-3" href="#" @click.prevent="onClickRegister" >Register</a >

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
      email: '',
      password: '',
    };
  },
  mounted() {
    
  },
  methods: {
    async onClickRegister() {
        AccountService.CustomerRegistration(this.email, this.password)
          .then((response) => {
            console.log("email and password:",this.email, this.password);
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