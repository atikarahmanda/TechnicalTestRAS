<template>
    <div v-if="items.length" class="text-md">
        <div class="flex justify-between items-center mb-4">
            <div class="relative group">
                <button v-if="context === 'managecandidate'" @click="openModal('register')"
                    class="btn-main-admin tooltip tooltip-right" data-tip="Register Candidate">
                    <i class="fa-solid fa-plus text-white text-xl"></i>
                </button>
                <!-- <div 
                    class="absolute top-full transform mb-2 hidden group-hover:flex bg-white text-black text-sm py-1 px-2 rounded shadow-lg whitespace-nowrap">
                    Register Candidate
                </div> -->
            </div>
            <div class="flex gap-x-3">
                <div class="flex rounded-md p-2 cursor-pointer bg-gray-300 hover:bg-red-700 hover:text-white"
                    @click="handleExport('pdf')">
                    <img src="../../assets/pdf-icon.png" alt="" class=" w-8 h-6" />
                    <span class="py-0 text-sm justify-center items-center me-2">Export to PDF</span>
                </div>
                <div class="flex rounded-md p-2 cursor-pointer bg-gray-300 hover:bg-green-700 hover:text-white"
                    @click="handleExport('excel')">
                    <img src="../../assets/excel-icon.png" alt="" class="ms-1 w-6 h-6" />
                    <span class="py-0 text-sm justify-center items-center me-2 ms-1">Export to Excel</span>
                </div>
            </div>
        </div>
        <div class="mb-4 flex justify-between items-center">
            <!-- Dropdown untuk memilih jumlah item per halaman -->
            <div class="flex gap-5">
                <div>
                    <label>Show: </label>
                    <select id="itemsPerPage" v-model="itemsPerPage"
                        class="select select-bordered select-sm pointer ml-1 border">
                        <option value="5">5</option>
                        <option value="10">10</option>
                        <option value="15">15</option>
                        <option value="25">25</option>
                    </select>
                </div>
                <!-- <label class=" ml-4">Entries </label> -->
                <div v-if="context === 'managecandidate'">
                    <label>Status : </label>
                    <select v-model="statusFilter" class="select select-bordered select-sm pointer ml-1 border">
                        <option value="all">All</option>
                        <option value="Completed">Completed</option>
                        <option value="Not Completed">Not Completed</option>
                    </select>
                </div>
            </div>

            <!-- Search bar di pojok kanan dengan label sejajar -->
            <div class="ml-auto flex items-center">
                <!-- <label for="search-input" class="mr-2">Search:</label> -->
                <div class="relative">
                    <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
                        <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                            fill="none" viewBox="0 0 20 20">
                            <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="m19 19-4-4m0-7A7 7 0 1 1 1 8a7 7 0 0 1 14 0Z" />
                        </svg>
                    </div>
                    <input type="search" id="search-input" v-model="searchQuery"
                        class="block w-full p-1 pl-10 text-sm text-black border border-black rounded-lg bg-white focus:ring-blue-500 focus:border-blue-500 dark:focus:ring-blue-500 dark:focus:border-blue-500"
                        placeholder="Search data..." />
                </div>
            </div>
        </div>

        <div ref="tableContainer" class="overflow-hidden rounded-lg bg-white">
            <table
                class="min-w-full table-auto  text-sm text-left text-gray-500 border-collapse border border-gray-300">
                <thead class="text-xs text-gray-700 uppercase bg-white">
                    <tr>
                        <th scope="col" class="px-6 py-3 border-b w-auto">No</th>
                        <th v-for="(column, key) in columns" :key="key" scope="col"
                            class="px-6 py-3 border-b cursor-pointer relative">
                            <span @click="column === 'Status Test' ? toggleStatusFilter() : toggleSort(column)">
                                {{ column }}
                            </span>
                            <span v-if="sortColumn === column">
                                <span v-if="sortDirection === 'asc'">▲</span>
                                <span v-else>▼</span>
                            </span>
                        </th>

                        <th v-if="context === 'manageaccess' || context === 'managecandidate'" scope="col"
                            class="px-6 py-3 border-b">
                            Action
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in paginatedItems" :key="item.id"
                        class="bg-white border-b hover:bg-gray-100">
                        <td class="px-6 py-4 font-medium text-black whitespace-nowrap border-b">
                            {{ (currentPage - 1) * itemsPerPage + index + 1 }}
                        </td>
                        <td v-for="(value, key) in filteredItem(item)" :key="key" class="px-6 py-4 text-black border-b">
                            <span v-if="key === 'Status Test'" :class="{
                                'bg-green-500 text-white': value === 'Completed',
                                'bg-red-500 text-white': value === 'Not Completed'
                            }" class="px-3 py-1 rounded-full flex items-center justify-center text-center">
                                {{ value }}
                            </span>
                            <span v-else-if="key === 'deadline'" :class="{
                                'text-black': filteredItem(item)['Status Test'] === 'Completed' ||
                                    (stripTime(new Date(value)) >= stripTime(new Date()) && filteredItem(item)['Status Test'] === 'Not Completed'),
                                'text-red-500 font-semibold': stripTime(new Date(value)) < stripTime(new Date()) && filteredItem(item)['Status Test'] === 'Not Completed'
                            }">
                                {{ new Date(value).toLocaleDateString('id-ID') }}
                            </span>

                            <span v-else>
                                {{ value }}
                            </span>
                        </td>
                        <td v-if="context === 'manageaccess'" class="px-6 py-4 text-black border-b">
                            <div class="relative group">
                                <button @click="openModal('deadline', item)" class="btn-sm btn-primary tooltip"
                                    data-tip="Edit Data">
                                    <i class="fas fa-edit fa-lg ml-1 text-[#34D399] cursor-pointer"></i>
                                </button>
                            </div>
                        </td>
                        <td v-if="context === 'managecandidate'" class="px-6 py-4 text-black border-b">
                            <div class="relative group">
                                <button @click="handleDelete(item.userId)" class="btn-sm btn-danger tooltip"
                                    data-tip="Delete Data">
                                    <i class="fas fa-trash fa-lg ml-1 text-[#f55a5a] cursor-pointer"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Menampilkan informasi di bawah tabel -->
        <div class="flex justify-between items-center mt-4">
            <span>
                Showing {{ (currentPage - 1) * itemsPerPage + 1 }} to
                {{ (currentPage - 1) * itemsPerPage + paginatedItems.length }} of
                {{ filteredItems.length }} entries
            </span>

            <!-- Pagination -->
            <div class="flex items-center space-x-1">
                <button @click="prevPage" class="btn-before-after">
                    Previous
                </button>

                <button v-for="page in totalPages" :key="page" @click="goToPage(page)"
                    :class="['flex items-center py-2 px-4 rounded font-medium select-none transition-colors', currentPage === page ? 'bg-red-500 text-white hover:bg-red-600' : 'bg-white text-black hover:bg-gray-100']">
                    {{ page }}
                </button>

                <button @click="nextPage" class="btn-before-after">
                    Next
                </button>
            </div>
        </div>
    </div>

    <div v-else class="flex flex-col items-center justify-center space-y-4">
        <p class="text-gray-600 text-lg">No data available.</p>
        <div class="relative group">
            <button v-if="context === 'managecandidate'" type="button" @click="openModal('register')"
                class="btn-main-admin tooltip tooltip-right" data-tip="Register Candidate">
                +
            </button>
        </div>
    </div>
    <ModalRegister v-if="isModalOpen && modalType === 'register'" @close="closeModal" @register="handleSubmit"
        @reloadData="handleReloadData" />
</template>

<script setup>
import { ref, computed } from "vue";
import ModalRegister from "./modalregister.vue";
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import * as XLSX from 'xlsx';
import { deleteCandidate } from '../../api/adminApi';
import Swal from 'sweetalert2/dist/sweetalert2';

const props = defineProps(["items", "context", "columns", "hiddenColumns"]);
const emit = defineEmits(["openModal", "delete", "reloadData"]);
const searchQuery = ref("");
const currentPage = ref(1);
const itemsPerPage = ref(10);
const isModalOpen = ref(false);
const modalType = ref("");
const selectedItem = ref(null);
const tableContainer = ref(null);
const tableName = props.context;
const currentDate = new Date();
const formattedDate = currentDate.toISOString().split('T')[0];
const formattedTime = currentDate.toTimeString().split(' ')[0];
const sortColumn = ref(null);
const sortDirection = ref(null);
const statusFilter = ref("all");

const statusFilterVisible = ref(false);

const toggleStatusFilter = () => {
    statusFilterVisible.value = !statusFilterVisible.value;
};

function stripTime(date) {
    date.setHours(0, 0, 0, 0); // Menghapus komponen waktu dari tanggal
    return date;
}

const toggleSort = (column) => {
    if (sortColumn.value === column) {
        // Toggle direction
        sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
    } else {
        // Set new column and default to ascending
        sortColumn.value = column;
        sortDirection.value = 'asc';
    }
};

const filteredItems = computed(() => {
    const searchFields = Object.keys(props.items[0] || {});
    return props.items.filter((item) => {
        if (
            statusFilter.value !== "all" &&
            item["Status Test"] !== statusFilter.value
        ) {
            return false;
        }

        return searchFields.some((field) => {
            const value = item[field];
            return (
                value &&
                typeof value === "string" &&
                value.toLowerCase().includes(searchQuery.value.toLowerCase())
            );
        });
    });
});

const sortedItems = computed(() => {
    if (!sortColumn.value || !sortDirection.value) return filteredItems.value;

    return [...filteredItems.value].sort((a, b) => {
        const aValue = a[sortColumn.value];
        const bValue = b[sortColumn.value];

        if (aValue == null || bValue == null) return 0; // Null-safe
        if (typeof aValue === 'string' && typeof bValue === 'string') {
            return sortDirection.value === 'asc'
                ? aValue.localeCompare(bValue)
                : bValue.localeCompare(aValue);
        }

        return sortDirection.value === 'asc' ? aValue - bValue : bValue - aValue;
    });
});

const handleDelete = (id) => {
    Swal.fire({
        title: 'Are you sure?',
        text: "This action cannot be undone!",
        icon: 'warning',
        confirmButtonText: 'Yes, delete it!'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                const token = localStorage.getItem('token')
                await deleteCandidate(token, id);
                emit('reloadData');
            } catch (error) {
                console.error("Error during deletion:", error);
            }
        }
    })
};

const filteredItem = (item) => {
    // Filter item properties based on visible columns
    return Object.fromEntries(
        Object.entries(item).filter(([key]) => !props.hiddenColumns.includes(key))
    );
};

const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage.value));

const paginatedItems = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage.value;
    return sortedItems.value.slice(start, start + itemsPerPage.value);
});

const openModal = (type, item = null) => {
    modalType.value = type;
    isModalOpen.value = true;
    selectedItem.value = item;
    emit("openModal", type, item);
};

const closeModal = () => {
    isModalOpen.value = false;
    modalType.value = "";
};

const handleSubmit = (candidateData) => {
    console.log("New Candidate:", candidateData);
    closeModal();
    emit("reloadData");
};

const nextPage = () => {
    if (currentPage.value < totalPages.value) {
        currentPage.value++;
    }
};

const handleReloadData = () => {
    console.log('Reload data event received');
    emit('reloadData');
}

const prevPage = () => {
    if (currentPage.value > 1) {
        currentPage.value--;
    }
};

const goToPage = (page) => {
    currentPage.value = page;
};

const handleExport = (exportType) => {
    if (exportType === 'pdf') {
        exportToPDF();
    } else if (exportType === 'excel') {
        exportToExcel();
    }
};

const exportToPDF = () => {
    const doc = new jsPDF({ orientation: 'landscape' });
    const columns = props.columns.filter(column => !props.hiddenColumns.includes(column));
    const data = props.items.map(item =>
        columns.map(column => item[column] ?? "")
    );
    doc.autoTable({
        head: [columns],
        body: data,
        startY: 20,
        theme: 'grid',
        headStyles: { fillColor: [165, 42, 42] },
        styles: { fontSize: 10 },
    });
    const filename = `${props.context}_${new Date().toISOString().split('T')[0]}.pdf`;
    doc.save(filename);
};


const exportToExcel = () => {
    const filename = `${tableName}_${formattedDate}_${formattedTime}.xlsx`;
    const workbook = XLSX.utils.book_new();
    const visibleColumns = props.columns.filter(column => !props.hiddenColumns.includes(column));
    const worksheetData = [
        visibleColumns,
        ...props.items.map(item => {
            return visibleColumns.map(column => item[column]);
        })
    ];
    const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
    XLSX.utils.book_append_sheet(workbook, worksheet);
    XLSX.writeFile(workbook, filename);
};

</script>
<style scoped>
table {
    width: 100%;
    border-collapse: collapse;
    border-radius: 10px;
}

th,
td {
    padding: 12px 16px;
    border: 1px solid #A9A9A9;
}

tr:hover {
    background-color: #f1f1f1;
}

th {
    background-color: brown;
    color: #fff;
}

table tr td:nth-child(6) {
    text-align: center;
}
</style>