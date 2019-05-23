<template>
  <v-app>
    <v-toolbar app>
      <v-toolbar-title class="headline text-uppercase">
        <span @click='showDev()'>Speckle </span>
        <span class="font-weight-light">{{$store.state.hostAppName}}</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn color="secondary" dark absolute bottom right fab :ripple="false" @click.native='showAddNewReceiver=true'>
        <v-icon>cloud_download</v-icon>
      </v-btn>
      <v-btn color="primary" absolute bottom right fab :ripple="false" @click.native='showAddNewSender=true' style="margin-right:60px">
        <v-icon>cloud_upload</v-icon>
      </v-btn>
    </v-toolbar>
    <v-dialog v-model="showAddNewReceiver" scrollable fullscreen>
      <NewClient :is-visible='showAddNewReceiver' @close='showAddNewReceiver=false'>
      </NewClient>
    </v-dialog>
    <v-dialog v-model="showAddNewSender" scrollable fullscreen>
      <NewClientSender :is-visible='showAddNewSender' @close='showAddNewSender=false'>
      </NewClientSender>
    </v-dialog>
    <v-content>
      <v-container grid-list-md pa-0 mt-4>
        <v-layout row wrap>
          <v-flex xs12 md6 pa-3 xxxv-if='receivers.length>0'>
            <span class='headline text-uppercase secondary--text'>Receivers</span>
            <v-divider class='my-4 secondary'></v-divider>
            <span class="" v-if="receivers.length===0">There are no receiver clients in this file.</span>
            <v-container grid-list-xl>
              <v-layout row wrap>
                <client-receiver v-for='client in receivers' :key='client.streamId + ":" + client.AccountId' :client='client '>
                </client-receiver>
              </v-layout>
            </v-container>
          </v-flex>
          <v-flex xs12 md6 pa-3>
            <span class='headline text-uppercase primary--text'>Senders</span>
            <v-divider class='my-4 primary'></v-divider>
             <v-container grid-list-xl>
              <v-layout row wrap>
                <client-sender v-for='client in senders' :key='client.streamId + ":" + client.AccountId' :client='client'>{{client}}</client-sender>
              </v-layout>
             </v-container>
          </v-flex>
        </v-layout>
      </v-container>
    </v-content>
  </v-app>
</template>
<script>
import HelloWorld from './components/HelloWorld'
import NewClient from './components/NewClient.vue'
import NewClientSender from './components/NewClientSender.vue'
import ClientReceiver from './components/ClientReceiver.vue'
import ClientSender from './components/ClientSender.vue'

export default {
  name: 'App',
  components: {
    HelloWorld,
    NewClient,
    NewClientSender,
    ClientReceiver,
    ClientSender
  },
  computed: {
    receivers( ) {
      return this.$store.state.clients.filter( cl => cl.type === 'receiver' )
    },
    senders( ) {
      return this.$store.state.clients.filter( cl => cl.type === 'sender' )
    }
  },
  data( ) {
    return {
      showAddNewReceiver: false,
      showAddNewSender: false
    }
  },
  methods: {
    showDev( ) {
      console.log( 'showing dev' )
      UiBindings.showDev( )
    }
  },
  mounted( ) {
    console.log( 'app mounted!' )

    this.$store.dispatch( 'getAccounts' )
    this.$store.dispatch( 'getApplicationHostName' )
    this.$store.dispatch( 'getExistingClients' )
  }
}
</script>