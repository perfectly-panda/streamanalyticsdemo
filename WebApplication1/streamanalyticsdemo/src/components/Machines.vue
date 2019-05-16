<template>
  <div>
    <h2>Machine List</h2>
    <b-form-checkbox
      id="checkbox-1"
      name="checkbox-1"
      v-model="inactive"
    >
      Show Inactive Machines
    </b-form-checkbox>
    <b-form-input v-model="machineFilter" placeholder="Filter"></b-form-input>
    <b-table 
      striped 
      :items="tableData"
      :filter="machineFilter"></b-table>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

import Machine from '../models/Machine';


@Component({
  components: {},
})
export default class Machines extends Vue {
  @Prop({ default:() => ([]) }) public machines!: Machine[];

  private inactive: boolean = false;

  private machineFilter:string = "";
  get tableData(): Machine[] {
    if(this.inactive){
      return this.machines;
    }
    return this.machines.filter(m => m.active == true); 
  }
}
</script>