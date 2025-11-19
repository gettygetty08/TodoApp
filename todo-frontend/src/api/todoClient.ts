import axios from 'axios'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL

export interface TodoDto {
  id: number
  title: string
  description: string | null
  isDone: boolean
  dueDate: string | null
  createdAt: string
  updatedAt: string
}

//新規登録時のカタチ（TodoCreateRequest）
export interface TodoCreateRequest {
  title: string
  description?: string | null
  dueDate: string | null
}


// 更新時の形（TodoUpdateRequest）
export interface TodoUpdateRequest{
    title : string
    description? : string | null
    isDone : boolean
    dueDate? : string | null
}

const client = axios.create({
    baseURL: apiBaseUrl
})

export const fetchTodos = async(): Promise<TodoDto[]> => {
    const res = await client.get<TodoDto[]>('/api/Todos')
    return res.data
}

export const createTodo = async(payload: TodoCreateRequest) : Promise<TodoDto> => {
    const res = await client.post<TodoDto>('/api/Todos', payload)
    return res.data
}

export const updateTodo = async(id:number,payload: TodoUpdateRequest) : Promise<TodoDto> => {
    const res = await client.put<TodoDto>(`/api/Todos/${id}`,payload)
    return res.data
}

export const deleteTodo = async(id:number) : Promise<void> =>{
   await client.delete<void>(`/api/Todos/${id}`)
}