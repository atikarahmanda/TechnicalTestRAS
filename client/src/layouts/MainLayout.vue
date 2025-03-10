<script setup>
import Navbar from '../components/Navbar.vue';
import Sidebar from '../components/Sidebar.vue';
import { computed } from 'vue';
import { useAuthStore } from '../store/auth';
import { ref } from 'vue';

const authStore = useAuthStore();
const sidebarOpen = ref(false);

const isAdmin = computed(() => authStore.userRole === 'Admin').value;

const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value;
};
</script>

<template>
  <div class="flex h-screen bg-gray-100">
    <Sidebar v-if="isAdmin" :sidebarOpen="sidebarOpen" />
    <div class="flex-1 flex flex-col">
      <Navbar :isAdmin="isAdmin" @clickEvent="toggleSidebar" />
      <main class="flex-1 overflow-x-auto my-2">
        <div class="content items-center justify-center">
          <router-view />
        </div>
      </main>
    </div>
  </div>
</template>