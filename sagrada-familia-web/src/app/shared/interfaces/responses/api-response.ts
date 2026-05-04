export interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
    errors: string[];
}

export interface PagedResponse<T> {
    success: boolean;
    message: string;
    data: T[];
    totalItems: number;
    page: number;
    pageSize: number;
    totalPages: number;
}