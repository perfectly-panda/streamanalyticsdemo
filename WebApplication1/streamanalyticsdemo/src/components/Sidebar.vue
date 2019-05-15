<template>
    <div id="sidebar" class="bg-dark">
        <h3>Machines</h3>
        <h4>Smashers</h4>
        <b-button-group>
            <b-button>-</b-button>
            <b-button>{{smasherCount}}</b-button>
            <b-button>+</b-button>
        </b-button-group>
        <h4>Slashers</h4>
        <b-button-group>
            <b-button>-</b-button>
            <b-button>{{slasherCount}}</b-button>
            <b-button>+</b-button>
        </b-button-group>
        <h4>Trashers</h4>
        <b-button-group>
            <b-button>-</b-button>
            <b-button>{{trasherCount}}</b-button>
            <b-button>+</b-button>
        </b-button-group>
        <h3>Create Order</h3>
        <b-form-input v-model="orderCount" placeholder="Order Count" id="orderCount"></b-form-input>
        <b-button>Create</b-button>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';

@Component({
  components: {},
})
export default class Sidebar extends Vue {
      @Prop() smasherCount!: number;
      @Prop() slasherCount!: number;
      @Prop() trasherCount!: number;

      orderCount: number = 0;
}
</script>

<style>
    #sidebar {
        height: 100vh;
        color: white;
        padding-top: 8px;
    }
    h3, h4{
        padding-top: 5px;
    }
    #orderCount {
        width: 50%;
        margin: auto;
        margin-bottom: 10px;
    }
</style>