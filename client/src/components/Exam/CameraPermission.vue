<template>
    <div class="flex items-center justify-center min-h-full mb-8">
        <div class="card text-center p-6 bg-white rounded shadow-xl max-w-md">
            <p class="text-lg text-gray-700 mb-6 ">
                Kami membutuhkan izin untuk mengakses kamera dan Mikrofon Anda agar ujian dapat dimulai.
            </p>
            <button @click="requestPermission" class="btn-main px-6 py-3 text-white transition">
                Izinkan Akses Kamera dan Mikrofon
            </button>
        </div>
    </div>
</template>

<script setup>
import { defineEmits } from 'vue';

const emit = defineEmits(['requestPermission']);

const requestPermission = () => {
    emit('requestPermission');
};
</script>
<style scoped></style>