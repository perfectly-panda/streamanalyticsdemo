<template>
  <div id="app">
    <Navigation
          v-on:navigate="onNav($event)"
        />
    <b-container fluid>
      <b-row>
        <b-col sm="2">
          <Sidebar
            v-bind:machines="machines"
          />
        </b-col>
        <b-col sm="10">   
          <Main 
            v-bind:view="currentView" 
            v-bind:orders="orders"
            v-bind:machines="machines"
            v-bind:logs="logs"
          />
        </b-col>
      </b-row>
    </b-container>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import axios from 'axios';
import BootstrapVue from 'bootstrap-vue'

import Main from './components/Main.vue';
import Sidebar from './components/Sidebar.vue';
import Navigation from './components/Navigation.vue';

import Order from './models/Order';
import Machine from './models/Machine';

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

const signalR = require("@aspnet/signalr");

Vue.use(BootstrapVue)
Vue.prototype.$axios = axios
axios.defaults.baseURL = 'https://sademo.azurewebsites.net/api'
//axios.defaults.baseURL = 'https://localhost:44311/api'

@Component({
  components: {
    Main,
    Navigation,
    Sidebar
  },
})
export default class App extends Vue {
  self = this;

  currentView : string = "Dashboard"
  orders :  Order[] = [];
  machines :  Machine[] = [];
  logs : string[] = [];

  logHub = new signalR.HubConnectionBuilder()
    .withUrl("https://sademo.azurewebsites.net/logHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

  onNav(nav: string) :void {
      this.currentView = nav;
  }

  mounted() {
    axios.all([
      axios.get("/machines")
         .then((response) => {
          this.machines = response.data;
        }),
      axios.get("/orders")
        .then((response) => {
          this.orders = response.data;
        })
    ]);
    this.logHub.start().catch((err: object) => console.error(err.toString()));
    this.logHub.on("newLog", (message: string) => {
      this.logs.unshift(message);
      while(this.logs.length > 100){
        this.logs.pop();
      }
    });
    this.logHub.on("orderUpdate", (message: string) => {
      this.updateOrder(JSON.parse(message));
    });
    this.logHub.on("machineUpdate", (message: string) => {
      this.updateMachine(JSON.parse(message));
    });
  }

  beforeDestroy() {
    this.logHub.stop();
  }

  updateOrder(message: Order){
    var index = this.orders.findIndex(element => element.id === message.id);

    if(index == undefined || index == -1 || index + 1 > this.orders.length){
        this.orders.push(message);
    }
    else {
      this.orders.splice(index, 1, message);
    }
  }

  updateMachine(message: Machine){
    var index = this.machines.findIndex(function(element){
          return element.id == message.id;
      });
      if(index == undefined || index == -1){
        this.machines.push(message);
      }
      else {
        this.machines.splice(index, 1, message);
      }
  }
}
</script>

<style>
  #app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
  }

  .col-sm-2, .col-sm-10{
    padding: 0;
  }

  .custom-select {
    max-width: 90%;
    margin-bottom: 15px;
  }
</style>
