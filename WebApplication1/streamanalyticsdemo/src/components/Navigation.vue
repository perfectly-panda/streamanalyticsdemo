<template>
  <div id="navigation">
    <b-navbar toggleable="lg" type="dark" variant="dark">
     <b-navbar-nav>
        <b-nav-item v-on:click="$emit('navigate', 'Dashboard')">Dashboard</b-nav-item>
        <b-nav-item v-on:click="$emit('navigate', 'Machines')">Machines</b-nav-item>
        <b-nav-item v-on:click="$emit('navigate', 'Orders')">Orders</b-nav-item>
        <b-nav-item v-on:click="$emit('navigate', 'History')">Event History</b-nav-item>
      </b-navbar-nav>
    </b-navbar>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component({
  components: {},
})
export default class Navigation extends Vue {}
</script>

<style>
    #navigation {
        border-bottom: 1px white solid;
    }
</style>