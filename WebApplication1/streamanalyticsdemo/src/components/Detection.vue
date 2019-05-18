<template>
  <div class="">
      <b-list-group flush>
        <b-list-group-item>Failures Detected: {{detectedFailures()}}</b-list-group-item>
        <b-list-group-item>Detection Rate: {{detectionRate()}}</b-list-group-item>
    </b-list-group>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

import Anomalies from '../models/Anomalies';

@Component({
  components: {
  },
})
export default class Detection extends Vue {
  @Prop() public view!: string;
  @Prop() public anomalies!: Anomalies[];

  getFailures(): number {
    if(this.anomalies.length == 0){
      return 0;
    }
    return this.anomalies.filter(a=> a.broken).length;
  }
  detectedFailures(): number {
    if(this.anomalies.length == 0){
      return 0;
    }
    return this.anomalies.filter(a=> a.isChangePointAnomaly).length;
  }
  detectionRate(): string {
    if(this.anomalies.length == 0){
      return "-";
    }
    var correct =  this.anomalies.filter(a=> a.correctDetection).length;
    return ((correct / this.anomalies.length) * 100).toFixed(2) + "%"
  }
}
</script>