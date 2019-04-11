<template>
  <v-card>
    <v-toolbar card dark color="primary">
      <v-btn icon dark @click="$emit('close')">
        <v-icon>close</v-icon>
      </v-btn>
    </v-toolbar>
    <v-card-text>
      <v-layout row wrap align-center class=''>
        <v-flex xs12>
          <p class='headline thin'>Select an account:</p>
          <p class='caption'>We first need to know which speckle server the data is coming from.</p>
          <v-overflow-btn :items="$store.state.accounts" label="Account" editable solo v-model='selectedAccount' item-text='fullName' return-object></v-overflow-btn>
        </v-flex>
      </v-layout>
      <v-layout row wrap align-center v-if='selectedAccount && selectedAccount.streams.length > 0'>
        <v-flex xs12>
          <p class='headline thin'>Select a stream:</p>
          <p class='caption'>If the stream you're looking for doesn't show up here, try refreshing the list and make sure it's shared with you!</p>
          <v-overflow-btn :items="selectedAccount.streams" :label="'Streams from ' + selectedAccount.fullName" editable v-model='selectedStream' item-text='fullName' return-object solo></v-overflow-btn>
        </v-flex>
        <v-flex xs12 v-if='selectedStream'>
          Last updated: {{selectedStream.updatedAt}}
        </v-flex>
      </v-layout>
      <v-layout row wrap align-center>
        <v-flex xs12 class='text-xs-left' v-if="selectedAccount && selectedAccount.streams.length===0">
          Seems like you don't have any streams to receive. Get someone to share some with you, or, even better, create one!
        </v-flex>
        <v-flex xs12 class='text-xs-left' v-if='!selectedAccount || !selectedAccount.validated'>
          Could not access that server (is it online?) or no server selected.
        </v-flex>
      </v-layout>
    </v-card-text>
    <v-card-text class='text-xs-center' dark>
      <v-layout row wrap align-center>
        <v-spacer></v-spacer>
        <v-btn @click.native='refreshStreamsAtAccount'>
          Refresh
          <v-icon right>refresh</v-icon>
        </v-btn>
        <v-btn color="primary" :ripple="false" :disabled='selectedStream===null' @click.native='addReceiver()'>
          Create Receiver
        </v-btn>
      </v-layout>
    </v-card-text>
  </v-card>
</template>
<script>
export default {
  name: 'NewClient',
  props: {
    isVisible: { type: Boolean, default: false }
  },
  watch: {
    selectedAccount( val ) {
      // todo: get streams for the account
    },
    isVisible( val ) {
      if ( val ) {
        this.selectedAccount = this.$store.state.accounts.find( ac => ac.IsDefault === true )
        this.selectedStream = null
      }
    }
  },
  data: ( ) => ( {
    selectedAccount: null,
    selectedStream: null,
  } ),
  methods: {
    refreshStreamsAtAccount( ) {
      this.$store.dispatch( 'getAccountStreams', this.selectedAccount )
    },
    async addReceiver( ) {
      let res = await this.$store.dispatch( 'addReceiverClient', { account: this.selectedAccount, stream: this.selectedStream } )
      this.$emit( "close" )
    }
  }
}
</script>
<style scoped lang='scss'>
</style>