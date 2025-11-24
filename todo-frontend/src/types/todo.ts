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
