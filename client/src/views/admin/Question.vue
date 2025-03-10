=<template>
    <div class="relative overflow-x-auto shadow-md sm:rounded-lg p-2">
        <!-- Search Bar -->
        <div class="flex justify-between items-center mb-4 border border-black rounded-md">
            <input
                v-model="searchQuery"
                type="text"
                placeholder="Search question..."
                class="p-2 border rounded w-full"
            />
        </div>

        <!-- Tabel -->
        <table class="min-w-full text-sm text-left text-gray-500">
            <thead class="text-xs text-gray-700 uppercase bg-white">
                <tr>
                    <th class="px-6 py-3 border-b">No</th>
                    <th class="px-6 py-3 border-b">Question Text</th>
                    <th class="px-6 py-3 border-b">Action</th>
                </tr>
            </thead>
            <tbody>
                <tr
                    v-for="(question, index) in filteredQuestions"
                    :key="question.questionId"
                    class="bg-white border-b hover:bg-gray-100"
                >
                    <td class="px-6 py-4 font-medium text-black border-b">{{ currentPageIndex + index + 1 }}</td>
                    <td class="px-6 py-4 font-medium text-black border-b">
                        <!-- Menampilkan teks pertanyaan -->
                        {{ question.questionText }}
                        <!-- Menampilkan gambar jika tersedia -->
                        <div v-if="question.questionImage">
                            <img :src="question.questionImage" style="height: 200px; object-fit: cover;" />
                        </div>
                    </td>
                    <td class="px-6 py-4 font-medium text-black border-b">
                        <div class="relative group">
                            <button @click="handleDelete(question.questionId)" class="btn-sm btn-danger tooltip"
                                data-tip="Delete Data">
                                <i class="fas fa-trash fa-lg ml-1 text-[#f55a5a] cursor-pointer"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>

        <!-- Pagination Controls -->
        <div class="flex justify-center mt-4 space-x-2">
            <button
                v-for="page in totalPages"
                :key="page"
                @click="currentPage = page"
                class="px-4 py-2 border rounded pagination"
                :class="{
                    'bg-brown text-white': page === currentPage,
                    'bg-white text-gray-700': page !== currentPage
                }"
            >
                {{ page }}
            </button>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { fetchAllQuestion, deleteQuestion } from '../../api/adminApi'; // Pastikan fungsi deleteQuestion sudah ada
import { useAuthStore } from '../../store/auth';
import Swal from 'sweetalert2/dist/sweetalert2';

const authStore = useAuthStore();
const questions = ref([]);
const searchQuery = ref(''); // Search input
const currentPage = ref(1);
const itemsPerPage = 10;

// Ambil data dari server
const fetchQuestion = async () => {
    try {
        const token = authStore.token;
        const data = await fetchAllQuestion(token);
        questions.value = data.map(question => {
            const imageName = question.imagePath
                ? new URL(`../../assets/Images/Question/${question.imagePath.split('\\').pop()}`, import.meta.url).href
                : null;
            return {
                questionId: question.question_id,
                questionText: question.question_text,
                questionImage: imageName,
            };
        });
    } catch (error) {
        console.error('Error fetching questions:', error);
    }
};

// Fungsi untuk menghapus pertanyaan
const handleDelete = async (questionId) => {
    Swal.fire({
        title: 'Are you sure?',
        text: "This action cannot be undone!",
        icon: 'warning',
        confirmButtonText: 'Yes, delete it!'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                const token = authStore.token;
                await deleteQuestion(questionId, token);
                questions.value = questions.value.filter(q => q.questionId !== questionId);
            } catch (error) {
                Swal.fire ({
                    title: 'Cannot delete',
                    text: "The question because it is referenced to answer data",
                    icon: 'error',
                })
                console.error('Error deleting question:', error);
            }
        }
    });
};

// Filter data berdasarkan pencarian
const filteredQuestions = computed(() => {
    const search = searchQuery.value.toLowerCase();
    const filtered = questions.value.filter(q => q.questionText.toLowerCase().includes(search));
    const start = (currentPage.value - 1) * itemsPerPage;
    const end = start + itemsPerPage;
    return filtered.slice(start, end);
});

// Total halaman berdasarkan hasil pencarian
const totalPages = computed(() => {
    const search = searchQuery.value.toLowerCase();
    const filtered = questions.value.filter(q => q.questionText.toLowerCase().includes(search));
    return Math.ceil(filtered.length / itemsPerPage);
});

// Indeks awal per halaman
const currentPageIndex = computed(() => (currentPage.value - 1) * itemsPerPage);

onMounted(() => {
    fetchQuestion();
});
</script>

<style scoped>
button {
    transition: background-color 0.3s ease, color 0.3s ease;
}
th {
    background-color: brown;
    border: 1px solid #A9A9A9;
    color: #fff;
}
td {
    padding: 12px 16px;
    border: 1px solid #A9A9A9;
}
.pagination:hover {
    background-color: #ed4e4e;
    color: #fff;
}

.bg-brown {
    background-color: brown;
}

.bg-brown:hover {
    background-color: #be0808;
}

.bg-white {
    background-color: white;
}
</style>
