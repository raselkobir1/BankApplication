<template>
  <div :class="{ 'form-field': true, invalid: hasError }">
    <label class="form-label" v-if="label && label.length > 0">{{label}}</label>
    <input class="form-control" :placeholder="placeholder"
      v-model="value_temp" :type="type ? type : 'text'"
      @change="onInput" @blur="onBlur"/>
    <span class="form-feedback" style="color:red" v-if="hasError">{{error}}</span>
  </div>
</template>

<script>
export default {
  props: ["label", "placeholder", "value", "type","error"],
  $_veeValidate: {
    name() {
      return this.$attr["data-vv-name"] ? this.$attr["data-vv-name"] : this.label;
    },
    value() {
      return this.value_temp;
    },
  },
  data() {
    return {
      value_temp: this.value,
    };
  },
  computed: {
    hasError() {
      return this.error && this.error.length > 0;
    },
  },
  methods: {
    onInput() {
      this.$emit("input", this.value_temp);
    },
    onBlur() {
      this.$emit("blur", this.value_temp);
    },
  },
  watch: {
    value() {
      this.value_temp = this.value;
    },
  },
};
</script>
