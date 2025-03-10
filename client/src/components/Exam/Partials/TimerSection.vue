<template>
    <div class="text-center text-xl font-semibold mb-6 text-gray-800">
        <p>Waktu Tersisa: {{ formattedTime }}</p>
    </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useTimer } from '../../../composables/Exam/useTimer';

const { formattedTime, timerDuration, startTimer, stopTimer, resetTimer } = useTimer();

onMounted(() => {
    const storedDuration = localStorage.getItem('timer_duration');
    if (storedDuration) {
        timerDuration.value = parseInt(storedDuration);
    }
    startTimer()
});
</script>