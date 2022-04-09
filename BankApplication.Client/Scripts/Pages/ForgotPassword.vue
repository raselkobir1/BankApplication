<template>
  <div class="page page-forgotpassword">
    <div class="row d-flex justify-content-center">
      <div class="col-3 text-center mt-5">
        <form>
          <h1 class="h3 mb-3 mt-5 fw-normal">Input your information</h1>
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
          <button @click.prevent="onClickForgotPassword" class="w-100 btn btn-lg btn-primary mb-2"  type="button">
            Submit
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
      email: '',
    };
  },
  mounted() {
  },
  methods: {
    async onClickForgotPassword() {
        AccountService.forgotPassword(this.email)
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
    ClearInputField() {
      this.email =''
    }
  },
};
</script>
