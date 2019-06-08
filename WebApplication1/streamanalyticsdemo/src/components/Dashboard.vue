<template>
  <div>
    <p>"First we smash it, then we slash it, then we trash it cause you didn't want that broken garbage anyway." --CEO</p>
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
      <b-row>
        <b-col sm="4">
          <b-card
              border-variant="dark"
              background-variant="light"
            >
              <h6 slot="header" class="mb-0">Past Hour Output</h6>
                <b-card-text>
                  <Totals
                    v-bind:aggregates="aggregates"
                  />
                </b-card-text>
            </b-card>
          </b-col>
        <b-col sm="4">
          <b-card
              border-variant="dark"
              background-variant="light"
            >
              <h6 slot="header" class="mb-0">Time to Completion</h6>
                <b-card-text>
                  <TTC 
                   v-bind:orders="orders"
                    v-bind:machines="machines"
                    v-bind:aggregates="aggregates"
                  />
                </b-card-text>
            </b-card>
          </b-col>
          <b-col sm="4">
          <b-card
              border-variant="dark"
              background-variant="light"
            >
              <h6 slot="header" class="mb-0">Failure Detection</h6>
                <b-card-text>
                  <Detection
                    v-bind:anomalies="anomalies"
                  />
                </b-card-text>
            </b-card>
          </b-col>
      </b-row>
    </b-container>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { Line } from 'vue-chartjs'

import MachineTile from './MachineTile.vue';
import TTC from './TTC.vue';
import Detection from './Detection.vue';
import Totals from './Totals.vue';

import Machine from '../models/Machine';
import Order from '../models/Order';
import Aggregates from '../models/Aggregates';
import Anomalies from '../models/Anomalies';


@Component({
  components: {
    MachineTile,
    Totals,
    TTC,
    Detection
  },
})
export default class Dashboard extends Vue {
  @Prop() public machines!: Machine[];
  @Prop() public orders!: Order[];
  @Prop() public aggregates!: Aggregates[];
  @Prop() public anomalies!: Anomalies[];

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

  private chartData = {
      datacollection: {
        labels: this.aggregates.map(a => a.windowEnd),
        datasets: [
          {
            label: 'Output',
            backgroundColor: 'blue',
            data: this.aggregates.map(a => a.totalOutput)
          },
          {
            label: 'Failures',
            backgroundColor: 'black',
            data: this.aggregates.map(a => a.totalFailed)
          }
        ]
      }
    };
    
    private options = {
      responsive: true,
      maintainAspectRatio: false
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
<style>
  .row {
    padding-bottom: 10px;

  }
</style>