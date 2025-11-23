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
  ApiError,
  type ValidationProblemDetails,
} from './api/todoClient'
// import { isDotDotDotToken } from 'typescript'

const todos = ref<TodoDto[]>([])
const loading = ref(true)

//画面上部などに出す「全体メッセージ」
const globalError = ref<string | null>(null)

//フォーム用のエラー（フィールド -> メッセージ配列)
const formErrors = ref<Record<string, string[]>>({})

const handleApiError = (err: unknown, fieldScope: 'form' | 'global' = 'global') => {
  if (!(err instanceof ApiError)) {
    //想定外（コードバグやライブラリの例外など）
    globalError.value = '不明なエラーが発生しました。'
    console.error(err)
    return
  }

  const status = err.status
  const problem = err.problem

  //まずフォーム側のエラーを一旦リセット
  if (fieldScope === 'form') {
    formErrors.value = {}
  }

  if (status === 400 && problem && 'errors' in problem && fieldScope === 'form') {
    const vpd = problem as ValidationProblemDetails
    formErrors.value = vpd.errors ?? {}
    globalError.value = null
    return
  }

  if (status === 404) {
    globalError.value = problem?.detail ?? '対象のデータが見つかりませんでした。'
    return
  }

  if (status && status >= 500) {
    globalError.value = problem?.detail ?? 'サーバーでエラーが発生しました。'
    return
  }

  // その他（401/403/409など）必要に応じて増やす
  globalError.value = err.message || 'エラーが発生しました。'
}

const loadTodos = async () => {
  try {
    loading.value = true
    globalError.value = null
    todos.value = await fetchTodos()
  } catch (err) {
    console.error(err)
    handleApiError(err, 'global')
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
    // ここはフロント側の簡易チェック（サーバにもRequiredはある）
    formErrors.value = { Title: ['タイトルは必須です。'] }
    return
  }

  try {
    creating.value = true
    globalError.value = null
    formErrors.value = {}

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
  } catch (err) {
    console.error(err)
    handleApiError(err, 'form')
  } finally {
    creating.value = false
  }
}

const updatingIds = ref<Set<number>>(new Set())
const toggleDone = async (todo: TodoDto) => {
  try {
    updatingIds.value.add(todo.id)
    globalError.value = null

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
  } catch (err) {
    console.error(err)
    handleApiError(err, 'global')
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
    globalError.value = null

    await apiDeleteTodo(todo.id)
    // ローカル一覧から除外
    todos.value = todos.value.filter((t) => t.id !== todo.id)
  } catch (err) {
    console.error(err)
    handleApiError(err, 'global')
  } finally {
    deletingIds.value.delete(todo.id)
  }
}

// const todoCopy = (todo: TodoDto, checkbox: HTMLInputElement): TodoDto => {
//   return {
//     ...todo,
//     isDone: checkbox.checked,
//   }
// }
</script>

<template>
  <main class="app">
    <p v-if="globalError" class="error global-error">
      {{ globalError }}
    </p>
    <h1>{{ appTitle }}</h1>
    <section class="create">
      <h2>新規Todo追加</h2>
      <div class="from-row">
        <label>
          タイトル
          <input v-model="newTitle" type="text" placeholder="やることを入力" />
        </label>
        <ul v-if="formErrors.Title" class="field-error">
          <li v-for="msg in formErrors.Title" :key="msg">{{ msg }}</li>
        </ul>
      </div>
      <div class="form-row">
        <label>
          説明
          <textarea v-model="newDescription" rows="2" placeholder="必要なら詳細を入力" />
        </label>
      </div>
      <button :disabled="creating" @click="createTodo">
        {{ creating ? '登録中...' : '追加' }}
      </button>
    </section>

    <section class="list">
      <h2>Todo一覧</h2>
      <p v-if="loading">読み込み中...</p>

      <ul v-if="!loading && !globalError">
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

.global-error {
  margin-bottom: 1rem;
}

.field-error {
  margin: 0.25rem 0 0;
  padding-left: 1.2rem;
  color: #c00;
  font-size: 0.9rem;
}
</style>
