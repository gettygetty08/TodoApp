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

export interface ProblemDetails{
    type?: string
    title?: string
    status?: number
    detail?: string
    instance?: string
     // その他のプロパティも来る可能性があるので緩めておく
    [key: string]: any
}

export interface ValidationProblemDetails extends ProblemDetails{
    errors: Record<string,string[]>
}

export class ApiError extends Error{
    status?: number
    problem?: ProblemDetails

    constructor(message: string, status?: number, problem?: ProblemDetails){
        super(message)
        this.name = 'ApiError'
        this.status = status
        this.problem = problem
    }

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

//レスポンスエラーをApiErrorに変換
client.interceptors.response.use(
    (response) => response,
    (error) => {
        //ネットワークエラーなど
        if(!error.response){
            return Promise.reject(new ApiError('ネットワークエラーが発生しました。'))
        }

        const status = error.response.status as number
        const data = error.response.data as ProblemDetails | undefined

        const message = (data && data.title) || (status ? `Request failed with status ${status}` : 'Request failed')

        return Promise.reject(new ApiError(message,status,data))
    }
)

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