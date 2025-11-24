// src/api/todos.ts
import { client } from './client'
import type { TodoDto, TodoCreateRequest } from '../types/todo'

export const fetchTodos = async (): Promise<TodoDto[]> => {
  const res = await client.get<TodoDto[]>('/api/Todos')
  return res.data
}

export const createTodo = async (payload: TodoCreateRequest): Promise<TodoDto> => {
  const res = await client.post<TodoDto>('/api/Todos', payload)
  return res.data
}

// update/delete はこのあと足す
