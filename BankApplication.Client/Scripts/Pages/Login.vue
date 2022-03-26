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
              class="form-control"
              id="floatingInput"
              placeholder="name@example.com"
            />
            <label for="floatingInput">Email address</label>
          </div>

          <div class="form-floating">
            <input
              type="password"
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
          <button class="w-100 btn btn-lg btn-primary" type="submit">
            Sign in
          </button>
          <p class="mt-5 mb-3 text-muted">&copy; 2017-2021</p>
        </form>
      </div>
    </div>
  </div>
</template>