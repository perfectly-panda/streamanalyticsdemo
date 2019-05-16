<template>
  <div class="Main">
    <component 
      v-bind:is="view" 
      v-bind:orders="orders"
      v-bind:machines="machines"
      v-bind:logs="logs"
    ></component>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import Dashboard from './Dashboard.vue';
import Orders from './Orders.vue';
import Machines from './Machines.vue';
import History from './History.vue';

import Order from '../models/Order';
import Machine from '../models/Machine';

@Component({
  components: {
    Dashboard,
    Orders,
    Machines,
    History
  },
})
export default class Main extends Vue {
  @Prop() public view!: string;
  @Prop() public orders!: Order[];
  @Prop() public machines!: Machine[];
  @Prop() public logs!: string[];
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
