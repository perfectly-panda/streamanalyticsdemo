<template>
    <div id="sidebar" class="bg-dark">
        <h3>Create Order</h3>
        <b-form-input v-model="orderCount" placeholder="Order Count" id="orderCount"></b-form-input>
        <b-button :disabled="waitToSend" v-on:click="createOrder()">Create</b-button>
        <h3>Machines</h3>
        <h4>Smashers</h4>
        <b-button-group>
            <b-button v-on:click="removeMachine('Smasher')">-</b-button>
            <b-button>{{getType("Smasher")}}</b-button>
            <b-button v-on:click="addMachine('Smasher')" >+</b-button>
        </b-button-group>
        <h4>Slashers</h4>
        <b-button-group>
            <b-button v-on:click="removeMachine('Slasher')">-</b-button>
            <b-button>{{getType("Slasher")}}</b-button>
            <b-button v-on:click="addMachine('Slasher')">+</b-button>
        </b-button-group>
        <h4>Trashers</h4>
        <b-button-group>
            <b-button v-on:click="removeMachine('Trasher')">-</b-button>
            <b-button>{{getType("Trasher")}}</b-button>
            <b-button v-on:click="addMachine('Trasher')">+</b-button>
        </b-button-group>
        <h3>Break Machine</h3>
        <b-form-select v-model="machineToBreak" :options="getMachinesToBreak()"></b-form-select>
        <b-button v-on:click="breakMachine()">Break!</b-button>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import axios from 'axios';

import Machine from '../models/Machine';
import Order from '../models/Order';

@Component({
  components: {},
})
export default class Sidebar extends Vue {
    @Prop({ default:() => ([]) }) public machines!: Machine[];

    private orderCount: number = 0;
    private waitToSend: boolean = false;
    private machineToBreak: number = 0;

    private getType(type: string): number {
        if(this.machines == undefined ||this.machines == null){
            return 0;
        }
        return this.machines.filter(m=> m.active ==  true && m.machineType == type).length;
    }

    private getBreakable(): Machine[] {
        return this.machines.filter(m => m.active && !m.broken);
    }

    addMachine(type: string){
        let newMachine: Machine = new Machine();
        newMachine.machineType = type;

        axios.post("/machines", newMachine)
            .then(message => console.log(message))
            .catch(exception => console.log(exception));
    }

    removeMachine(type:string){
        if(this.getType(type) > 0){
            var options = this.machines.filter(m=> m.active ==  true && m.machineType == type);

            var firstBroken = options.findIndex(element => element.broken);

            var id = 0;

            if(firstBroken >=0){
                id = options[firstBroken].id;
            }
            else{
                id = options[0].id;
            }

            axios.delete("/machines/" + id).then(message => console.log(message));
        }
    }

    breakMachine(){
        axios.post("/machines/break/" + this.machineToBreak)
            .then(message => console.log(message))
            .catch(exception => console.log(exception));
    }

    getMachinesToBreak(): Object[] {
        var possibleMachines = this.machines.filter(m=> m.active && !m.broken);
        var list: Object[] = [];
        possibleMachines.forEach((machine) => list.push({value: machine.id, text: machine.id + " - " + machine.machineType}));

        return list;
    }

    createOrder(){
        if(this.orderCount> 0){
            if(this.orderCount > 10000){
                this.orderCount = 10000;
            }

            if(!this.waitToSend){
                let order: Order = new Order();
                order.widgetCount = this.orderCount;

                axios.post("/orders", order)
                    .then(message => console.log(message))
                    .catch(exception => console.log(exception));

                this.waitToSend = true;
                this.orderCount = 0;

                setTimeout(() => { this.waitToSend = false; }, 3000);
            }
        }
    }
}
</script>

<style>
    #sidebar {
        height: 100vh;
        color: white;
        padding-top: 8px;
    }
    h3, h4{
        padding-top: 5px;
    }
    #orderCount {
        width: 50%;
        margin: auto;
        margin-bottom: 10px;
    }
</style>