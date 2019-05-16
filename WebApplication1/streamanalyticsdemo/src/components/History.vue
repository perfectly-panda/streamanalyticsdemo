<template>
  <div>
    <h2>Event History</h2>
    <ul id="logList">
      <li v-for="(log, index) in logs" v-bind:key="index">
        {{log}}
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component({
  components: {},
})
export default class History extends Vue {
  @Prop() public logs!: string[];
}
</script>

<style>
  #logList {
    text-align: left;
  }
</style>