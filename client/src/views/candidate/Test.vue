<template>
  <div class="container mx-auto px-4 py-8">
    <!-- Check for Mobile Devices -->
    <div v-if="isMobile" class="text-center text-red-500">
      Maaf, versi ujian untuk perangkat mobile tidak tersedia saat ini. Mohon gunakan perangkat komputer untuk mengakses
      ujian.
    </div>

    <!-- Camera Permission Section -->
    <CameraPermission v-if="!cameraPermissionGranted && !hasStarted && !isMobile"
      @requestPermission="requestCameraPermission" />

    <!-- Exam Guide Section -->
    <ExamGuide v-if="cameraPermissionGranted && !hasStarted && !isMobile" @startExam="startExam" />

    <!-- Exam Section -->
    <ExamSection v-if="cameraPermissionGranted && hasStarted && !isMobile" />

  </div>
</template>
<script setup>
import { ref, watch, onMounted } from 'vue';
import '../../assets/css/exam.css';
import ExamGuide from '../../components/Exam/ExamGuide.vue';
import ExamSection from '../../components/Exam/ExamSection.vue';
import CameraPermission from '../../components/Exam/CameraPermission.vue';

import { useCameraPermission } from '../../composables/Exam/useCameraPermission';
import { useTimer } from '../../composables/Exam/useTimer';

// Check for mobile devices
const isMobile = ref(false);

// Use this to detect mobile devices based on screen width or user agent
const checkMobileDevice = () => {
  if (window.innerWidth <= 768 || /Mobi|Android/i.test(navigator.userAgent)) {
    isMobile.value = true;
  } else {
    isMobile.value = false;
  }
};

onMounted(() => {
  checkMobileDevice(); // Check on mount

  // Optional: Update on window resize
  window.addEventListener('resize', checkMobileDevice);
});

// Permissions and Timer logic
const { cameraPermissionGranted, requestCameraPermission } = useCameraPermission();
const { startTimer } = useTimer();

const hasStarted = ref(localStorage.getItem('hasStarted') === 'true');

watch(() => hasStarted.value, (newVal) => {
  localStorage.setItem('hasStarted', newVal.toString());
});

const startExam = () => {
  hasStarted.value = true;
  localStorage.setItem('hasStarted', 'true');
  startTimer();
};

</script>
