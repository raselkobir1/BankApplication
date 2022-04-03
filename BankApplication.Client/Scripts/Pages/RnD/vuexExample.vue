<template>
    <div class="row">
        <div v-if="!!loginUser">
         <h5> Get Data from vueX, data set on store when user login. note: data show if page no refresh</h5>
        <p><b>User email :</b> {{loginUser.userEmail}}</p>
        <p><b>User Role :</b>{{loginUser.role}}</p>
        <p><b>User Token :</b>{{loginUser.token}}</p>
        <p><b>User Id : </b>{{loginUser.appUserId}}</p>
        </div>
        <div v-else>
            <p>You can't see user value because you refresh the page.Please login again</p>
        </div>

    </div>
</template>

<script>
import Store from "@scripts/Store/Store"

export default {
  data() {
    return {
      loginUser:'',
    };
  },
  mounted(){
    this.getloginData()
  },
  methods: {
    getloginData() {
         this.loginUser = Store.getLoggedinUser();
      },
  },
};
</script>