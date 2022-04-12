<template>
  <div class="page page-changepassword">
    <div class="row d-flex justify-content-center">
      <div class="col-3 text-center">
        <form>
          <h1 class="h3 mb-3 fw-normal">Please input your information</h1>
         
          <div class="form-floating">
            <input
              type="password"
              v-model="currentPassword"
              class="form-control mb-3"
              id="floatingPassword"
              placeholder="Currrent Password"
            />
            <label for="floatingPassword">Current Password</label>
          </div>

          <div class="form-floating">
            <input
              type="password"
              v-model="newPassword"
              class="form-control mb-3"
              id="floatingPassword"
              placeholder="New Password"
            />
            <label for="floatingPassword">New Password</label>
          </div>
          <div class="form-floating">
            <input
              type="password"
              v-model="confirmNewPassword"
              class="form-control mb-3"
              id="floatingPassword"
              placeholder="Confirm New Password"
            />
            <label for="floatingPassword">Confirm New Password</label>
          </div>
          <button @click.prevent="onChangePassword" class="w-100 btn btn-lg btn-primary mb-2"  type="button">
            Change Password
          </button>
        </form>
      </div>
    </div>
  </div>
</template>
<script>
import AccountService from "@scripts/Services/AccountServices";
export default {
    data(){
        return{
            currentPassword:'',
            newPassword: '',
            confirmNewPassword: '',
        }
    },
    methods: {
        onChangePassword() {
            let model = {
                currentPassword: this.currentPassword,
                newPassword: this.newPassword,
                confirmNewPassword: this.confirmNewPassword
            }
           AccountService.changePassword(model)  
          .then((response) => {
            this.$router.push({ name: ""});
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
              this.ClearInputField();
          });
        },
        ClearInputField() {
                this.currentPassword ='',
                this.newPassword ='',
                this.confirmNewPassword =''
            }
    }
}
</script>