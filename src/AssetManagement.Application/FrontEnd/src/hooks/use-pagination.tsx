import { Dispatch, SetStateAction, useState } from "react";

interface UsePaginationProps {
  initialPage?: number;
  initialPageSize?: number;
}

export interface Pagination {
  page: number;
  pageSize: number;
  pageCount: number;
  setPage: Dispatch<SetStateAction<number>>;
  setPageSize: Dispatch<SetStateAction<number>>;
  setPageCount: (count: number) => void;
}

export function usePagination({
  initialPage = 1,
  initialPageSize = 10,
}: UsePaginationProps): Pagination {
  const [page, setPage] = useState(initialPage);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [pageCount, setPageCount] = useState(0);

  return {
    page,
    pageSize,
    pageCount,
    setPage,
    setPageSize,
    setPageCount,
  };
}
