<template>
  <div id="presentation">
    <b-row>
          <b-col sm="2">
            <div >
              <h4>Open Orders: {{orders.length}}</h4>
              <h4>Pending Widgets: {{getPendingCount()}}</h4>
              <h4>Failure Detection:</h4>
                <Detection
                   v-bind:anomalies="anomalies"
                />
              <h4>Time To Complete Open Orders:</h4>
              <TTC 
                   v-bind:orders="orders"
                    v-bind:machines="machines"
                    v-bind:aggregates="aggregates"
                />
                <h4>Past Hour Output:</h4>
                <Totals
                    v-bind:aggregates="aggregates"
                  />
            </div>
          </b-col>
          <b-col sm="10">   
            <iframe src="https://onedrive.live.com/embed?cid=430A52718776F39B&amp;resid=430A52718776F39B%2164994&amp;authkey=AKqqOzBXrq0QlaQ&amp;em=2&amp;wdAr=1.7777777777777777" width="1186px" height="691px" frameborder="0">This is an embedded <a target="_blank" href="https://office.com">Microsoft Office</a> presentation, powered by <a target="_blank" href="https://office.com/webapps">Office Online</a>.</iframe>          </b-col>
        </b-row>
      </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import TTC from './TTC.vue';
import Detection from './Detection.vue';
import Totals from './Totals.vue';

import Machine from '../models/Machine';
import Order from '../models/Order';
import Aggregates from '../models/Aggregates';
import Anomalies from '../models/Anomalies';

@Component({
  components: {
    TTC,
    Detection,
    Totals
  },
})
export default class Presentation extends Vue {
    @Prop() public machines!: Machine[];
    @Prop() public orders!: Order[];
    @Prop() public aggregates!: Aggregates[];
    @Prop() public anomalies!: Anomalies[];

    getPendingCount() :number{
      var filtered = this.orders.filter(o => !o.completed);
      if(filtered.length == 0) return 0;

      return filtered.reduce(function(a: number, b: Order){
            return b == null? a : a + b.widgetCount - b.completedCount;
          }, 0);
    }

}
</script>

<style>
  iframe {
      display: block;       /* iframes are inline by default */
      background: #000;
      border: none;         /* Reset default border */
      height: 90vh;        /* Viewport-relative units */
      width: 80vw;
  }
</style>