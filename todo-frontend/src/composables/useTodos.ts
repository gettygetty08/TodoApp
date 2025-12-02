// 仮: src/composables/useTodos.ts
import { ref, onMounted } from 'vue'
import { fetchTodos } from '@/api/todos'
import type { TodoDto } from '@/types/todo'

export const useTodos = () => {
  const todos = ref<TodoDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const loadTodos = async () => {
    loading.value = true
    error.value = null
    try {
      todos.value = await fetchTodos()
    } catch {
      error.value = '一覧取得に失敗しました'
    } finally {
      loading.value = false
    }
  }

  onMounted(() => {
    void loadTodos()
  })

  // コンポーネント側に渡したいものを返す
  return {
    todos,
    loading,
    error,
    loadTodos,
  }
}
