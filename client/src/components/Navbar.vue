<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../store/auth';
import { useCameraPermission } from '../composables/Exam/useCameraPermission';

const { cameraPermissionGranted } = useCameraPermission()

console.log(`camera access : ${cameraPermissionGranted.value}`)

const authStore = useAuthStore();
const router = useRouter();
const fullname = authStore.fullname;
const email = authStore.email;
const isDropdownOpen = ref(false);
const dropdownRef = ref(null);

const emit = defineEmits(['clickEvent']);
const props = defineProps({
  isAdmin: Boolean
});

function toggleSidebar() {
  emit('clickEvent', 'toggle-sidebar');
}

function logout() {
  authStore.logout();
  router.push('/login');
}
</script>

<template>
  <nav class="flex justify-between items-center px-4 py-2 text-semibold text-gray-900 shadow-md navbar bg-white"
    :class="{ 'invisible': useCameraPermission.value }">

    <button v-if="props.isAdmin" class="p-1 mr-4" @click="toggleSidebar">
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" class="h-8 w-6">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
      </svg>
    </button>

    <div v-else class="ml-3">
      <img src="../assets/bemind-logo2.png" class="h-10 w-auto" alt="Logo Berca">
    </div>

    <div class="relative">
      <!-- Dropdown Button from DaisyUI -->
      <div class="dropdown dropdown-end">
        <button class="bg-transparent mx-4 h-full flex items-center space-x-2 dropdown-toggle">
          <i class="fas fa-2x fa-user-circle"></i>
          <div class="font-bold text-xl">{{ fullname }}</div>
          <i class="fas fa-caret-down pl-2"></i>
        </button>

        <!-- Dropdown Menu -->
        <div tabindex="0"
          class="dropdown-content card card-compact bg-white z-[1] w-72 p-2 shadow rounded-md border border-1 px-3 py-4 mt-2">
          <div>
            <div class="flex gap-3">
              <div class="flex flex-col">
                <p class="font-bold text-xl">{{ fullname }}</p>
                <p class="w-full max-w-full overflow-hidden text-ellipsis break-words italic font-extralight text-sm">
                  {{ email }}
                </p>
              </div>
            </div>
            <hr class="mt-3 border border-1">
            <div class="mt-2 py-2 rounded-md hover:bg-slate-100 cursor-pointer" @click="logout">
              <i class="fas fa-right-from-bracket pl-2"></i>
              <span class="ml-2">Logout</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>
