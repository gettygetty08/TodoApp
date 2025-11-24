<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { fetchTodos } from '@/api/todos'
import type { TodoDto } from '../types/todo'

const todos = ref<TodoDto[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    todos.value = await fetchTodos()
  } catch {
    error.value = '一覧取得に失敗しました'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <main>
    <h1>Todo 一覧</h1>
    <p v-if="loading">読み込み中...</p>
    <p v-else-if="error">{{ error }}</p>
    <ul v-else>
      <li v-for="todo in todos" :key="todo.id">
        {{ todo.title }}
      </li>
    </ul>
  </main>
</template>
