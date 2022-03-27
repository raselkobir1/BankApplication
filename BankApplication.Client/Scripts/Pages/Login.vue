<template>
  <div class="page page-login">
    <div class="row d-flex justify-content-center">
      <div class="col-3 text-center">
        <form>
          <img
            class="mt-4 img-bs"
            src="/assets/Images/Bootstrap_logo.svg.png"
            width="130px"
          />
          <h1 class="h3 mb-3 fw-normal">Please sign in</h1>
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
          <div class="checkbox mb-3">
            <label>
              <input type="checkbox" value="remember-me" />Remember me
            </label>
          </div>
          <button @click="onSignin" class="w-100 btn btn-lg btn-primary"  type="button">
            Sign in
          </button>
          <a href="#">
          <h6> Register your account</h6>
          </a>
          <p class="mt-5 mb-3 text-muted">&copy; 2017-2021</p>
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
    async onSignin() {
        AccountService.singin(this.email, this.password)
          .then((response) => {
            console.log("email and password:",this.email, this.password);
            console.log("Response data :", response.data);
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
            
          });
    },
  },
};
</script>
