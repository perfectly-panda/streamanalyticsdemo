<template>
  <div>
    <h2>Order List</h2>
    <b-form-checkbox
      id="checkbox-1"
      name="checkbox-1"
      v-model="showCompleted"
    >
      Show Completed Orders
    </b-form-checkbox>
    <b-form-input v-model="filter" placeholder="Filter"></b-form-input>
    <b-table 
      striped 
      :items="tableData"
      :filter="filter"></b-table>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import Order from '../models/Order';

@Component({
  components: {},
})
export default class Orders extends Vue {
  @Prop({ default: () => ([]) }) public orders!: Order[];
  private showCompleted: boolean = false;
  private filter:string = "";
  get tableData(): Order[] {
    if(this.showCompleted){
      return this.orders;
    }
    return this.orders.filter(order => order.completed == false); 
  }

}
</script>