import { defineStore } from 'pinia';
import { ref } from 'vue';
import VueJwtDecode from 'vue-jwt-decode';

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref(localStorage.getItem('token') || null);
  const userRole = ref(null);
  const fullname = ref(null);
  const email = ref(null);
  const isCompleted = ref(null);
  const isDeadline = ref(null)

  // Function untuk mengatur token dan decode JWT
  function setToken(newToken) {
    token.value = newToken;
    if (newToken) {
      localStorage.setItem('token', newToken);
      const decodedToken = VueJwtDecode.decode(newToken);

      // Ambil role dan informasi lain dari token
      userRole.value = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      fullname.value = decodedToken.Fullname;
      email.value = decodedToken.Email;
      isCompleted.value = decodedToken.IsCompleted;
      isDeadline.value = decodedToken.Deadline;
      
    } else {
      localStorage.removeItem('token');
      userRole.value = null;
      fullname.value = null;
      email.value = null;
      isCompleted.value = null;
      isDeadline.value = null
    }
  }

  // Inisialisasi token saat store dibuat
  if (token.value) {
    setToken(token.value);
  }

  // Function untuk login
  function login(newToken) {
    setToken(newToken);
  }

  // Function untuk logout
  function logout() {
    setToken(null);
    localStorage.removeItem('finished')
    // localStorage.removeItem('exitCounter')
    localStorage.removeItem('camera_permission');
    localStorage.removeItem('hasStarted');
    localStorage.removeItem('questions');
    localStorage.removeItem('timer_duration');
    localStorage.removeItem('user_answers');
    localStorage.removeItem('exitCounter');

  }

  function setFinished(bool) {
    isCompleted.value = bool
  }

  return {
    token,
    userRole,
    fullname,
    email,
    isCompleted,
    isDeadline,
    isAuthenticated: () => !!token.value,
    login,
    logout,
    setFinished
  };
});