<template>
  <div class="">
      <b-list-group flush>
        <b-list-group-item>Widgets Proccessed: {{getLatestOutput().toFixed(0)}}</b-list-group-item>
        <b-list-group-item>Failed Production: {{getLatestFailed().toFixed(0)}}</b-list-group-item>
    </b-list-group>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

import Aggregates from '../models/Aggregates';

@Component({
  components: {
  },
})
export default class Totals extends Vue {
    
  @Prop() public aggregates!: Aggregates[];

  
  private getLatestOutput(): number {
      if(this.aggregates[this.aggregates.length - 1] == undefined){
        return 0;
      }
      return this.aggregates[this.aggregates.length - 1].totalOutput
    }

private getLatestFailed(): number {
      if(this.aggregates[this.aggregates.length - 1] == undefined){
        return 0;
      }
      return this.aggregates[this.aggregates.length - 1].totalFailed
    }
}
</script>