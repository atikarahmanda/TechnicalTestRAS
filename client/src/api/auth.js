import axios from 'axios';
import Swal from 'sweetalert2/dist/sweetalert2';

export async function login(email, password, router) {
  try {
    const response = await axios.post('https://localhost:7090/api/User/login', {
      'email': email,
      'password': password,
    });

    Swal.fire({
      toast: true,
      position: 'bottom-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      icon: 'success',
      text: response.data.message
    });

    return response.data.data;
  } catch (error) {
    if (error.response && error.response.status === 403) {
      router.push('/noaccess');
      Swal.fire({
        toast: true,
        position: 'bottom-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        icon: 'question',
        text: 'Token Expired!'
      });
    } else {
      Swal.fire({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        icon: 'error',
        text: error.response.data.message
      });
    }
  }
}
