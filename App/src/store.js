import Vue from 'vue'
import Vuex from 'vuex'
import Axios from 'axios'

Vue.use( Vuex )

export default new Vuex.Store( {
  state: {
    test: {},
    accounts: [ ],
    clients: [ ],
    hostAppName: null,
    currentFileName: null,
    errors: [ ],
    selectionCount: 0
  },
  mutations: {
    ADD_CLIENT( state, client ) {
      if ( !client.hasOwnProperty( "objects" ) ) client.objects = [ ]
      state.clients.unshift( client )
    },

    REMOVE_CLIENT( state, _id ) {
      let index = state.clients.findIndex( cl => cl._id === _id )
      if ( index >= 0 )
        state.clients.splice( index, 1 )
      else
        console.error( 'client not found', _id )
    },

    SET_CLIENT_DATA( state, props ) {
      let found = state.clients.find( cl => cl._id === props._id )
      Object.keys( props ).forEach( key => {
        found[ key ] = props[ key ]
      } )
    },

    DELETE_ALL_CLIENTS( state ) {
      state.clients = [ ]
    },

    SET_ACCOUNTS( state, accounts ) {
      state.accounts = accounts
    },

    SET_ACCOUNT_DATA( state, props ) {
      let found = state.accounts.find( a => a.AccountId === props.AccountId )
      Object.keys( props ).forEach( key => {
        found[ key ] = props[ key ]
      } )
    },

    SET_HOST_APP( state, appName ) {
      state.hostAppName = appName
    },

    SET_SELECTION_COUNT( state, count ) {
      state.selectionCount = count
    }
  },
  actions: {
    bakeReceiver: ( context, client ) => new Promise( async( resolve, reject ) => {
      await UiBindings.bakeReceiver( JSON.stringify( client ) )
      client.expired = false
      client.loading = false
      context.commit( 'SET_CLIENT_DATA', { _id: client._id, expired: false, loading: false } )
    } ),

    addSenderClient: ( context, { account, streamName, objects } ) => new Promise( async( resolve, reject ) => {
      console.log( streamName, objects )
      let res = await Axios.post( `${account.RestApi}/streams`, { name: streamName }, { headers: { Authorization: account.Token } } )
      let stream = res.data.resource
      console.log( stream )

      let client = {...stream }
      client.objects = objects
      client.AccountId = account.AccountId
      client.account = { RestApi: account.RestApi, Email: account.Email, Token: account.Token }
      client.type = 'sender'
      client.expired = true
      client.loading = false
      client.loadingBlurb = 'This stream might be expired.'
      client.isLoadingIndeterminate = true
      client.loadingProgress = 0
      client.message = ''
      client.errors = null
      client.clientId = null

      let docName = await UiBindings.getFileName( )
      let docId = await UiBindings.getDocumentId( )
      let clientCreationRes = await Axios.post( `${account.RestApi}/clients`, { documentType: context.state.hostAppName, streamId: stream.streamId, documentName: docName, documentGuid: docId, role: 'sender' }, { headers: { Authorization: account.Token } } )
      client.clientId = clientCreationRes.data.resource._id

      context.commit( 'ADD_CLIENT', client )

      let dupe = {...client }
      dupe.account = {...dupe.account }
      delete dupe.account.Token

      console.log( 'Sending this to ui bindings to add as a receiver' )
      console.log( client )

      await UiBindings.addSender( JSON.stringify( client ) )
      return resolve( )
    } ),

    addReceiverClient: ( context, { account, stream } ) => new Promise( async( resolve, reject ) => {
      let client = {...stream }

      client.AccountId = account.AccountId
      client.account = { RestApi: account.RestApi, Email: account.Email, Token: account.Token }
      client.type = 'receiver'
      client.expired = true
      client.loading = false
      client.loadingBlurb = ''
      client.isLoadingIndeterminate = true
      client.loadingProgress = 0
      client.message = ''
      client.errors = null
      client.objects = [ ]
      client.clientId = null
      let docName = await UiBindings.getFileName( )
      let docId = await UiBindings.getDocumentId( )
      let res = await Axios.post( `${account.RestApi}/clients`, { documentType: context.state.hostAppName, streamId: stream.streamId, documentName: docName, documentGuid: docId, role: 'receiver' }, { headers: { Authorization: account.Token } } )
      client.clientId = res.data.resource._id
      context.commit( 'ADD_CLIENT', client )

      let dupe = {...client }
      dupe.account = {...dupe.account }
      delete dupe.account.Token
      await UiBindings.addReceiver( JSON.stringify( client ) )
      return resolve( )
    } ),

    removeReceiverClient: ( context, client ) => new Promise( async( resolve, reject ) => {
      await UiBindings.removeClient( JSON.stringify( client ) )
      try {
        await Axios.delete( `${client.account.RestApi}/clients/${client.clientId}`, { headers: { Authorization: client.account.Token } } )
          // TODO: mark stream as deleted too!
      } catch {}
      context.commit( 'REMOVE_CLIENT', client._id )
      console.log( 'hello refresh - this is important' )
    } ),

    updateClient: ( context, { client, expire } ) => new Promise( async( resolve, reject ) => {
      // note: real update, with all the heavy object lifting, happens in .NET
      let res = await Axios.get( `${client.account.RestApi}/streams/${client.streamId}?fields=name,updatedAt`, { headers: { Authorization: client.account.Token } } )
      console.log( res.data.resource )
      let cl = { _id: res.data.resource._id, name: res.data.resource.name, updatedAt: res.data.resource.updatedAt }
      console.log( expire )
      if ( expire ) cl.expired = true
      context.commit( 'SET_CLIENT_DATA', cl )
    } ),

    flushClients: ( context ) => new Promise( async( resolve, reject ) => {
      context.commit( 'DELETE_ALL_CLIENTS' )
    } ),

    getAccounts: ( context ) => new Promise( async( resolve, reject ) => {
      let res = await UiBindings.getAccounts( )
      let accounts = JSON.parse( res )

      accounts.forEach( ac => {
        ac.fullName = ac.Email + ' - ' + ac.ServerName
        ac.streams = [ ]
        ac.validated = false
        context.dispatch( 'getAccountStreams', ac )
      } )

      context.commit( 'SET_ACCOUNTS', accounts )
    } ),

    getAccountStreams: ( context, account ) => new Promise( async( resolve, reject ) => {
      Axios.get( `${account.RestApi}/streams?fields=streamId,name,updatedAt,parent&deleted=false&isComputedResult=false&sort=updatedAt`, { headers: { Authorization: account.Token } } )
        .then( res => {
          res.data.resources.forEach( s => s.fullName = `${s.streamId} - ${s.name}` )
          let sorted = res.data.resources.sort( ( a, b ) => {
            let ad = new Date( a.updatedAt )
            let bd = new Date( b.updatedAt )
            return ad > bd ? -1 : 1
          } ).filter( s => s.parent === null )
          context.commit( 'SET_ACCOUNT_DATA', {...account, validated: true, streams: sorted } )
          resolve( res.data.resources )
        } )
        .catch( err => {
          // console.log( err )
          context.commit( 'SET_ACCOUNT_DATA', {...account, validated: false } )
          reject( err )
        } )
    } ),

    getApplicationHostName: ( context ) => new Promise( async( resolve, reject ) => {
      let res = await UiBindings.getApplicationHostName( )
      context.commit( 'SET_HOST_APP', res )
    } ),

    getExistingClients: ( context ) => new Promise( async( resolve, reject ) => {
      let clients = JSON.parse( await UiBindings.getFileClients( ) )
      console.log( clients )
      if ( clients.length === 0 ) return resolve( )
      clients.forEach( existingClient => {
        try {
          let account = context.state.accounts.find( ac => ac.Email === existingClient.account.Email && ac.RestApi === existingClient.account.RestApi )
          if ( account !== null ) {
            existingClient.account.Token = account.Token
            context.commit( 'ADD_CLIENT', existingClient )
              // TODO: update state on server (client: online)
          } else {
            console.warn( 'no account found for client. sorrrrry!', existingClient )
          }
        } catch {
          console.warn( 'Error in recreating client ' + existingClient.streamId )
        }
      } )
    } ),

    cloneStream: ( context, client ) => new Promise( async( resolve, reject ) => {
      let res = await Axios.post( `${client.account.RestApi}/streams/${client.streamId}/clone`, null, { headers: { Authorization: client.account.Token } } )
      console.log( res.data )
      let tempClient = { _id: client._id, children: res.data.parent.children }
      context.commit( 'SET_CLIENT_DATA', tempClient )
    } )
  }
} )