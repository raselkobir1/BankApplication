<template>
  <div class="page page-login">
    <div class="row d-flex justify-content-center">
      <div class="col-3 text-center">
        <form>
          <!-- <img
            class="mt-4 img-bs"
            src="/assets/Images/Bootstrap_logo.svg.png"
            width="130px"
          /> -->
          <h1 class="h3 mb-3 fw-normal">Please sign in</h1>
          <div class="form-floating">
            <input
              type="email"
              v-model="email"
              class="form-control mb-3"
              id="floatingInput"
              placeholder="name@example.com"
            />
            <label for="floatingInput">Email address</label>
          </div>

          <div class="form-floating">
            <input
              type="password"
              v-model="password"
              class="form-control mb-3"
              id="floatingPassword"
              placeholder="password"
            />
            <label for="floatingPassword">password</label>
          </div>
          <div class="d-flex checkbox mb-3 justify-content-between">
            <label>
              <input type="checkbox" v-model="isRemembeMe" value="remember-me" />Remember me
            </label>
              <a href="#" @click.prevent="onForgotPassword" >Forgot password</a >
          </div>
          <button @click.prevent="onSignin" class="w-100 btn btn-lg btn-primary mb-2"  type="button">
            Sign in
          </button>
          <a href="#" @click.prevent="onClickRegistration" >Register your account</a >

          <p class="mt-5 mb-3 text-muted">&copy;
              <a :href="siteApi +'swagger/index.html'" target="_blank">view api documentation</a>
          </p>

        </form>
      </div>
    </div>
  </div>
</template>

<script>
//import LoginModel from "@scripts/Models/Accounts/LoginModel";
import AccountService from "@scripts/Services/AccountServices";
import Store from "@scripts/Store/Store"

export default {
  data() {
    return {
      email: '',
      password: '',
      isRemembeMe: false,
      siteApi:SITE_API_ROOT,
    };
  },
  mounted() {
    
  },
  methods: {
    async onSignin() {
        AccountService.singin(this.email,this.isRemembeMe, this.password)
          .then((response) => {
            console.log("login Response data -:", response.data);
            Store.setLoggedinUser(response.data);
            if(response.data.role =="Administrator") {
              this.$router.push({ name: "admin" });
            }
            else{
              this.$router.push({ name: "customer" });
            }
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
              this.ClearInputField();
          });
    },
    onClickRegistration() {
      //alert("cliek registration");
      this.$router.push({ name: "registration" });
    },
    onForgotPassword() {
      this.$router.push({ name: "forgotPassword" });
    },
    ClearInputField() {
      this.email ='',
      this.password =''
    }
  },
};
</script>
