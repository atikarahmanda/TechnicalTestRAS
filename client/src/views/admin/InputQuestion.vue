<template>
    <div class="relative overflow-x-auto sm:rounded-lg py-6 px-6 bg-gray-50">
        <div class="container mx-auto space-y-8 text-gray-800">

            <!-- Two-Column Layout -->
            <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">

                <!-- Left Column (File Upload) -->
                <div class="lg:col-span-8 bg-white rounded-lg shadow-lg p-8 space-y-6 border border-gray-200">
                    <h1 class="text-3xl font-semibold text-center text-gray-800 mb-4">Upload a New Question</h1>
                    <p class="text-lg text-gray-600 mb-6">
                        To add a new question, you can use the provided templates. Please
                        pay attention to the available question templates.<span class="text-red-500">
                            Tutorial
                            in the L1 column.</span>
                    </p>
                    <p class="text-lg mb-4">
                        To download the template, click <a
                            href="https://docs.google.com/spreadsheets/d/1eP-7ocZA-eCV4phrS2kdb19hV8WZvdnrQXyT6e5MAVo"
                            target="_blank" class="text-blue-500 hover:text-blue-700 transition-colors">here</a>.
                    </p>

                    <div class="mb-6">
                        <label for="file_input" class="block text-lg font-medium text-gray-700 mb-2">Upload Your
                            Question File:</label>
                        <input id="file_input" type="file" accept=".xlsx, .xls"
                            class="mt-2 block w-full md:w-96 mx-auto px-4 py-3 text-sm text-gray-900 border border-gray-300 rounded-lg cursor-pointer bg-gray-50 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-red-500 transition-all"
                            @change="handleFileUpload" />
                        <p v-if="fileName" class="mt-2 text-sm text-gray-600">Selected File: <strong>{{ fileName
                                }}</strong></p>
                    </div>

                    <button @click="submitFile" :disabled="!fileSelected"
                        class="w-full py-3 mt-4 bg-red-500 text-white rounded-lg hover:bg-red-600 focus:outline-none focus:ring-2 focus:ring-red-500 disabled:bg-gray-400 disabled:cursor-not-allowed transition-all">
                        Submit File
                    </button>
                </div>

                <!-- Right Column (Question Overview) -->
                <div class="lg:col-span-4 bg-white rounded-lg shadow-lg p-8 space-y-8 border border-gray-200">
                    <h2 class="text-2xl font-semibold text-gray-700 mb-6 text-center">Question Overview</h2>

                    <!-- Total Questions Section -->
                    <div class="mb-6">
                        <h3 class="text-xl font-semibold text-gray-700 text-center">Total Questions</h3>
                        <p class="mt-2 text-5xl font-extrabold text-gray-900 text-center">
                            {{ statistics?.totalQuestion || 0 }}
                        </p>
                    </div>

                    <div class="border-t border-gray-200 my-6"></div>

                    <!-- Difficulty Levels Section -->
                    <div class="mb-6">
                        <h3 class="text-xl font-semibold text-gray-700 text-center">Difficulty Levels</h3>
                        <div class="mt-4 space-y-2 text-center">
                            <p class="text-gray-600">Easy: <span class="text-black font-semibold">{{
                                statistics?.levelEasy || 0 }}</span></p>
                            <p class="text-gray-600">Medium: <span class="text-black font-semibold">{{
                                statistics?.levelMedium || 0 }}</span></p>
                            <p class="text-gray-600">Hard: <span class="text-black font-semibold">{{
                                statistics?.levelHard || 0 }}</span></p>
                        </div>
                    </div>

                    <div class="border-t border-gray-200 my-6"></div>

                    <!-- Categories Section -->
                    <div class="mb-6">
                        <h3 class="text-xl font-semibold text-gray-700 text-center">Categories Breakdown</h3>
                        <div class="mt-4 space-y-2 text-center">
                            <p class="text-gray-600">Basic Programming: <span class="text-black font-semibold">{{
                                statistics?.categoryPemograman || 0 }}</span></p>
                            <p class="text-gray-600">Database: <span class="text-black font-semibold">{{
                                statistics?.categoryDatabase || 0 }}</span></p>
                            <p class="text-gray-600">Analogy: <span class="text-black font-semibold">{{
                                statistics?.categoryAnalogi || 0 }}</span></p>
                            <p class="text-gray-600">Code Snippets: <span class="text-black font-semibold">{{
                                statistics?.categoryCodingan || 0 }}</span></p>
                            <p class="text-gray-600">Basic Logic: <span class="text-black font-semibold">{{
                                statistics?.categoryLogika || 0 }}</span></p>
                        </div>
                    </div>

                    <div class="border-t border-gray-200 my-6"></div>

                    <!-- Helper Text Section -->
                    <div class="text-center mt-6 text-sm text-gray-500">
                        <p class="font-medium text-gray-700">Need help with uploading?</p>
                        <p class="mt-2">Make sure that your file is in the correct format (.xlsx, .xls) and contains the
                            right template structure. If you need a guide on how to format your question, please
                            download the template above or refer to the tutorial section for more details.</p>
                    </div>
                </div>

            </div>

        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import Swal from 'sweetalert2';

const fileName = ref('');
const fileSelected = ref(false);
const fileToUpload = ref(null);
const statistics = ref(null);
const token = localStorage.getItem('token');

const handleFileUpload = (event) => {
    const file = event.target.files[0];
    if (file) {
        fileName.value = file.name;
        fileSelected.value = true;
        fileToUpload.value = file;
    } else {
        fileName.value = '';
        fileSelected.value = false;
    }
};

const submitFile = async () => {
    if (!fileToUpload.value) return;
    const result = await Swal.fire({
        title: 'Apakah Anda Yakin?',
        text: 'Setelah file diunggah, pertanyaan tidak bisa diedit atau dihapus!',
        icon: 'warning',
        confirmButtonText: 'Yes, Upload!',
        reverseButtons: true,
    });

    if (result.isConfirmed) {
        const formData = new FormData();
        formData.append('file', fileToUpload.value);

        try {
            const response = await axios.post('https://localhost:7090/api/UploadExcel', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    'Authorization': `Bearer ${token}`,
                },
            });
            fetchStatistics()
            Swal.fire({
                title: 'Success!',
                text: 'File uploaded successfully!',
                icon: 'success',
                confirmButtonText: 'OK',
            }).then(() => {
                fileName.value = '';
                fileSelected.value = false;
                fileToUpload.value = null;
                document.getElementById('file_input').value = '';
            });
        } catch (error) {
            Swal.fire({
                title: 'Error!',
                text: 'File Upload Tidak Sesuai Format!',
                icon: 'error',
                confirmButtonText: 'Try Again',
            });
        }
    } else {
        Swal.fire({
            title: 'Cancelled',
            text: 'Upload file dibatalkan!',
            icon: 'info',
            confirmButtonText: 'OK',
        });
    }
};

const fetchStatistics = async () => {
    try {
        const response = await axios.get('https://localhost:7090/api/Questions/GetStatisticQuestion', {
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        });

        if (response.data.status === 'success') {
            statistics.value = response.data.data;
        } else {
            console.error('Failed to fetch statistics');
        }
    } catch (error) {
        console.error('Error fetching statistics:', error);
    }
};

onMounted(fetchStatistics);
</script>

<style lang="scss" scoped></style>
