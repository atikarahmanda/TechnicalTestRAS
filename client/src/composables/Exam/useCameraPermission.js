import { ref } from 'vue';
import Swal from 'sweetalert2';

export function useCameraPermission() {
  const cameraPermissionGranted = ref(localStorage.getItem('camera_permission') === 'true');

  // Request camera access
  const requestCameraPermission = async () => {
    try {
      await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
      cameraPermissionGranted.value = true;
      localStorage.setItem('camera_permission', 'true');
    } catch (error) {
      Swal.fire({
        icon: 'error',
        title: 'Izin Kamera Tidak Diberikan',
        text: 'Akses ke kamera diperlukan untuk melanjutkan ujian.'
      });
      cameraPermissionGranted.value = false;
      localStorage.setItem('camera_permission', 'false');
    }
  };

  return {
    cameraPermissionGranted,
    requestCameraPermission
  };
}
