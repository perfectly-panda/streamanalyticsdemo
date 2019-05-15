<template>
  <div>
    <h2>Order List</h2>
    <b-form-checkbox
      id="checkbox-1"
      name="checkbox-1"
      value="accepted"
      unchecked-value="not_accepted"
    >
      Show Completed Orders
    </b-form-checkbox>
    <b-table striped></b-table>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component({
  components: {},
})
export default class Orders extends Vue {}
</script>