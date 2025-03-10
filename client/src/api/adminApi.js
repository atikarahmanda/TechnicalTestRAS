import axios from "axios";
import Swal from 'sweetalert2/dist/sweetalert2';

const BASE_URL = 'https://localhost:7090/api/User'

export const getAuthHeader = (token) => {
    return { Authorization: `Bearer ${token}` };
}

export const fetchAllUsers = async (token) => {
    try {
      const response = await axios.get(BASE_URL, {
        headers: getAuthHeader(token),
      });
      return response.data.data;
    } catch (error) {
      throw error;
    }
};

export const fetchAllQuestion = async (token) => {
  try {
    const response = await axios.get('https://localhost:7090/api/Questions/QuestionAll', {
      headers: getAuthHeader(token),
    });
    return response.data.data;
  } catch (error) {
    throw error;
  }
};

export const fetchExpiredUsers = async (token) => {
    try {
      const response = await axios.get(`${BASE_URL}/PastDeadline`, {
        headers: getAuthHeader(token),
      });
      return response.data.data;
    } catch (error) {
      throw error;
    }
};

export const fetchAllResult = async (token) => {
    try {
      const response = await axios.get('https://localhost:7090/api/Questions/users/scores', {
        headers: getAuthHeader(token),
      });
      return response.data.data;
    } catch (error) {
      throw error;
    }
};

export const addUser = async (token, email, firstName, lastName) => {
  try {
    const response = await axios.post(`${BASE_URL}/register`, {
      email: email,
      firstName: firstName,
      lastName: lastName
    }, {
      headers: getAuthHeader(token)
    })
    const message = response.data.message;

    Swal.fire({
        icon: 'success',
        title: 'Success',
        text: message,
    });

    return response
  } catch (error) {
    const errorMessage = error.response?.data?.message || 'Registration failed. Please try again later.';
        
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: errorMessage,
        confirmButtonText: 'OK'
    });
    throw error
  }
}

export const deleteQuestion = async (questionId, token) => {
    try {
        const response = await axios.delete(`https://localhost:7090/api/Questions/DeleteQuestion/${questionId}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data;
    } catch (error) {
        console.error('Failed to delete question:', error);
        throw new Error(error.response?.data?.message || 'Failed to delete question');
    }
};



export const deleteCandidate = async (token, userId) => {
  try {
      const response = await axios.delete(`${BASE_URL}/${userId}`, {
          headers: getAuthHeader(token),
      });
      Swal.fire({
        icon: 'success',
        title: 'Success',
        text: 'Candidate deleted successfully',
      });
      return response.data;
  } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to delete candidate.';
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: errorMessage,
      });
      throw error;
  }
};

export const fetchStatistics = async (token) => {
  try {
    const response = await axios.get(`${BASE_URL}/statistics`, {
      headers: getAuthHeader(token),
    });
    return response.data.data;
  } catch (error) {
    throw error;
  }
};




