<script setup lang="ts">
const appTitle = 'Todo フロント（仮）'
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL
import axios from 'axios'
import { ref, onMounted  } from 'vue'

interface Todo {
  id: number
  title: string
  description: string | null
  isDone: boolean
  dueDate: string | null
  createdAt: string
  updatedAt: string
}

const todos = ref<Todo[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    const response = await axios.get<Todo[]>(`${apiBaseUrl}/api/Todos`)
    todos.value = response.data
  } catch (err: any) {
    error.value = err.message ?? 'データ取得に失敗しました'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <main class="app">
      <h1>{{ appTitle }}</h1>
      <!-- ロード中 -->
      <p v-if="loading">読み込み中...</p>

      <!-- エラー -->
      <p v-if="error" class="error">{{ error }}</p>

      <!-- 一覧 -->
      <ul v-if="!loading && !error">
        <li v-for="todo in todos" :key="todo.id">
          <strong>{{ todo.title }}</strong>
          <span v-if="todo.isDone">(完了)</span>
          <div class="desc" v-if="todo.description">{{ todo.description }}</div>
        </li>
      </ul>

  </main>
</template>

<style scoped>
.app {
  min-height: 100vh;
  padding: 2rem;
  font-family:
    system-ui,
    -apple-system,
    BlinkMacSystemFont,
    'Segoe UI',
    sans-serif;
}

.app-header {
  margin-bottom: 2rem;
}

.subtitle {
  margin: 0.25rem 0;
  color: #666;
}

.content {
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 8px;
}
</style>
