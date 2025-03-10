<template>
    <div class="exam-content flex flex-col lg:flex-row mx-auto space-y-6 lg:space-y-0 lg:space-x-6">
        <!-- Left Column: Questions -->
        <div class="lg:w-2/3 bg-white p-6 rounded-lg shadow-lg border">
            <!-- Progress Bar -->
            <ProgressBar />

            <!-- Questions -->
            <div v-for="(question, index) in currentQuestions" :key="question.question_id" class="mb-6">
                <p class="font-semibold text-lg mb-4 text-gray-800">{{ startIndex + index + 1 }}. {{
                    question.question_text }}</p>

                <!-- Display Image if available -->
                <div v-if="question.imagePath">
                    <img :src="'src/assets/' + question.imagePath" :class="['w-1/4', 'h-auto', 'hover:scale-105']"
                        @click="openFullscreen(question.imagePath)" />
                </div>

                <!-- Options (Text or Image) -->
                <div v-for="option in question.shuffledOptions" :key="option.option_Id" class="mt-3">
                    <label v-if="option.option_Text" class="block mb-4">
                        <input type="radio" :name="'q' + question.question_id" :value="option.option_Id"
                            :checked="isChecked(question.question_id, option.option_Id)"
                            @change="updateAnswer(question.question_id, option.option_Id)"
                            class="mr-3 form-radio text-red-600" />
                        <span class="text-gray-700">{{ option.option_Text }}</span>
                    </label>

                    <label v-if="option.option_Image" class="block mb-4">
                        <input type="radio" :name="'q' + question.question_id" :value="option.option_Id"
                            :checked="isChecked(question.question_id, option.option_Id)"
                            @change="updateAnswer(question.question_id, option.option_Id)" />
                        <img :src="'src/assets/' + option.option_Image" :class="['w-1/4', 'h-auto', 'object-cover']"
                            @click="openFullscreen(option.option_Image)" />
                    </label>
                </div>
            </div>

            <div class="text-center mt-6">
                <button v-if="isLastPage" @click="submitAnswers" class="btn-main">Simpan Jawaban</button>
            </div>

            <!-- Pagination Controls -->
            <div class="flex justify-between mt-8">
                <button @click="prevPage" :disabled="isFirstPage" class="btn-pagination"
                    :class="{ 'opacity-50 cursor-not-allowed': isFirstPage }">Sebelumnya</button>
                <button @click="nextPage" :disabled="isLastPage" class="btn-main"
                    :class="{ 'opacity-50 cursor-not-allowed': isLastPage }">Selanjutnya</button>
            </div>
        </div>

        <!-- Right Column: Pagination -->
        <div class="lg:w-1/3 flex flex-col bg-white p-6 rounded-lg shadow-lg h-full border">
            <TimerSection />
            <h3 class="text-lg font-semibold text-gray-800 mb-4">Nomor Soal</h3>
            <ul class="grid grid-cols-5 gap-2">
                <li v-for="page in totalPages" :key="page" @click="goToPage(page)" :class="{
                    'bg-red-600 text-white': currentPage + 1 === page,
                    'bg-green-600 text-white': isPageAnswered(page),
                    'hover:bg-gray-200': currentPage + 1 !== page && !isPageAnswered(page)
                }"
                    class="cursor-pointer py-2 px-4 rounded-lg text-center transition duration-300 ease-in-out transform hover:scale-105">
                    {{ page }}
                </li>
            </ul>
        </div>
    </div>

    <!-- Fullscreen Modal -->
    <div v-if="isFullscreen" class="fullscreen-modal" @click="closeFullscreen">
        <div class="fullscreen-content">
            <img :src="'src/assets/' + fullscreenImage" alt="Fullscreen Image" class="fullscreen-image" />
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch, computed } from 'vue';
import ProgressBar from './Partials/ProgressBar.vue';
import TimerSection from './Partials/TimerSection.vue';
import { useExam } from '../../composables/Exam/useExam';
import { useTimer } from '../../composables/Exam/useTimer';
import Swal from 'sweetalert2';

const { startTimer } = useTimer();
const {
    fetchQuestions,
    questions,
    answers,
    currentPage,
    currentQuestions,
    totalPages,
    startIndex,
    isFirstPage,
    isLastPage,
    isChecked, isPageAnswered,
    updateAnswer,
    submitAnswers,
    autoSubmitAnswers,
    goToPage,
    prevPage,
    nextPage,
    hasStarted,
    testing
} = useExam();

const lastVisited = ref("");
let exitCounter = ref(parseInt(localStorage.getItem('exitCounter'), 10) || 0);

const isFullscreen = ref(false);
const fullscreenImage = ref(null);

const openFullscreen = (image) => {
    fullscreenImage.value = image;
    isFullscreen.value = true;
};

const closeFullscreen = () => {
    isFullscreen.value = false;
};

onMounted(() => {
    if (hasStarted) {
        fetchQuestions();
        testing();
        startTimer();
        handleVisibilityChange();
        handleRightClick();
        document.addEventListener('visibilitychange', handleVisibilityChange);
        document.addEventListener('keydown', preventKeys);
        fullscreenEvents.forEach(event => document.addEventListener(event, handleFullscreenChange));
        if (!document.fullscreenElement) {
            handleFullscreenChange();
            document.body.addEventListener('click', requestFullscreen);
        };
    }
});
const preventKeys = (e) => {
    const forbiddenKeys = [
        { ctrlKey: true, key: 'C' }, { ctrlKey: true, key: 'V' }, { ctrlKey: true, key: 'X' },
        { ctrlKey: true, shiftKey: true, key: 'I' }, { ctrlKey: true, key: 'T' }, { ctrlKey: true, key: 'N' }
    ];

    forbiddenKeys.forEach((keyCombination) => {
        if ((e.ctrlKey === keyCombination.ctrlKey) &&
            (e.shiftKey === keyCombination.shiftKey || keyCombination.shiftKey === undefined) &&
            e.key === keyCombination.key) e.preventDefault();
    });
};

const handleRightClick = () => {
    document.addEventListener('contextmenu', (event) => {
        event.preventDefault();
    });
};

const handleVisibilityChange = () => {
    if (document.visibilityState === 'hidden' && hasStarted.value) {
        lastVisited.value = new Date().toLocaleString();
        if (exitCounter.value < 4) {
            exitCounter.value += 1;
            localStorage.setItem('exitCounter', exitCounter.value.toString());
        }

        if (exitCounter.value === 3) {
            alert("Anda telah meninggalkan halaman sebanyak 2 kali. Mohon untuk melanjutkan ujian dan menyelesaikannya. Ujian akan selesai secara otomatis jika Anda meninggalkan halaman lagi.");
        } else if (exitCounter.value === 4) {
            const examContent = document.querySelector('.exam-content');
            if (examContent) {
                alert("Anda telah meninggalkan halaman sebanyak 3 kali. Ujian akan diselesaikan otomatis.");
                autoSubmitAnswers(true);
                setTimeout(() => {
                    examContent.style.filter = 'blur(5px)';
                }, 500);
            }
        }
    }
};

const fullscreenEvents = ['fullscreenchange', 'webkitfullscreenchange', 'mozfullscreenchange', 'msfullscreenchange'];

onBeforeUnmount(() => {
    document.removeEventListener('keydown', preventKeys);
    document.removeEventListener('visibilitychange', handleVisibilityChange);
    fullscreenEvents.forEach(event => document.removeEventListener(event, handleFullscreenChange));
});

const requestFullscreen = () => {
    const element = document.documentElement;

    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.webkitRequestFullscreen) { // Safari
        element.webkitRequestFullscreen();
    } else if (element.mozRequestFullScreen) { // Firefox
        element.mozRequestFullScreen();
    } else if (element.msRequestFullscreen) { // IE/Edge
        element.msRequestFullscreen();
    } else {
        console.warn('Fullscreen not supported in this browser');
    }
};


const handleFullscreenChange = () => {
    if (!document.fullscreenElement) {
        const examContent = document.querySelector('.exam-content');
        if (examContent) {
            examContent.style.filter = 'blur(5px)';
        }
        Swal.fire({
            title: 'Keluar dari Mode Layar Penuh',
            text: 'Anda telah keluar dari mode layar penuh. Pastikan untuk tetap fokus pada ujian.',
            icon: 'info',
            confirmButtonText: 'Oke',
        }).then(() => {
            examContent.style.filter = '';
            document.body.addEventListener('click', requestFullscreen);
        });
    }
};
</script>

<style scoped>
.fullscreen-modal {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.8);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
    cursor: pointer;
}

.fullscreen-content {
    position: relative;
    display: flex;
    justify-content: center;
    align-items: center;
}

.fullscreen-image {
    width: 800px;
    height: 650px;
    object-fit: contain;
}
</style>
