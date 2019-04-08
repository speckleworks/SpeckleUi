import Vue from 'vue'
import './plugins/vuetify'
import App from './App.vue'
import router from './router'
import store from './store'
import 'roboto-fontface/css/roboto/roboto-fontface.css'
import 'material-design-icons-iconfont/dist/material-design-icons.css'

Vue.config.productionTip = false

import VueTimeago from 'vue-timeago'
Vue.use( VueTimeago, {
  name: 'Timeago',
  locale: 'en',
} )

// set up an event bus on the window, to be used by the SpeckleUiBindings class to send events here
window.EventBus = new Vue( )

window.EventBus.$on( 'update-client', args => {
  let cl = JSON.parse( args )
  console.log( cl )
  window.Store.commit('SET_CLIENT_DATA', cl )
} )

window.Store = store
window.app = new Vue( {
  router,
  store,
  render: h => h( App )
} ).$mount( '#app' )