import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../store/auth';
import Login from '../views/auth/Login.vue';
import ManageCandidate from '../views/admin/ManageCandidate.vue';
import Test from '../views/candidate/Test.vue';
import ManageAccess from '../views/admin/ManageAccess.vue';
import MainLayout from '../layouts/MainLayout.vue';
import ManageResult from '../views/admin/ManageResult.vue';
import InputQuestion from '../views/admin/InputQuestion.vue';
import FinishedTest from '../views/candidate/FinishedTest.vue';
import ErrorLayout from '../layouts/ErrorLayout.vue';
import NotFound from '../views/NotFound.vue';
import ExpiredTest from '../views/candidate/ExpiredTest.vue';
import { ref } from 'vue';
import Dashboard from '../views/admin/Dashboard.vue';
import Question from '../views/admin/Question.vue';

const brandName = "BeMind";
const routes = [
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: {
      title: brandName+'- Login'
    }
  },
  {
    path: '/',
    component: MainLayout,
    children: [
      {
        path: '/dashboard',
        name: 'Dashboard',
        component: Dashboard,
        meta: { requiresAuth: true, role: 'Admin',
          title: brandName+'- Dashboard'
        }
      },
      {
        path: '/dashboard/managecandidate',
        name: 'ManageCandidate',
        component: ManageCandidate,
        meta: { requiresAuth: true, role: 'Admin',
          title: brandName+'- Manage Candidate'

         }
      },
      {
        path: 'dashboard/manageaccess',
        name: 'ManageAccess',
        component: ManageAccess,
        meta: { requiresAuth: true, role: 'Admin',
          title:brandName+'- Manage Access'
         }
      },
      {
        path: 'dashboard/manageresult',
        name: 'ManageResult',
        component: ManageResult,
        meta: { requiresAuth: true, role: 'Admin',
          title:brandName+'- Manage Result'
         }
      },
      {
        path: 'dashboard/inputquestion',
        name: 'InputQuestion',
        component: InputQuestion,
        meta: { requiresAuth: true, role: 'Admin',
          title:brandName+'- Input Question'
         }
      },
      {
        path: 'dashboard/question',
        name: 'Question',
        component: Question,
        meta: { requiresAuth: true, role: 'Admin',
          title:brandName+'- Question'
         }
      },
      {
        path: 'user',
        name: 'Test',
        component: Test,
        meta: { requiresAuth: true, role: 'Candidate',
          title:brandName+'- Technical Test'
         }
      },
      {
        path: 'user',
        name: 'ExpiredTest',
        component: ExpiredTest,
        meta: { requiresAuth: true, role: 'Candidate' }
      },
      {
        path: 'finished',
        name: 'FinishedTest',
        component: FinishedTest,
        meta: { requiresAuth: true, role: 'Candidate',
          title:brandName+'- Finished'
         }
      }
    ]
  },
  {
    path: '/',
    component: ErrorLayout,
    children: [
      {
        path: 'notfound',
        name: 'NotFound',
        component: NotFound
      }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/notfound'
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// MIDDLEWARE
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const isAuthenticated = authStore.isAuthenticated();
  const isCompleted = authStore.isCompleted;
  const isDeadline = authStore.isDeadline;
  const finished = ref(localStorage.getItem('finished') === 'true');

  // console.log(isCompleted)
  if (to.meta.title) {
    document.title = to.meta.title;
  }
  
  if (to.path === '/' && !isAuthenticated) {
    return next('/login');
  }

  if (authStore.userRole === 'Candidate') {
    if (isDeadline === 'True' && to.name !== 'ExpiredTest') {
      return next({ name: 'ExpiredTest' });
    }
  
    if (isDeadline === 'False') {
      if (isCompleted === 'True' || finished.value) {
        if (to.path !== '/finished') {
          return next('/finished');
        }
      } else if(to.name !== 'Test') {
        return next({ name: 'Test' });
      }
    }
  }
  

  //user akses '/' tapi sudah login
  if (to.path === '/' && isAuthenticated) {
    const userRole = authStore.userRole;
    if (userRole === 'Admin') {
      return next('/dashboard');
    } else if (userRole === 'Candidate') {
      return isDeadline === 'True' ? next({ name: 'ExpiredTest' }) : next({ name: 'Test' })
    }    
  }

  // Jika rute memerlukan otentikasi dan pengguna tidak memiliki token, redirect ke login
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!isAuthenticated) {
      return next('/login');
    } else {
      const userRole = authStore.userRole;
      // Jika pengguna memiliki token tetapi peran tidak sesuai, redirect ke halaman login
      if (to.meta.role && to.meta.role !== userRole) {
        return next('/login');
      }
    }
  }

  next();
});

export default router;