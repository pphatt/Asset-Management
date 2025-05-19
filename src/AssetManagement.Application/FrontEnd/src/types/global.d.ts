declare global {
  interface PaginatedResult<T = any> {
    items: T[];
    paginationMetadata: {
      pageSize: number;
      currentPage: number;
      totalItems: number;
      totalPages: number;
      hasNextPage: boolean;
      hasPreviousPage: boolean;
    };
  }

  interface HttpResponse<T = any> {
    data: T;
    success: boolean;
    message: string;
    paginatedResult?: PaginatedResult<T>;
    error?: string[];
  }
}

export { };

