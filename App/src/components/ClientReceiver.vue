<template>
  <v-flex xs12>
    <v-hover>
      <v-card
        :class="`elevation-${client.expired ? '15' : '1'} ${client.expired ? 'expired' : ''}`"
        slot-scope="{ hover }"
      >
        <v-toolbar color="secondary text-truncate elevation-0" dark>
          <v-icon color="white">cloud_download</v-icon>
          <v-toolbar-title class="text-truncate font-weight-light">{{client.name}}</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn
            icon
            :href="`${client.account.RestApi.replace('api','#')}streams/${client.streamId}`"
            target="_blank"
          >
            <v-icon>open_in_new</v-icon>
          </v-btn>
          <v-btn :flat="!client.expired" @click.native="bakeReceiver()">
            Pull
            <v-icon small right>cloud_download</v-icon>
          </v-btn>
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
            v-show="client.loading"
            :active="client.loading"
            :indeterminate="client.isLoadingIndeterminate"
            height="2"
            v-model="client.loadingProgress"
            color="primary darken-1"
          ></v-progress-linear>
          <br>
          <span class="caption text--lighten-3">{{client.loadingBlurb}}</span>&nbsp;
          <span class="caption grey--text">Total objects: {{client.objects.length}}</span>
        </v-card-text>
        <v-card-actions>
          <v-btn small icon @click.native="selectObjects">
            <v-icon small>gps_fixed</v-icon>
          </v-btn>
          <v-spacer></v-spacer>
          <v-btn small flat outline icon color="error" @click.native="deleteClient">
            <v-icon small>delete</v-icon>
          </v-btn>
        </v-card-actions>
        <v-alert
          v-model="client.expired"
          dismissible
          color="grey darken-2"
          v-if="client.message && client.message!== ''"
        >{{client.message}}</v-alert>
        <v-alert
          v-model="client.errors"
          dismissible
          type="warning"
          xxxcolor="grey darken-2"
          v-if="client.errors && client.errors!== ''"
          
        ><div v-html="client.errors"></div></v-alert>
      </v-card>
    </v-hover>
  </v-flex>
</template>
<script>
import Sockette from "sockette";

export default {
  name: "Client",
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
  data: () => ({}),
  methods: {
    bakeReceiver() {
      this.$store.dispatch("bakeReceiver", this.client);
    },
    deleteClient() {
      this.$store.dispatch("removeReceiverClient", this.client);
      this.sockette.close();
    },
    selectObjects() {
      UiBindings.selectClientObjects(JSON.stringify(this.client));
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
      try {
        let message = JSON.parse(e.data);
        switch (message.args.eventType) {
          case "update-global":
            this.$store.dispatch("updateClient", {
              client: this.client,
              expire: true
            });
            break;
          case "update-meta":
            this.$store.dispatch("updateClient", {
              client: this.client,
              expire: false
            });
            break;
        }
      } catch (err) {
        console.warn(
          `Could not parse/interpret ${e.data} for ${this.client.streamId}`
        );
        console.log(e.data);
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
    console.log("client mounted!");
    console.log(this.client);
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
    console.log("bye bye...");
    this.sockette.close();
  }
};
</script>
<style scoped lang='scss'>
</style>