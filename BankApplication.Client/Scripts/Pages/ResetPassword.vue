<template>
  <div class="page page-resetpassword">
    <div class="row d-flex justify-content-center">
      <div class="col-3 text-center mt-5">
        <form>
          <h1 class="h3 mb-3 fw-normal">Input your information</h1>
          <div class="form-floating">
            <input
              type="password"
              v-model="newPassword"
              class="form-control mb-3"
              id="floatingInput"
              placeholder="New password"
            />
            <label for="floatingInput">New Password</label>
            <div class="form-floating">
            <input
              type="password"
              v-model="confirmPassword"
              class="form-control mb-3"
              id="floatingInput"
              placeholder="Confirm password"
            />
            <label for="floatingInput">Confirm password</label>
          </div>
          </div>
          <button @click.prevent="onClickResetPassword" class="w-100 btn btn-lg btn-primary mb-2"  type="button">
            Reset Password
          </button>
          <p class="mt-5 mb-3 text-muted">&copy; 2021-2022</p>
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
      newPassword: '',
      confirmPassword: '',
      
    };
  },
  mounted() {
  },
  methods: {
    async onClickResetPassword() {
        let model = {
            newPassword: this.newPassword,
            confirmPassword: this.confirmPassword,
            email: this.$route.query.email,
            token: this.$route.query.token,
        }
        AccountService.resetPassword(model)
          .then((response) => {
            this.$router.push({ name: "afterPassReset"});
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
              this.ClearInputField();
          });
    },
    ClearInputField() {
      this.newPassword =''
      this.confirmPassword =''
    }
  },
};
</script>
