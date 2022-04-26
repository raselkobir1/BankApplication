<template>
  <div class="page page-login">
    <div class="row d-flex justify-content-center">
      <div class="col-6 text-center mb-5">
        <form enctype="multipart/form-data">
          <h1 class="h3 mb-3 fw-normal">User Registration Form</h1>
            <input-field
                class="col-12"
                label="First Name"
                type="text"
                data-vv-name="firstName"
                v-validate="'required'"
                v-model="firstName"
                :error="checkValidation('firstName')"
              >
           </input-field>
            <input-field
                class="col-12"
                label="Last Name"
                type="text"
                data-vv-name="lastName"
                v-validate="'required'"
                v-model="lastName"
                :error="checkValidation('lastName')"
              >
           </input-field>
            <input-field
                class="col-12"
                label="Password"
                type="password"
                data-vv-name="password"
                v-validate="'required'"
                v-model="password"
                :error="checkValidation('password')"
              >
           </input-field>
            <input-field
                class="col-12"
                label="Confirm password"
                type="password"
                data-vv-name="confirmPassword"
                v-validate="'required'"
                v-model="confirmPassword"
                :error="checkValidation('confirmPassword')"
              >
           </input-field>

              <label>Email</label>
              <input class="form-control" type="text" disabled readonly v-model="invitedEmail">

          <a class="btn btn-primary mb-5 mt-3" href="#" @click.prevent="onClickAcceptInvitationUser" >Signup</a >
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import AccountService from "@scripts/Services/AccountServices";

export default {
  data() {
    return {
      firstName: '',
      lastName: '',
      password: '',
      confirmPassword:'',
      invitedEmail:'',
    };
  },
  mounted() {
    this.getInvitedEmail(); 
  },
  methods: {
     onClickAcceptInvitationUser() {
        let userAcceptModel = {
            firstName : this.firstName,
            lastName : this.lastName,
            password : this.password,
            confirmPassword : this.confirmPassword,
            code : this.$route.query.code,
            email : this.$route.query.email,
        }
        
        AccountService.AcceptInvitationUser(userAcceptModel)
          .then((response) => {
            this.$router.push({ name: "verifyEmail"});
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
              this.ClearInputField();
          });
      },
    getInvitedEmail() {
      this.invitedEmail = this.$route.query.email;
    },
    checkValidation(field) {
      return this.$validator.errors.first(field);
    },
      ClearInputField() {
        this.email ='',
        this.password =''
      }
  },
};
</script>