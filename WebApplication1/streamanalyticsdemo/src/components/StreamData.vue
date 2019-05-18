<template>
    <div>
    <b-tabs content-class="mt-3">
        <b-tab title="Aggregates" active>
            Time Frame: 60 minutes
            <b-table 
                striped 
                :items="aggregateData"></b-table>
        </b-tab>
        <b-tab title="Anomalies">
            <b-table 
                striped 
                :items="anomalyData"></b-table>
        </b-tab>
    </b-tabs>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

import Aggregates from '../models/Aggregates';
import Anomalies from '../models/Anomalies';

@Component({
  components: {},
})
export default class StreamData extends Vue {
  @Prop() public aggregates!: Aggregates[];
  @Prop() public anomalies!: Anomalies[];

  get aggregateData(): object[] {
      return this.aggregates.map(a => ({
        Timestamp: a.windowEnd,
        Total: a.totalOutput,
        TotalFailed: a.totalFailed,
        PerMachineOutput: a.perMachineOutput,
        PerMachineFailed: a.perMachineFailure
      }));
  }

  get anomalyData(): object[] {
      return this.anomalies.map(a => ({
        Timestamp: a.timeStamp,
        MachineId: a.machineId,
        FailureDetected: a.isChangePointAnomaly == 1 ? "Yes" : "No",
        Correct: a.correctDetection == 1? "Yes" : "No"
      }));
  }


}
</script>