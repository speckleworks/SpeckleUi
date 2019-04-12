<template>
  <v-card>
    <v-toolbar card dark color="black">
      <v-btn icon dark @click="$emit('close')">
        <v-icon>close</v-icon>
      </v-btn>
    </v-toolbar>
    <v-card-text>
      <v-layout row wrap align-center class=''>
        <v-flex xs12>
          <p class='headline thin'>Select an account:</p>
          <p class='caption'>We first need to know which speckle server the data is going to go to.</p>
          <v-overflow-btn :items="$store.state.accounts" label="Account" editable solo v-model='selectedAccount' item-text='fullName' return-object></v-overflow-btn>
        </v-flex>
      </v-layout>
      <v-layout row wrap align-center v-if='selectedAccount'>
        <v-flex xs12>
          <p class='headline thin'>Specify a stream name:</p>
          <p class='caption'>Something meaningful would do, like 'walls-final-final-2'.</p>
          <v-text-field label="Test-Final-Final2.psd" single-line solo v-model='newStreamName'></v-text-field>
        </v-flex>
        <v-flex xs12>
          <p class='headline thin'>Object selection <v-btn small @click.native='refreshSelection()'>refresh selection ({{selectedObjects.length}})</v-btn>
          </p>
          <p class='caption'>Below are the selected objects that we'll try and send. Note: you'll need to select them first!</p>
          <v-layout row wrap>
            <v-flex xs12 v-for='obj in selectedObjects' :key='obj.id'>
              <v-layout row>
                <v-flex xs4>{{obj.id}}</v-flex>    
                <v-flex xs4>{{obj.type}}</v-flex>    
                <v-flex xs4>{{obj.cat}}</v-flex>    
              </v-layout>
            </v-flex>
          </v-layout>
        </v-flex>
      </v-layout>
      <v-layout row wrap align-center>
        <v-flex xs12 class='text-xs-left' v-if='!selectedAccount || !selectedAccount.validated'>
          Could not access that server (is it online?) or no server selected.
        </v-flex>
      </v-layout>
      <br>
      <v-layout row wrap align-center>
        <v-spacer></v-spacer>
        <v-btn color="primary" :ripple="false" :disabled='!validated' @click.native='addSender()'>
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