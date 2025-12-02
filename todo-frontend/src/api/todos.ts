// src/api/todos.ts
import { client } from './client'
import type { TodoDto, TodoCreateRequest,TodoUpdateRequest } from '@/types/todo'

const TODOS_BASE = '/api/Todos'

export const fetchTodos = async (): Promise<TodoDto[]> => {
  const res = await client.get<TodoDto[]>(TODOS_BASE)
  return res.data
}

export const createTodo = async (payload: TodoCreateRequest): Promise<TodoDto> => {
  const res = await client.post<TodoDto>(TODOS_BASE, payload)
  return res.data
}

// update/delete はこのあと足す
export const updateTodo = async(id:number,payload: TodoUpdateRequest) : Promise<TodoDto> => {
    const res = await client.put<TodoDto>(`${TODOS_BASE}/${id}`,payload)
    return res.data
}

export const deleteTodo = async(id:number) : Promise<void> =>{
   await client.delete<void>(`${TODOS_BASE}/${id}`)
}