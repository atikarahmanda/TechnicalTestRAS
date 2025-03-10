<template>
    <div class="bg-gray-100 flex items-center justify-center min-h-screen">
      <section class="content">
        <div class="error-page flex gap-6 items-center">
          <h2 class="text-7xl font-bold text-red-600">404</h2>
          <div class="error-content max-w-md">
            <h3 class="text-2xl font-semibold text-red-600 whitespace-nowrap flex items-center">
              <i class="fas fa-exclamation-triangle text-red-600 mr-2"></i> Oops! Page not found.
            </h3>
            <p class="mt-4 text-gray-600">
              We could not find the page you were looking for. Meanwhile, you may
              <a @click="back" class="text-blue-500 hover:underline cursor-pointer">return to index</a>.
            </p>
          </div>
        </div>
      </section>
    </div>
  </template>
  


<script setup> 
    import { useRouter } from 'vue-router';

    const router = useRouter()

    const back = () => {
        router.go(-1)
    }
</script>