<template>
  <div class="machineCard">
    <b-card
      border-variant="dark"
      background-variant="light"
    >
      <h6 slot="header" class="mb-0">{{typeName}}</h6>
        <b-card-text>
          <b-list-group flush>
            <b-list-group-item>Pending Widgets: {{orderCount}}</b-list-group-item>
            <b-list-group-item>Active Machines: {{machines.length}}</b-list-group-item>
          </b-list-group>
        </b-card-text>
    </b-card>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';


import Machine from '../models/Machine';

@Component({
  components: {},
})
export default class MachineTile extends Vue {
    @Prop() public typeName!: string;
    @Prop() public machines!: Machine[];
    @Prop() public orderCount!: number;

}
</script>
