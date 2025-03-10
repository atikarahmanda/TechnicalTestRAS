<template>
    <div class="relative overflow-x-auto shadow-md sm:rounded-lg py-6 px-6">
        <Table :items="users" :hiddenColumns="['userId']" :columns="columns" :context="'manageaccess'"
            @openModal="openModal"> </Table>
        <EditDeadlineModal v-if="isModalOpen" :item="selectedUser" @close="isModalOpen = false" @update="handleUpdate"
            @reloadDataDeadline="fetchUsers" />
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import Table from "../../components/Utils/table.vue";
import EditDeadlineModal from "../../components/Utils/modaleditdeadline.vue";
import axios from 'axios';

const users = ref([]);
const columns = ["email", "fullname", "deadline", "Status Test"];
const isModalOpen = ref(false);
const selectedUser = ref(null);

const fetchUsers = async () => {
    try {
        const token = localStorage.getItem('token');
        const response = await axios.get("https://localhost:7090/api/User/PastDeadline", {
            headers: { Authorization: `Bearer ${token}` }
        });
        users.value = response.data.data.map(user => ({
            userId: user.user_id,
            email: user.email,
            fullname: user.fullname,
            deadline: new Date(user.deadline).toLocaleDateString(),
            "Status Test": user.statusTest ? "Completed" : "Not Completed"
        }));
    } catch (error) {
        console.error("Error fetching users:", error);
    }
};

onMounted(() => {
    fetchUsers();
});

const openModal = (type, user) => {
    if (type === 'deadline' && user) {
        selectedUser.value = user;
        isModalOpen.value = true;
    }
};

const handleUpdate = (updatedUser) => {
    const index = users.value.findIndex(user => user.user_id === updatedUser.user_id);
    if (index !== -1) {
        users.value[index].deadline = new Date(updatedUser.deadline).toLocaleDateString();
    }
    isModalOpen.value = false;
};
</script>
