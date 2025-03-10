<template>
  <div class="flex min-h-screen bg-gray-100 login-view">
    <!-- Left side with logo -->
    <div class="w-3/5 my-4 ml-4 flex relative">
      <div class="flex items-center justify-center my-auto">
        <img src="../../assets/working2-vector.png" width="80%" alt="working-vector" class="mx-auto">
      </div>
    </div>

    <!-- Right side with login form -->
    <div class="w-2/5 flex items-center justify-center bg-white rounded-md m-3">
      <div class="w-full h-auto max-w-md bg-white pb-10 px-14">.
        <div class="flex items-center">
            <img src="../../assets/bemind-logo2.png" alt="Berca Logo" class="w-full h-full">
          </div>
      <h2 class="text-4xl font-bold mt-8 text-center">Technical Test (Online)</h2>
      <p class="text-sm font-thin text-gray-400 mt-2 mb-14 text-center">Please Enter Your Detail</p>
        <form @submit.prevent="handleLogin" class="space-y-3">
          <!-- Email Input -->
          <div class="relative">
            <input 
              id="email"
              v-model="email" 
              type="email" 
              class="w-full p-2 border rounded-md pr-10" 
              placeholder="Email"
              @blur="emailTouched = true"
            >
            <span class="absolute right-3 top-3 text-gray-400">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
            </span>
            <span class="text-red-500 text-xs italic" :style="{visibility: emailTouched && !isEmailValid ? 'visible' : 'hidden'}">
              Masukkan format email yang benar.
            </span>
          </div>


          <!-- Password Input -->
          <div class="relative">
            <input 
              id="password"
              v-model="password" 
              :type="showPassword ? 'text' : 'password'" 
              class="w-full p-2 border rounded-md pr-10" 
              placeholder="Password"
              @blur="passwordTouched = true"
            >
            <span 
              @click="togglePasswordVisibility" 
              class="absolute right-3 top-3 text-gray-400 cursor-pointer"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path v-if="!showPassword" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path v-if="!showPassword" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
              </svg>
            </span>
          </div>
          <span class="text-red-500 text-xs italic" :style="{visibility: passwordTouched && !password ? 'visible' : 'hidden'}">
            Password wajib diisi.
          </span>
          <!-- Login Button -->
          <br>
          <button 
            type="submit" 
            :class="submitButtonClass"
            :disabled="!formIsValid"
            class="text-white py-2 px-4 rounded-md font-semibold transition-transform duration-300 ease-in-out w-full"
            >
            <template v-if="isLoading">
              <div class="inline-flex items-center gap-2">
                <div
                  class="animate-spin inline-block w-5 h-5 border-[3px] border-current border-t-transparent text-white rounded-full"
                  role="status"
                  aria-label="loading"
                >
                </div>
                <p class="text-white">Loading...</p>
              </div>
            </template>
            <template v-else>
              <span>Masuk</span>
            </template>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>


<script setup>
import { ref, computed } from 'vue';
import { login } from '../../api/auth';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../../store/auth';

const authStore = useAuthStore();
const email = ref('');
const password = ref('');
const emailTouched = ref(false);
const passwordTouched = ref(false);
const showPassword = ref(false);
const router = useRouter();
const isLoading = ref(false);


const handleLogin = async () => {
  if(formIsValid.value) {
    isLoading.value = true;
    try {
      const token = await login(email.value, password.value, router);
      authStore.login(token)
      const userRole = authStore.userRole;
      const isCompleted = authStore.isCompleted;
      const isDeadline = authStore.isDeadline;

      // Redirect berdasarkan role
      if (userRole === 'Candidate') {
        if (isCompleted === 'True') {
          router.push('finished')
        }else if (isDeadline === 'True') {
          router.push ({ name: 'ExpiredTest' })
        }else {
          router.push({ name: 'Test' })
        }
      } else if (userRole === 'Admin') {
        router.push('/dashboard');
      }
    } catch (error) {
        throw error

    } finally {
      isLoading.value = false;
    }
  }
};



// Computed properties for validation
const isEmailValid = computed(() => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email.value);
});

const formIsValid = computed(() => {
  return isEmailValid.value && password.value;
});

// Computed class for submit button
const submitButtonClass = computed(() => {
  return formIsValid.value
    ? 'bg-red-600 cursor-pointer'
    : 'bg-gray-600 cursor-not-allowed';
});


</script>
