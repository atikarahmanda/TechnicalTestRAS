<template>
    <div class="max-w-2xl mx-auto text-center mb-8">
        <h2 class="text-2xl font-semibold mb-6 text-gray-900">Panduan Ujian</h2>
        <div class="text-left space-y-4">
            <div v-for="(instruction, index) in instructions" :key="index" class="text-lg text-justify">
                <strong>{{ instruction.title }}:</strong> {{ instruction.description }}
            </div>
        </div>
        <button @click="startExam" class="btn-main mt-6">Mulai Ujian</button>
    </div>
</template>

<script setup>
import { ref } from "vue"
const emit = defineEmits(['startExam']);

const startExam = () => {
    emit('startExam');
    window.location.reload();

};

// Exam instructions data
const instructions = ref([
    {
        title: 'Izin Kamera dan Mikrofon',
        description: 'Kami memerlukan akses ke kamera dan Mikrofon Anda untuk memastikan keamanan dan kejujuran ujian. Pastikan kamera berfungsi dengan baik selama ujian berlangsung.'
    },
    {
        title: 'Durasi Ujian',
        description: 'Ujian ini memiliki batas waktu 30 menit. Timer akan terus berjalan selama ujian berlangsung. Anda akan diberi pemberitahuan saat waktu hampir habis.'
    },
    {
        title: 'Navigasi Soal',
        description: 'Soal ujian dibagi menjadi beberapa halaman. Gunakan tombol "Sebelumnya" atau "Selanjutnya" untuk berpindah antar halaman.'
    },
    {
        title: 'Menjawab Soal',
        description: 'Pilih jawaban yang menurut Anda paling tepat. Anda dapat mengubah jawaban kapan saja sebelum ujian diselesaikan.'
    },
    {
        title: 'Menyimpan Jawaban',
        description: 'Semua jawaban Anda akan disimpan otomatis saat Anda berpindah halaman.'
    },
    {
        title: 'Waktu Habis',
        description: 'Jika waktu habis, semua jawaban akan disimpan otomatis dan ujian akan berakhir.'
    }
]);
</script>

<style scoped></style>