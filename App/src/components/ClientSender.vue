<template>
  <v-flex xs12>
    <v-card class="elevation-10">
      <v-toolbar color="secondary xxxdarken-1 text-truncate elevation-0" dark>
        <v-icon xxxcolor="white">cloud_upload</v-icon>&nbsp;
        <!-- <v-btn disabled icon color="primary">
              <v-icon small>cloud_upload</v-icon>
        </v-btn>-->
        <v-toolbar-title class="text-truncate font-weight-light">{{client.name}}</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn icon :href="`${client.account.RestApi.replace('api','#')}streams/${client.streamId}`" target="_blank"><v-icon>open_in_new</v-icon></v-btn>
        <v-toolbar-items></v-toolbar-items>
      </v-toolbar>
      <v-card-text class="caption">
        <span>
          <v-icon small>developer_board</v-icon>
          {{account.ServerName}}
        </span>&nbsp;
        <span class="caption">
          <v-icon small>fingerprint</v-icon>StreamId:
          <span style="user-select:all;">
            <b>{{client.streamId}}</b>
          </span>
        </span>&nbsp;
        <span class="caption">
          <v-icon small>hourglass_full</v-icon>Last update:
          <timeago :datetime="client.updatedAt" :auto-update="60"></timeago>
        </span>
        <v-progress-linear
          :active="client.loading"
          :indeterminate="client.isLoadingIndeterminate"
          height="2"
          v-model="client.loadingProgress"
          color="secondary darken-1"
        ></v-progress-linear>
        <span class="caption text--lighten-3">{{client.loadingBlurb}}</span>
      </v-card-text>
      <v-card-actions>
        <v-btn @click.native="startUpload()">PUSH</v-btn>
        <v-btn icon>
          <v-icon>add</v-icon>
        </v-btn>
        <v-btn icon>
          <v-icon>remove</v-icon>
        </v-btn>
        <v-spacer></v-spacer>
        <v-btn small flat outline icon color="error" @click.native="deleteClient">
          <v-icon small>delete</v-icon>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-flex>

  <!-- <v-layout align-center>
      <v-flex xs2>
        <v-btn
          fab
          :outline="!client.expired"
          icon
          color="black"
          dark
          :loading="client.loading"
          @click.native="startUpload()"
        >
          <v-icon>{{ client.expired?"cloud_upload":"cloud_upload" }}</v-icon>
        </v-btn>
      </v-flex>
      <v-flex text-xs-left>
        <span class="title">{{client.name}}</span>
        <br>
        <span class="caption">{{account.ServerName}}</span>
      </v-flex>
      <v-flex text-xs-right>
        <v-chip outline color="black">
          <span class="caption" style="user-select:all">{{client.streamId}}</span>
        </v-chip>
      </v-flex>
    </v-layout>
    <v-layout row wrap align-center>
      <v-flex xs12 v-show="client.loading">
        <v-progress-linear
          :active="client.loading"
          :indeterminate="client.isLoadingIndeterminate"
          height="21"
          v-model="client.loadingProgress"
        ></v-progress-linear>
        <span class="caption">{{client.loadingBlurb}}</span>
      </v-flex>
      <v-flex xs4 text-xs-left pl-4 class="caption gray">
        Last update: {{updatedAt}} (
        <b>
          <timeago :datetime="client.updatedAt" :auto-update="60"></timeago>
        </b>)
      </v-flex>
      <v-flex xs8 text-xs-right>
        <v-btn small flat icon color="error" @click.native="deleteClient">
          <v-icon>delete</v-icon>
        </v-btn>
      </v-flex>
      <v-flex xs12 class="caption">{{client.children}}</v-flex>
  </v-layout>-->
</template>
<script>
import Sockette from "sockette";

export default {
  name: "SenderClient",
  props: {
    client: {
      type: Object,
      default: null
    }
  },
  computed: {
    account() {
      return this.$store.state.accounts.find(
        ac => ac.AccountId === this.client.AccountId
      );
    },
    updatedAt() {
      return new Date(this.client.updatedAt).toLocaleDateString();
    }
  },
  watch: {
    "client.loading"(val, oldVal) {
      if (!val && this.sendStarted) this.broadcastSendEnd();
    },
    client: {
      handler(val, oldVal) {
        console.log(val);
      },
      deep: true
    }
  },
  data: () => ({
    sendStarted: false
  }),
  methods: {
    startUpload() {
      this.sendStarted = true;
      this.$store.dispatch("cloneStream", this.client);
      UiBindings.updateSender(JSON.stringify(this.client));
    },
    deleteClient() {
      this.$store.dispatch("removeReceiverClient", this.client);
      this.sockette.close();
    },
    broadcastSendEnd() {
      this.sendStarted = false;
      this.sockette.json({
        eventName: "broadcast",
        resourceType: "stream",
        resourceId: this.client.streamId,
        args: {
          eventType: "update-global"
        }
      });
    },
    wsOpen(e) {
      this.sockette.json({
        eventName: "join",
        resourceType: "stream",
        resourceId: this.client.streamId
      });
    },
    wsMessage(e) {
      console.log(e.data);
      if (e.data === "ping") {
        this.sockette.send("alive");
        return;
      }
    },
    wsError(e) {
      console.log(e);
    },
    wsReconnect(e) {
      console.log(e);
    },
    wsClose(e) {
      console.log(e);
    }
  },
  mounted() {
    let wsUrl = this.account.RestApi.replace("http", "ws");
    this.sockette = new Sockette(
      `${wsUrl}?client_id=${this.client.clientId}&access_token=${
        this.account.Token
      }`,
      {
        timeout: 5e3,
        maxAttempts: 100,
        onopen: this.wsOpen,
        onmessage: this.wsMessage,
        onerror: this.wsError,
        onreconnect: this.wsReconnect,
        onclose: this.wsClose
      }
    );
  },
  beforeDestroy() {
    this.sockette.close();
  }
};
</script>
<style scoped lang='scss'>
</style>