<template>
    <div class="fixed inset-0 flex justify-center items-center z-50 bg-gray-500 bg-opacity-50" @click.self="closeModal">
        <div class="bg-white px-3 py-4 rounded-lg shadow-md w-1/3">
            <!-- <h2 class="text-lg font-semibold mb-4">Register Candidate</h2> -->

            <!-- HEADER -->
            <div class="flex justify-between items-center py-3 px-2 border-b">
                <h3 id="hs-vertically-centered-modal-label" class="font-bold text-gray-800">
                    Register Candidate
                </h3>
                <button type="button"
                    class="size-8 inline-flex justify-center items-center gap-x-2 rounded-full  text-gray-800"
                    @click="closeModal">
                    <span class="sr-only">Close</span>
                    <svg class="shrink-0 size-4" xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                        stroke-linejoin="round">
                        <path d="M18 6 6 18"></path>
                        <path d="m6 6 12 12"></path>
                    </svg>
                </button>
            </div>


            <form @submit.prevent="handleSubmit" class=" mx-2 mt-6">
                <div class="mb-2">
                    <label for="email" class="block text-sm">Email <span class="text-red-600">*</span></label>
                    <input v-model="email" type="email" id="email" class="w-full border p-1 rounded"
                        @blur="emailTouched = true" />
                    <span class="text-red-500 text-xs italic"
                        :style="{ visibility: emailTouched && !isEmailValid ? 'visible' : 'hidden' }">
                        Masukkan format email yang benar.
                    </span>
                </div>
                <div class="mb-2">
                    <label for="firstName" class="block text-sm">First Name <span class="text-red-600">*</span></label>
                    <input v-model="firstName" type="text" id="firstName" class="w-full border p-1 rounded" required
                        @blur="firstNameTouched = true" />
                    <span class="text-red-500 text-xs italic"
                        :style="{ visibility: firstNameTouched && !firstName ? 'visible' : 'hidden' }">
                        First name wajib diisi.
                    </span>
                </div>
                <div class="mb-2">
                    <label for="lastName" class="block text-sm">Last Name <span class="text-red-600">*</span></label>
                    <input v-model="lastName" type="text" id="lastName" class="w-full border p-1 rounded" required
                        @blur="lastNameTouched = true" />
                    <span class="text-red-500 text-xs italic"
                        :style="{ visibility: lastNameTouched && !lastName ? 'visible' : 'hidden' }">
                        Last Name wajib diisi.
                    </span>
                </div>
                <div class="flex justify-end">
                    <button type="submit" :class="submitButtonClass" :disabled="!formIsValid"
                        class="text-white py-2 px-4 rounded-md font-semibold transition-transform duration-300 ease-in-out">
                        Submit
                    </button>
                </div>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import Swal from 'sweetalert2';
import { useAuthStore } from '../../store/auth';
import { addUser } from '../../api/adminApi';

const emit = defineEmits();
const authStore = useAuthStore();

const emailTouched = ref(false);
const firstNameTouched = ref(false);
const lastNameTouched = ref(false);
const email = ref('');
const firstName = ref('');
const lastName = ref('');


const isEmailValid = computed(() => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email.value);
});

const formIsValid = computed(() => {
    return isEmailValid.value && firstName.value && lastName.value;
});

const submitButtonClass = computed(() => {
    return formIsValid.value
        ? 'bg-red-600 cursor-pointer'
        : 'bg-gray-600 cursor-not-allowed';
});

const handleSubmit = async () => {
    Swal.fire({
        title: 'Loading...',
        text: 'Please wait, we will send email to the candidate',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    try {
        const token = authStore.token;
        console.log(token);
        await addUser(token, email.value, firstName.value, lastName.value);
        closeModal();
        emit('reloadData');
    } catch (error) {
        throw error;
    }
};

const closeModal = () => {
    emit('close');
};
</script>


<style scoped></style>
