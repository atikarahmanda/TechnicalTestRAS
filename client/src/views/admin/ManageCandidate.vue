<template>
    <div class="relative overflow-x-auto shadow-md sm:rounded-lg py-6 px-6">
        <Table :items="users" :columns="columns" :hiddenColumns="['userId']" :context="'managecandidate'"
            @reloadData="fetchUsers" />
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../../store/auth';
import Table from "../../components/Utils/table.vue";
import { fetchAllUsers } from '../../api/adminApi';


const users = ref([]);
const columns = ["email", "fullname", "deadline", 'Status Test'];

const authStore = useAuthStore();
const fetchUsers = async () => {
    try {
        const token = authStore.token;
        const data = await fetchAllUsers(token);
        users.value = data.map(user => ({
            userId: user.user_id,
            email: user.email,
            fullname: user.fullname,
            deadline: new Date(user.deadline).toLocaleDateString(),
            'Status Test': user.statusTest ? "Completed" : "Not Completed"
        }));
    } catch (error) {
        console.error("Error fetching users:", error);
    }
};

onMounted(() => {
    fetchUsers();
});
</script>
