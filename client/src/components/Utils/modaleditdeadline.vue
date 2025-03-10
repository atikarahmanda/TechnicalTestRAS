<template>
    <div class="fixed inset-0 flex justify-center items-center z-50 bg-gray-500 bg-opacity-50" @click.self="closeModal">
        <div class="bg-white p-6 rounded-lg shadow-md w-1/3">
            <!-- <h2 class="text-lg font-semibold mb-4">Edit Deadline</h2> -->
            <div class="flex justify-between items-center py-3 px-2 border-b">
                <h3 id="hs-vertically-centered-modal-label" class="font-bold text-gray-800 text-xl">
                Edit Deadline
                </h3>
                <button type="button" class="size-8 inline-flex justify-center items-center gap-x-2 rounded-full text-gray-800" @click="closeModal">
                <span class="sr-only">Close</span>
                <svg class="shrink-0 size-4" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M18 6 6 18"></path>
                    <path d="m6 6 12 12"></path>
                </svg>
                </button>
            </div>
            <form @submit.prevent="updateDeadline">
                <!-- <label for="deadline" class="block text-sm">add number of days:</label> -->
                <div class="mb-10 mt-8 flex gap-3">
                    <input
                        v-model="deadline"
                        type="number"
                        id="deadline"
                        class="w-full border p-2 rounded"
                        min="1"
                        required
                    />
                    <p class=" justify-center items-center my-auto">Days</p>
                </div>

                <div class="flex justify-end">
                    <button
                        type="submit"
                        class="btn-main-admin"
                    >
                        Update
                    </button>
                </div>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue';
import Swal from 'sweetalert2';

const emit = defineEmits();

const props = defineProps({
    item: {
        type: Object,
        required: true, 
    }
});


const deadline = ref(3); 

const updateDeadline = async () => {
    if (!deadline.value) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Please provide a valid deadline.',
        });
        return;
    }

    try {
        Swal.fire({
            title: 'Loading...',
            text: 'Please wait while we update the deadline.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        const response = await fetch(`https://localhost:7090/api/User/UpdateDeadline/${props.item.userId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
            },
            body: deadline.value.toString(), 
        });

        const result = await response.json();

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Success',
                text: 'Deadline updated successfully.',
            });

            closeModal(); 
            emit('reloadDataDeadline');
            emit('update', result.data);
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: result.message || 'An error occurred while updating the deadline.',
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'An error occurred while updating the deadline.',
        });
    }
};

const closeModal = () => {
    emit('close');
};
</script>

<style scoped>
</style>
