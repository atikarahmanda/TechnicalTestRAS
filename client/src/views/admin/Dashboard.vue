<template>
    <div class="shadow-md sm:rounded-lg py-6 px-4 sm:px-6">
        <!-- Dashboard Header -->
        <h1 class="text-3xl font-medium mb-6 text-gray-800">Dashboard</h1>

        <!-- Dashboard Cards -->
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 mb-10">
            <!-- Candidate Count Card -->
            <div
                class="bg-[#f55a5a] text-white p-6 rounded-lg shadow-sm hover:shadow-xl transform hover:scale-105 transition-all duration-300 flex items-center">
                <i class="fas fa-users text-4xl mr-4" aria-label="Candidates"></i>
                <div>
                    <h2 class="text-lg font-semibold">Candidates</h2>
                    <p class="text-2xl font-bold">{{ candidatesCount }}</p>
                </div>
            </div>

            <!-- Completed Exams Card -->
            <div
                class="bg-green-500 text-white p-6 rounded-lg shadow-sm hover:shadow-xl transform hover:scale-105 transition-all duration-300 flex items-center">
                <i class="fas fa-check-circle text-4xl mr-4" aria-label="Completed Exams"></i>
                <div>
                    <h2 class="text-lg font-semibold">Completed</h2>
                    <p class="text-2xl font-bold">{{ completedExamsCount }}</p>
                </div>
            </div>

            <!-- Pending Exams Card -->
            <div
                class="bg-yellow-500 text-white p-6 rounded-lg shadow-sm hover:shadow-xl transform hover:scale-105 transition-all duration-300 flex items-center">
                <i class="fas fa-clock text-4xl mr-4" aria-label="Pending Exams"></i>
                <div>
                    <h2 class="text-lg font-semibold">Uncompleted</h2>
                    <p class="text-2xl font-bold">{{ pendingExamsCount }}</p>
                </div>
            </div>
        </div>

        <!-- Chart.js -->
        <div class="bg-white p-6 rounded-lg shadow-sm mb-6 mx-auto max-w-full">
            <h2 class="text-xl font-semibold text-gray-800 mb-4">Statistics</h2>
            <canvas id="examStatsChart" class="w-full max-h-[450px]"></canvas>
        </div>
    </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import { fetchStatistics } from '../../api/adminApi';
import { useAuthStore } from '../../store/auth';
import Chart from 'chart.js/auto';

const authStore = useAuthStore();
let examStatsChart = null;
const candidatesCount = ref(0);
const completedExamsCount = ref(0);
const pendingExamsCount = ref(0);

const getStatistics = async () => {
    try {
        const token = authStore.token;
        const data = await fetchStatistics(token);
        candidatesCount.value = data.totalCandidate;
        completedExamsCount.value = data.totalComplete;
        pendingExamsCount.value = data.totalUncomplete;
        createChart();
    } catch (error) {
        console.error("Error fetching statistics:", error);
    }
};

const createChart = () => {
    const ctx = document.getElementById('examStatsChart').getContext('2d');
    if (examStatsChart) {
        examStatsChart.destroy();
    }

    examStatsChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Total Candidate', 'Completed', 'Uncompleted'],
            datasets: [{
                label: "Data",
                data: [candidatesCount.value, completedExamsCount.value, pendingExamsCount.value],
                backgroundColor: ['#f55a5a', '#34D399', '#FBBF24'],
                borderColor: ['#f55a5a', '#10B981', '#F59E0B'],
                borderWidth: 0
            }]
        },
        options: {
            scales: {
                x: {
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)',
                        lineWidth: 2
                    }
                },
                y: {
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)',
                        lineWidth: 2
                    }
                }
            },
            plugins: {
                legend: {
                    display: true
                }
            }
        }
    });
};

onMounted(() => {
    getStatistics();
});
</script>

<style scoped>
canvas {
    width: 100% !important;
    height: auto !important;
}
</style>