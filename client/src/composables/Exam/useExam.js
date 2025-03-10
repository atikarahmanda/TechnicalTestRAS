import { ref, computed, watch } from 'vue';
import { fetchQuestionsFromApi, submitAnswer } from '../../api/Exam';
import { useTimer } from './useTimer';
import VueJwtDecode from 'vue-jwt-decode';
import Swal from 'sweetalert2';

export function testing() {
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        navbar.remove();
    }
}

export function useExam() {
    const questions = ref([]);
    const answers = ref(JSON.parse(localStorage.getItem('user_answers')) || {});
    const currentPage = ref(0);
    const hasStarted = ref(localStorage.getItem('hasStarted') === 'true' || false);
    const questionsPerPage = 1;
    const timerDuration = useTimer();

    // Computed properties for questions, pages, and navigation
    const currentQuestions = computed(() => questions.value.slice(currentPage.value * questionsPerPage, (currentPage.value + 1) * questionsPerPage));
    const totalPages = computed(() => Math.ceil(questions.value.length / questionsPerPage));
    const isFirstPage = computed(() => currentPage.value === 0);
    const isLastPage = computed(() => currentPage.value === totalPages.value - 1);
    const startIndex = computed(() => currentPage.value * questionsPerPage);

    const token = localStorage.getItem('token');
    let userId = null;
    if (token) {
        try {
            const decodedToken = VueJwtDecode.decode(token);
            userId = decodedToken.userId;
        } catch (error) {
            console.error('Invalid token:', error);
        }
    }

    // Watcher for saving answers
    watch(answers, (newAnswers) => {
        localStorage.setItem('user_answers', JSON.stringify(newAnswers));
    }, { deep: true });

    // Fetching and handling questions
    const fetchQuestions = async () => {
        const storedQuestions = localStorage.getItem('questions');
        if (storedQuestions) {
            questions.value = JSON.parse(storedQuestions);
        } else {
            try {
                const questionsData = await fetchQuestionsFromApi();
                if (Array.isArray(questionsData)) {
                    questions.value = questionsData.map(q => ({ ...q, shuffledOptions: shuffleOptions(q.options) }));
                    localStorage.setItem('questions', JSON.stringify(questions.value));
                }
            } catch (error) {
                console.error('Error fetching questions:', error);
            }
        }
    };

    // Shuffle options
    const shuffleOptions = (options) => [...options].sort(() => Math.random() - 0.5);

    // Answer handling
    const updateAnswer = (questionId, optionId) => {
        answers.value[questionId] = optionId;
    };
    const isChecked = (questionId, optionId) => answers.value[questionId] === optionId;

    // Page answered check
    const isPageAnswered = (page) => {
        const pageQuestions = questions.value.slice((page - 1) * questionsPerPage, page * questionsPerPage);
        return pageQuestions.every(q => answers.value[q.question_id] !== undefined);
    };

    // Submit answers with confirmation
    const submitAnswers = async () => {
        if (hasStarted.value) {
            const result = await Swal.fire({
                title: 'Konfirmasi Pengiriman Jawaban',
                text: 'Apakah Anda yakin ingin menyimpan jawaban Anda?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Ya, Simpan Jawaban',
                cancelButtonText: 'Batal',
            });

            if (result.isConfirmed) {
                await autoSubmitAnswers(true);
            }
        }
    };

// Auto-submit answers
const autoSubmitAnswers = async (isConfirmed) => {
    if (hasStarted.value && isConfirmed) {
        try {
            const validAnswers = Object.entries(answers.value).filter(([qId, oId]) => qId && oId);
            
            if (validAnswers.length > 0) {
                await Promise.all(validAnswers.map(([qId, oId]) => submitAnswer(userId, qId, oId)));
                Swal.fire({ icon: 'success', title: 'Jawaban Berhasil Disimpan', text: 'Jawaban Anda telah berhasil disimpan.' });
                resetExam();
            } else {
                Swal.fire({icon: 'warning', title: 'Gagal Mengirim Jawaban', text: 'Tidak ada jawaban yang valid untuk disimpan.', timer: 3000, timerProgressBar: true,willClose: () => {window.location.reload();}});
            }
        } catch (error) {
            Swal.fire({ icon: 'error', title: 'Gagal Mengirim Jawaban', text: 'Terjadi kesalahan : ' + error.message });
        }
    }
};

    // Reset the exam
    const resetExam = () => {
        hasStarted.value = false;
        timerDuration.value = 30 * 60;
        setTimeout(() => {
            location.href = "/finished"
            localStorage.setItem('finished', true)
            clearExamData(); 
        }, 2000)
    };

    // Clear stored data
    const clearExamData = () => {
        localStorage.removeItem('camera_permission');
        localStorage.removeItem('hasStarted');
        localStorage.removeItem('questions');
        localStorage.removeItem('timer_duration');
        localStorage.removeItem('user_answers');
        localStorage.removeItem('exitCounter');
    };

    // Page navigation
    const goToPage = (page) => currentPage.value = page - 1;
    const prevPage = () => { if (!isFirstPage.value) currentPage.value--; };
    const nextPage = () => { if (!isLastPage.value) currentPage.value++; };

    return {
        questions, answers, currentPage, currentQuestions, totalPages, startIndex, isFirstPage, isLastPage, 
        isChecked, isPageAnswered, fetchQuestions, updateAnswer, submitAnswers, autoSubmitAnswers, goToPage, 
        prevPage, nextPage, hasStarted, testing
    };
}
