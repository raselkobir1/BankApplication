<template>
    <select class="dropdown form-control" v-model="SelectedItem" @change="handle">
        <option disabled value="">--Select search items--</option>
        <option v-for="item in optionValues" :key="item.index" :value="item.value">{{item.text}}</option>
    </select>
</template>

<script>
export default {
    props: ['optionValues'],
    data() {
        return{
            SelectedItem:'',
        }
    },
    methods: {
        handle() {
            if(this.SelectedItem){
                this.$emit('change',this.SelectedItem);
                this.$emit('input',this.SelectedItem);
            }
        }
    }
}
</script>