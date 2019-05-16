<template>
  <div>
    <h2>Dashboard</h2>
    <b-container fluid>
      <b-row>
        <b-col sm="4" v-for="(type, index) in types" v-bind:key="index">
          <MachineTile
            v-bind:typeName="type"
            v-bind:machines="getType(type)"
            v-bind:orderCount="getPending(type)"
          >
          </MachineTile>
        </b-col>
      </b-row>
    </b-container>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import MachineTile from './MachineTile.vue';

import Machine from '../models/Machine';
import Order from '../models/Order';

@Component({
  components: {
    MachineTile
  },
})
export default class Dashboard extends Vue {
  @Prop() public machines!: Machine[];
  @Prop() public orders!: Order[];

  types: string[] = ["Smasher", "Slasher", "Trasher"];

  private getType(type: string): Machine[] {
        if(this.machines == undefined ||this.machines == null){
            return [];
        }
        return this.machines.filter(m=> m.active ==  true && m.machineType == type);
  }

  private getPending(type: string): number {
        if(this.orders == undefined ||this.orders == null){
            return 0;
        }

        var filteredOrders = this.orders.filter(o=> !o.completed);
        
        if(type == "Smasher"){
          return filteredOrders.reduce(function(a: number, b: Order){
            return b.pendingCount == null? a : a + b.pendingCount;
          }, 0);
        }
        else if(type == "Slasher"){
          return filteredOrders.reduce(function(a: number, b: Order){
              return b.smashedCount == null? a : a + b.smashedCount;
          }, 0);
        }
        else {
          return filteredOrders.reduce(function(a: number, b: Order){
              return b.slashedCount == null? a : a + b.slashedCount;
          }, 0);
        }

        return 0;
  }
}
</script>
<style>
</style>