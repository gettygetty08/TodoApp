<script setup lang="ts">
import { useTodos } from '@/composables/useTodos'
import TodoForm from '@/pages/TodoForm.vue'
import type { TodoCreateRequest } from '@/types/todo'

const { todos, loading, error, loadTodos } = useTodos()

const handleSubmit = async (payload: TodoCreateRequest) => {
  await create(payload)   // ここはあとで useTodos に実装
  await loadTodos()
}
</script>

<template>
  <main>
    <h1>Todo 一覧</h1>

    <TodoForm :pending="loading" @submit="handleSubmit"/>

    <button type="button" @click="loadTodos" :disabled="loading">
        再読込
    </button>
    <p v-if="loading">読み込み中...</p>
    <p v-else-if="error">{{ error }}</p>
    <ul v-else>
      <li v-for="todo in todos" :key="todo.id">
        {{ todo.title }}
      </li>
    </ul>
  </main>
</template>
