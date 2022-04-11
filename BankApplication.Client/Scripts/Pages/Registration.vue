<template>
  <div class="page page-login">
    <div class="row d-flex justify-content-center">
      <div class="col-6 text-center mb-5">
        <form enctype="multipart/form-data">
          <h1 class="h3 mb-3 fw-normal">User Registration Form</h1>
          <div class="form-floating">
            <input
              type="email"
              v-model="email"
              class="form-control mb-2"
              id="floatingInput"
              ref="email"
              placeholder="name@example.com"
            />
            <label for="floatingInput">Email address</label>
          </div>

          <div class="form-floating">
            <input
              type="password"
              v-model="password"
              class="form-control mb-2"
              ref="password"
              id="floatingPassword"
              placeholder="password"
            />
            <label for="floatingPassword">password</label>
          </div>
          <div class="">
            <input class="form-check-input" v-model="role" type="checkbox" id="flexCheckIndeterminate">
            <label class="form-check-label" for="flexCheckIndeterminate">
              Is admin user
            </label>
        </div>
          <!-- <input type="file" ref="file" @change="onSelect"> <br>  -->

          <a class="btn btn-primary mb-5 mt-3" href="#" @click.prevent="onClickRegister" >Register user</a >
          <!-- <button class="btn btn-primary mb-5 mt-3" @click.prevent="onSubmit">Submit image</button> -->
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
      role: false,
      file:null,
    };
  },
  mounted() {
    
  },
  methods: {
    onSelect(event) {
      //this.file = this.$refs.file.files[0];
      this.file = event.target.files[0];
      console.log("select image :",this.file );
    },
    onSubmit() {
      const fd = new FormData();
      fd.append('Image', this.file);
        AccountService.CustomerRegistrationFormSubmit(fd)
          .then((response) => {
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
              this.ClearInputField();
          });
    },

    async onClickRegister() {
        AccountService.CustomerRegistration(this.email, this.password, this.role)
          .then((response) => {
            this.$router.push({ name: "verifyEmail"});
            console.log("Response data :", response.data);
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
              this.ClearInputField();
          });
    },
      ClearInputField() {
        this.email ='',
        this.password =''
      }
  },
};
</script>