<template>
  <div id="app">
    <Navigation
          v-on:navigate="onNav($event)"
        />
    <b-row>
      <b-col sm="2">
        <Sidebar
        />
      </b-col>
      <b-col sm="10">
        
        <Main v-bind:view="currentView"/>
      </b-col>
    </b-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import axios from 'axios';
import Main from './components/Main.vue';
import Sidebar from './components/Sidebar.vue';
import Navigation from './components/Navigation.vue';
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.use(BootstrapVue)
Vue.prototype.$axios = axios
axios.defaults.baseURL = 'http://192.168.1.225:8088'

@Component({
  components: {
    Main,
    Sidebar,
    Navigation
  },
})
export default class App extends Vue {

  currentView :string = "Dashboard"
  orders : object = {};
  machines :object = {};
  history : object = {};

  onNav(nav: string) :void {
      this.currentView = nav;
      console.log(nav);
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
</style>
