<template>
  <v-container grid-list-md mb-4 pa-0 v-if='client' class='elevation-1'>
    <v-layout align-center>
      <v-flex xs2>
        <v-btn fab :outline='!client.expired' icon color='black' dark :loading='client.loading' @click.native='startUpload()'>
          <v-icon>{{ client.expired?"cloud_upload":"cloud_upload" }}</v-icon>
        </v-btn>
      </v-flex>
      <v-flex text-xs-left>
        <span class='title'>{{client.name}}</span>
        <br>
        <span class='caption'>
          {{account.ServerName}}
        </span>
        <!-- <v-divider></v-divider> -->
      </v-flex>
      <v-flex text-xs-right>
        <v-chip outline color='black'>
          <span class='caption' style="user-select:all">
            {{client.streamId}}
          </span>
        </v-chip>
      </v-flex>
    </v-layout>
    <v-layout row wrap align-center>
      <v-flex xs12 v-show='client.loading'>
        <v-progress-linear :active="client.loading" :indeterminate="client.isLoadingIndeterminate" height="21" v-model="client.loadingProgress"></v-progress-linear>
        <span class="caption">{{client.loadingBlurb}}</span>
      </v-flex>
      <v-flex xs4 text-xs-left pl-4 class='caption gray'>
        Last update: {{updatedAt}} (<b>
          <timeago :datetime="client.updatedAt" :auto-update="60"></timeago>
        </b>)
      </v-flex>
      <v-flex xs8 text-xs-right>
        <v-btn small>
          edit selection ({{client.objects.length}} objs)
        </v-btn>
        <v-btn small flat icon color='error' @click.native='deleteClient'>
          <v-icon>delete</v-icon>
        </v-btn>
      </v-flex>
      <v-flex xs12>
      </v-flex>
    </v-layout>
    <v-layout row wrap align-center v-if='stage==="new"'>
    </v-layout>
  </v-container>
</template>
<script>
import Sockette from 'sockette'

export default {
  name: "SenderClient",
  props: {
    client: {
      type: Object,
      default: null
    }
  },
  computed: {
    account( ) {
      return this.$store.state.accounts.find( ac => ac.AccountId === this.client.AccountId )
    },
    updatedAt( ) {
      return ( new Date( this.client.updatedAt ) ).toLocaleDateString( )
    }
  },
  data: ( ) => ( {
    stage: "new"
  } ),
  methods: {
    startUpload( ) {
      // TODO
      console.log( 'not done yet' )
      UiBindings.updateSender( JSON.stringify( this.client ) )
    },
    deleteClient( ) {
      this.$store.dispatch( 'removeReceiverClient', this.client )
      this.sockette.close( )
    },
    wsOpen( e ) {
      this.sockette.json( { eventName: 'join', resourceType: 'stream', resourceId: this.client.streamId } )
    },
    wsMessage( e ) {
      console.log( e.data )
      if ( e.data === 'ping' ) {
        this.sockette.send( 'alive' )
        return
      }
      try {
        let message = JSON.parse( e.data )
        switch ( message.args.eventType ) {
          case 'update-global':
            break
          case 'update-meta':
            break
        }

      } catch ( err ) {
        console.warn( `Could not parse/interpret ${e.data} for ${this.client.streamId}` )
        console.log( e.data )
      }

    },
    wsError( e ) { console.log( e ) },
    wsReconnect( e ) { console.log( e ) },
    wsClose( e ) { console.log( e ) }
  },
  mounted( ) {
    console.log( 'client mounted!' )
    console.log( this.client )
    let wsUrl = this.account.RestApi.replace( 'http', 'ws' )
    this.sockette = new Sockette( `${wsUrl}?client_id=${this.client.clientId}&access_token=${this.account.Token}`, {
      timeout: 5e3,
      maxAttempts: 100,
      onopen: this.wsOpen,
      onmessage: this.wsMessage,
      onerror: this.wsError,
      onreconnect: this.wsReconnect,
      onclose: this.wsClose,
    } )
  },
  beforeDestroy( ) {
    console.log( 'bye bye...' )
    this.sockette.close( )
  }
}
</script>
<style scoped lang='scss'>
</style>