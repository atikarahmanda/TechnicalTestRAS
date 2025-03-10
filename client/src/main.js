// src/main.js
import { createApp } from 'vue';
import '../src/style.css'
import App from './App.vue';
import router from './router';
import './style.css'
import '@fortawesome/fontawesome-free/css/all.min.css';
import VueSweetalert2 from 'vue-sweetalert2';
import 'sweetalert2/dist/sweetalert2.min.css';
import { createPinia } from 'pinia';
import 'preline';

const app = createApp(App);
const pinia = createPinia();
app.use(VueSweetalert2);
app.use(router);
app.use(pinia);
app.mount('#app');

export { pinia };