import axios from 'axios';

const API_URL = 'https://localhost:7090/api/Questions';
const UPDATE_REQUEST_URL = 'https://localhost:7090/api/User/UpdateRequest';
const token = localStorage.getItem('token');

export const fetchQuestionsFromApi = async () => {
  try {
    const response = await axios.get(API_URL, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });
    return response.data.data;
  } catch (error) {
    console.error('Error fetching questions:', error);
    throw error;
  }
};

export const submitAnswer = async (userId, qId, oId) => {
  try {
    await axios.post(
      `${API_URL}/Answer`,
      {
        user_id: userId,
        question_id: qId,
        option_id: oId,
      },
      {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      }
    );

    await axios.put(
      `${UPDATE_REQUEST_URL}/${userId}`,
      true,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      }
    );

    console.log('Request status updated to true');
  } catch (error) {
    console.error('Error submitting answer or updating request:', error);
    throw error;
  }
};
