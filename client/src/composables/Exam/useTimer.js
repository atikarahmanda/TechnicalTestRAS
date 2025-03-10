import { ref, computed, watch } from 'vue';
import { useExam } from './useExam';

const { autoSubmitAnswers } = useExam();

export function useTimer() {
  // Timer duration is stored in localStorage or defaults to 30 minutes (in seconds)
  const timerDuration = ref(parseInt(localStorage.getItem('timer_duration')) || 30 * 60);
  const initialDuration = ref(30 * 60); // Initial duration is also set to 30 minutes
  const timerInterval = ref(null);

  // Format timer as mm:ss
  const formattedTime = computed(() => {
    const minutes = Math.floor(timerDuration.value / 60);
    const seconds = timerDuration.value % 60;
    return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
  });

  // Calculate progress bar width as a percentage of the initial timer duration
  const progressBarWidth = computed(() => {
    return `${(timerDuration.value / initialDuration.value) * 100}%`;
  });

  // Start the timer
  const startTimer = () => {
    if (timerInterval.value) return; // Prevent multiple intervals

    timerInterval.value = setInterval(() => {
      if (timerDuration.value <= 0) {
        clearInterval(timerInterval.value); // Stop the timer when it reaches zero
        timerInterval.value = null;
        autoSubmitAnswers(true); // Automatically submit the answers when time is up
      } else {
        timerDuration.value--; // Decrease remaining time by 1 second
        localStorage.setItem('timer_duration', timerDuration.value); // Save updated timer duration
      }
    }, 1000);
  };

  // Stop the timer
  const stopTimer = () => {
    if (timerInterval.value) {
      clearInterval(timerInterval.value); // Stop the interval
      timerInterval.value = null;
    }
  };

  // Reset timer to given duration (default is 30 minutes)
  const resetTimer = (duration = 30 * 60) => {
    initialDuration.value = duration; // Reset initial duration
    timerDuration.value = duration; // Reset remaining time
    localStorage.setItem('timer_duration', timerDuration.value); // Save new duration to localStorage
  };

  // Watch for timer duration changes and store it in localStorage
  watch(timerDuration, (newDuration) => {
    localStorage.setItem('timer_duration', newDuration);
  });

  return {
    timerDuration,
    formattedTime,
    progressBarWidth,
    startTimer,
    stopTimer,
    resetTimer,
  };
}
