<script setup>
const props = defineProps({
    sidebarOpen: Boolean
}) 
</script>

<template>
    <aside class="flex-shrink-0 w-64 flex flex-col border-r transition-all duration-300 bg-white border-gray-200"
        :class="{ '-ml-64': !sidebarOpen }">
        <div class="h-full px-3 pb-4 overflow-y-auto bg-white">
            <ul class="space-y-2 font-medium">
                <li class="pt-3">
                    <div class="pb-4">
                        <img src="../assets/bemind-logo2.png" class="h-10 w-auto" alt="Berca Logo">
                    </div>
                </li>
                <li>
                    <router-link to="/dashboard"
                        class="flex items-center p-2 rounded-lg hover:bg-blue-100  duration-200"
                        exact-active-class="bg-blue-200">
                        <i class="fa-solid fa-tachometer-alt text-blue-600 hover:text-blue-700 fa-lg ml-1"></i>
                        <span class="flex-1 ms-3 text-gray-700 hover:text-blue-600 whitespace-nowrap">Dashboard</span>
                    </router-link>
                </li>
                <li>
                    <router-link to="/dashboard/managecandidate"
                        class="flex items-center p-2 rounded-lg hover:bg-green-100 duration-200"
                        exact-active-class="bg-green-200">
                        <i class="fa-regular fa-user text-green-600 hover:text-green-700 fa-lg ml-1"></i>
                        <span class="ms-3 text-gray-700 hover:text-green-600">All Candidate</span>
                    </router-link>
                </li>
                <li>
                    <router-link to="/dashboard/manageaccess"
                        class="flex items-center p-2 rounded-lg hover:bg-orange-100  duration-200"
                        exact-active-class="bg-orange-200">
                        <i class="fa-solid fa-lock text-orange-600 hover:text-orange-700 fa-lg ml-1"></i>
                        <span class="flex-1 ms-3 text-gray-700 hover:text-orange-600 whitespace-nowrap">Request
                            Access</span>
                    </router-link>
                </li>
                <li>
                    <router-link to="/dashboard/manageresult"
                        class="flex items-center p-2 rounded-lg hover:bg-purple-100 duration-200"
                        exact-active-class="bg-purple-200">
                        <i
                            class="fa-solid fa-square-poll-vertical text-purple-600 hover:text-purple-700 fa-lg ml-1"></i>
                        <span class="flex-1 ms-3 text-gray-700 hover:text-purple-600 whitespace-nowrap">Score</span>
                    </router-link>
                </li>
                <li>
                    <router-link to="/dashboard/inputquestion"
                        class="flex items-center p-2 rounded-lg hover:bg-teal-100  duration-200"
                        exact-active-class="bg-teal-200">
                        <i class="fas fa-question-circle text-teal-600 hover:text-teal-700 fa-lg ml-1"></i>
                        <span class="flex-1 ms-3 text-gray-700 hover:text-teal-600 whitespace-nowrap">Input
                            Question</span>
                    </router-link>
                </li>
                <li>
                    <router-link to="/dashboard/question"
                        class="flex items-center p-2 rounded-lg hover:bg-teal-100 focus:outline-none focus:ring-2 focus:ring-teal-500 transition-colors duration-200"
                        exact-active-class="bg-teal-200">
                        <i class="fas fa-table text-teal-600 hover:text-teal-700 fa-lg ml-1"></i>
                        <span class="flex-1 ms-3 text-gray-700 hover:text-teal-600 whitespace-nowrap">Question List</span>
                    </router-link>
                </li>
            </ul>

        </div>
    </aside>
</template>
