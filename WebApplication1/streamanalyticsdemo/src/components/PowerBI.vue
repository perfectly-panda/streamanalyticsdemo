<template>
    <div>
        <div id="tile1"></div>
        <div id="tile2"></div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import axios from 'axios';
    import * as pbi from 'powerbi-client';

    @Component({
    components: {},
    })
    export default class PowerBI extends Vue {
        //private models  = window['powerbi-client'].models;
        private powerbi =  new pbi.service.Service(pbi.factories.hpmFactory, pbi.factories.wpmpFactory, pbi.factories.routerFactory);

        mounted() {
            axios.all([
                axios.get("/powerbi/tile?tileid=4f66f127-49c8-4be8-9535-8f8a7afda133")
                    .then((response) => {
                        console.log(response.data);
                        var config= {
                            type: 'tile',
                            dashboardId: 'd9d062a1-6e5f-44f1-a1b3-51969d401d67',
                            tokenType: pbi.models.TokenType.Embed,
                            accessToken: response.data.embedToken.token,
                            embedUrl: response.data.embedUrl,
                            height: 450
                        };

                        var embedContainer :HTMLElement = document.getElementById('tile1') as HTMLElement;
                        var report = this.powerbi.embed(embedContainer, config);
                }).catch((error) => { console.log(error);}
                ),
                axios.get("/powerbi/tile?tileid=c05fe350-8a57-4a54-a492-3f9b47040e0c")
                    .then((response) => {
                    //console.log(response.data);
                })
            ]);
        }

        loadTile(){

        }
    }
</script>
<style>
    #tile1 {
        height: 500px;
    }

</style>