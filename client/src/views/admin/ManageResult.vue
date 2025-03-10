<template>
    <div class="relative overflow-x-auto shadow-md sm:rounded-lg py-6 px-6">
        <Table :items="users" :hiddenColumns="['userId']" :columns="columns" :context="'manageresult'"> </Table>
    </div>
</template>


<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../../store/auth';
import { fetchAllResult } from '../../api/adminApi';
import Table from "../../components/Utils/table.vue";


const users = ref([]);
const columns = ["fullname", "email", "Total Score"];
const authStore = useAuthStore()

const fetchUsers = async () => {
    try {
        const token = authStore.token;
        const data = await fetchAllResult(token)
        users.value = data.map(user => ({
            userId: user.userId,
            fullname: user.fullname,
            email: user.email,
            "Total Score": user.totalScore
        }));
    } catch (error) {
        console.error("Error fetching users:", error);
    }
};

onMounted(() => {
    fetchUsers();
});
</script>

<style lang="scss" scoped></style>
