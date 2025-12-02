<!-- src/components/TodoForm.vue -->
<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { TodoCreateRequest } from '@/types/todo'

interface Props {
  initial?: TodoCreateRequest | null
  pending?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  initial: null,
  pending: false,
})

const emit = defineEmits<{
  submit: [payload: TodoCreateRequest]
  cancel: []
}>()

const title = ref('')
const description = ref<string | null>(null)
const isDone = ref(false)
const dueDate = ref<string | null>(null)
const localError = ref<string | null>(null)

// initial が変わったらフォームに反映
watch(
  () => props.initial,
  (value) => {
    title.value = value?.title ?? ''
    description.value = value?.description ?? null
    isDone.value =  false
    dueDate.value = value?.dueDate ?? null
  },
  { immediate: true }
)

const isDisabled = computed(() => props.pending === true)

const onSubmit = () => {
  // 簡易バリデーション（とりあえずタイトル必須だけ）
  if (!title.value.trim()) {
    localError.value = 'タイトルは必須です'
    return
  }
  localError.value = null

  const payload: TodoCreateRequest = {
    title: title.value.trim(),
    description: description.value,
    dueDate: dueDate.value,
  }

  emit('submit', payload)
}

const onCancel = () => {
  emit('cancel')
}
</script>

<template>
  <form @submit.prevent="onSubmit">
    <div>
      <label>
        タイトル
        <input v-model="title" type="text" />
      </label>
    </div>

    <div>
      <label>
        説明
        <textarea v-model="description" />
      </label>
    </div>

    <div>
      <label>
        期限
        <input v-model="dueDate" type="date" />
      </label>
    </div>

    <div>
      <label>
        完了
        <input v-model="isDone" type="checkbox" />
      </label>
    </div>

    <p v-if="localError" style="color: red">
      {{ localError }}
    </p>

    <div>
      <button type="submit" :disabled="isDisabled">
        {{ pending ? '送信中...' : '保存' }}
      </button>
      <button type="button" @click="onCancel" :disabled="isDisabled">
        キャンセル
      </button>
    </div>
  </form>
</template>
