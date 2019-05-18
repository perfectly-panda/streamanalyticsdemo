<template>
  <div class="">
      <b-list-group flush>
        <b-list-group-item>Per Machine Output: {{getLatestOutput().toFixed(2)}} (per hour)</b-list-group-item>
        <b-list-group-item>Time in Hours: {{getTTC().toFixed(2)}}</b-list-group-item>
    </b-list-group>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

import Aggregates from '../models/Aggregates';
import Order from '../models/Order';
import Machine from '../models/Machine';

@Component({
  components: {
  },
})
export default class TTC extends Vue {
  @Prop() public view!: string;
  @Prop() public aggregates!: Aggregates[];
  @Prop() public orders!: Order[];
  @Prop() public machines!: Machine[];

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


  private getLatestOutput(): number {
      if(this.aggregates[this.aggregates.length - 1] == undefined){
        return 0;
      }
      return this.aggregates[this.aggregates.length - 1].perMachineOutput
    }

    private getTTC(): number {
      if(this.getLatestOutput() == 0 || this.machines.length == 0){
        return 0;
      }
      var tasks = this.getPending("Smasher") * 3 + this.getPending("Slasher") * 2 + this.getPending("Trasher");
      return  tasks / (this.getLatestOutput() * this.machines.length)

    }
}
</script>