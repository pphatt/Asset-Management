import { Dispatch, SetStateAction } from "react";

import { usePagination } from "@/hooks/use-pagination";

export interface DataFacedFilter {
  name: string;
  list: {
    label: string;
    value: string;
  }[];
  setSelect?: (value: string[]) => void; // Multi-select
  setSingleSelect?: (value: string) => void; // Single-select
}

export interface FilterTable<T> {
  data: T[];
  pagination: ReturnType<typeof usePagination>;
  placeholder: string;
  isLoading: boolean;
  search?: string;
  setSearch?: Dispatch<SetStateAction<string>>;
  facedFilter?: DataFacedFilter[];
}
