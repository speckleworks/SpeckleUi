<template>
  <v-card>
    <v-toolbar card dark color="primary">
      <v-btn icon dark @click="$emit('close')">
        <v-icon>close</v-icon>
      </v-btn>
      <v-toolbar-title class="text-truncate font-weight-light">Add a new sender</v-toolbar-title>
    </v-toolbar>
    <v-card-text class='pa-5'>
      <v-layout row wrap align-center >
        <v-flex xs12>
          <p class='headline font-weight-light'>Account</p>
          <p class='caption'>We first need to know which speckle server the data is going to go to.</p>
          <v-overflow-btn :items="$store.state.accounts" label="Account" editable solo v-model='selectedAccount' item-text='fullName' return-object></v-overflow-btn>
        </v-flex>
      </v-layout>
      <v-layout row wrap align-center v-if='selectedAccount'>
        <v-flex xs12>
          <p class='headline font-weight-light'>Stream Name</p>
          <p class='caption'>Something meaningful would do, like 'walls-final-final-2'.</p>
          <v-text-field label="walls-final-final-2" single-line solo v-model='newStreamName'></v-text-field>
        </v-flex>
        <v-flex xs12>
          <p class='headline font-weight-light'>Objects</p>
          <p class='caption'>Will add current object selection to this stream ({{$store.state.selectionCount}}). If no objects are selected in the host application, you will be able to add some later.</p>

        </v-flex>
      </v-layout>
      <v-layout row wrap align-center>
        <v-flex xs12 class='text-xs-left' v-if='!selectedAccount || !selectedAccount.validated'>
          Could not access that server (is it online?) or no server selected.
        </v-flex>
      </v-layout>
      <br>
      <v-layout row wrap align-center>
        <!-- <v-spacer></v-spacer> -->
        <v-btn block color="primary" :ripple="false" :disabled='!validated' @click.native='addSender()'>
          Create Sender
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
    isVisible( val ) {
      if ( val ) {
        this.selectedAccount = this.$store.state.accounts.find( ac => ac.IsDefault === true )
        this.selectedStream = null
      }
    }
  },
  computed: {
    validated( ) {
      if ( this.newStreamName !== null )
        return true
      return false
    }
  },
  data: ( ) => ( {
    selectedAccount: null,
    newStreamName: null,
    selectedObjects: [ ]
  } ),
  methods: {
    async refreshSelection( ) {
      let res = await UiBindings.getObjectSelection( )
      if ( res ) {
        this.selectedObjects = JSON.parse( res )
      } else
        this.selectedObjects = [ ]
    },
    refreshStreamsAtAccount( ) {
      this.$store.dispatch( 'getAccountStreams', this.selectedAccount )
    },
    async addSender( ) {
      let res = await this.$store.dispatch( 'addSenderClient', { account: this.selectedAccount, streamName: this.newStreamName, objects: this.selectedObjects } )
      this.$emit( "close" )
    }
  },
  mounted( ) {
    this.refreshSelection( )
  }
}
</script>
<style scoped lang='scss'>
</style>