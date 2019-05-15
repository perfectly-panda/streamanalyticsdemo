<template>
  <div>
    <h2>Machine List</h2>
    <b-form-checkbox
      id="checkbox-1"
      name="checkbox-1"
      value="accepted"
      unchecked-value="not_accepted"
    >
      Show Inactive Machines
    </b-form-checkbox>
    <b-table striped></b-table>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component({
  components: {},
})
export default class Machines extends Vue {}
</script>