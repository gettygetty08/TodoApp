<script setup lang="ts">
const appTitle = 'Todo フロント（仮）'
import { ref, onMounted } from 'vue'
import {
  fetchTodos,
  createTodo as apiCreateTodo,
  updateTodo as apiUpdateTodo,
  deleteTodo as apiDeleteTodo,
  type TodoDto,
  type TodoCreateRequest,
  type TodoUpdateRequest,
} from './api/todoClient'
// import { isDotDotDotToken } from 'typescript'

const todos = ref<TodoDto[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const loadTodos = async () => {
  try {
    loading.value = true
    error.value = null
    todos.value = await fetchTodos()
  } catch (err: any) {
    console.error(err)
    error.value = err.message ?? 'Todo一覧の取得に失敗しました'
  } finally {
    loading.value = false
  }
}

onMounted(loadTodos)

const newTitle = ref<string>('')
const newDescription = ref<string>('')

const creating = ref(false)

const createTodo = async () => {
  if (!newTitle.value.trim()) {
    alert('タイトルは必須です。')
    return
  }

  try {
    creating.value = true
    error.value = null

    const payload: TodoCreateRequest = {
      title: newTitle.value,
      description: newDescription.value || null,
      dueDate: null,
    }

    const created = await apiCreateTodo(payload)

    // 末尾に追加
    todos.value.push(created)

    // フォームリセット
    newTitle.value = ''
    newDescription.value = ''
  } catch (err: any) {
    console.error(err)
    error.value = err.message ?? 'Todoの登録に失敗しました'
  } finally {
    creating.value = false
  }
}

const updatingIds = ref<Set<number>>(new Set())
const toggleDone = async (todo: TodoDto) => {
  try {
    updatingIds.value.add(todo.id)
    error.value = null

    const payload: TodoUpdateRequest = {
      title: todo.title,
      description: todo.description,
      isDone: !todo.isDone, // ここで反転
      dueDate: todo.dueDate,
    }

    const updated = await apiUpdateTodo(todo.id, payload)

    // レスポンスで配列を更新
    const index = todos.value.findIndex((t) => t.id === todo.id)
    if (index !== -1) {
      todos.value[index] = updated
    }
  } catch (err: any) {
    console.error(err)
    error.value = err.message ?? 'Todoの更新に失敗しました。'
  } finally {
    updatingIds.value.delete(todo.id)
  }
}

const deletingIds = ref<Set<number>>(new Set())

const deleteTodo = async (todo: TodoDto) => {
  if (!confirm(`「${todo.title}」を削除します。よろしいですか？`)) {
    return
  }

  try {
    deletingIds.value.add(todo.id)
    error.value = null

    await apiDeleteTodo(todo.id)
    // ローカル一覧から除外
    todos.value = todos.value.filter((t) => t.id !== todo.id)
  } catch (err: any) {
    console.error(err)
    error.value = err.message ?? 'Todoの削除に失敗しました'
  } finally {
    deletingIds.value.delete(todo.id)
  }
}

const todoCopy = (todo: TodoDto, checkbox: HTMLInputElement): TodoDto => {
  return {
    ...todo,
    isDone: checkbox.checked,
  }
}
</script>

<template>
  <main class="app">
    <h1>{{ appTitle }}</h1>
    <section class="create">
      <h2>新規Todo追加</h2>
      <div class="from-row">
        <label>
          タイトル
          <input v-model="newTitle" type="text" placeholder="やることを入力" />
        </label>
      </div>
      <div class="form-row">
        <label>
          説明
          <textarea v-model="newDescription" rows="2" placeholder="必要なら詳細を入力" />
        </label>
      </div>
      <button :disabled="creating || !newTitle.trim()" @click="createTodo">
        {{ creating ? '登録中...' : '追加' }}
      </button>
    </section>

    <section class="list">
      <h2>Todo一覧</h2>
      <p v-if="loading">読み込み中...</p>
      <p v-if="error" class="error">{{ error }}</p>

      <ul v-if="!loading && !error">
        <li v-for="todo in todos" :key="todo.id" class="todo-item">
          <label>
            <input
              type="checkbox"
              :checked="todo.isDone"
              :disabled="updatingIds.has(todo.id)"
              @change="toggleDone(todo)"
            />
            <span :class="{ done: todo.isDone }">
              {{ todo.title }}
            </span>
          </label>

          <div v-if="todo.description" class="desc">
            {{ todo.description }}
          </div>

          <button class="danger" :disabled="deletingIds.has(todo.id)" @click="deleteTodo(todo)">
            {{ deletingIds.has(todo.id) ? '削除中...' : '削除' }}
          </button>
        </li>
      </ul>
    </section>
  </main>
</template>

<style scoped>
.app {
  padding: 2rem;
  font-family:
    system-ui,
    -apple-system,
    BlinkMacSystemFont,
    'Segoe UI',
    sans-serif;
}

h1 {
  margin-bottom: 1.5rem;
}

.create,
.list {
  margin-bottom: 2rem;
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.form-row {
  margin-bottom: 0.75rem;
}

input[type='text'],
textarea {
  display: block;
  width: 100%;
  margin-top: 0.25rem;
  padding: 0.5rem;
  box-sizing: border-box;
}

button {
  padding: 0.5rem 1rem;
  cursor: pointer;
}

button:disabled {
  opacity: 0.6;
  cursor: default;
}

.todo-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
}

.todo-item:last-child {
  border-bottom: none;
}

.done {
  text-decoration: line-through;
  color: #777;
}

.desc {
  margin-left: 1.5rem;
  color: #555;
  flex: 1;
}

button.danger {
  margin-left: auto;
  background: #f5e5e5;
}
.error {
  color: red;
}
</style>
