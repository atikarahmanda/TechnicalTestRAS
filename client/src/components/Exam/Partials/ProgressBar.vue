<template>
    <div class="relative pb-5">
        <div class="flex mb-2 items-center justify-between">
            <span class="text-sm font-semibold text-gray-400">Waktu Tersisa</span>
            <span class="text-xs font-semibold text-gray-400">{{ formattedTime }}</span>
        </div>
        <div class="flex mb-2 items-center justify-between">
            <div class="w-full bg-gray-200 rounded-full h-2.5">
                <!-- Progress bar fill -->
                <div class="bg-red-600 h-2.5 rounded-full" :style="{ width: progressBarWidth }"></div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useTimer } from '../../../composables/Exam/useTimer';

const { formattedTime, timerDuration, startTimer, progressBarWidth } = useTimer();

onMounted(() => {
    const storedDuration = localStorage.getItem('timer_duration');
    if (storedDuration) {
        timerDuration.value = parseInt(storedDuration);
    }
    startTimer()
});
</script>
