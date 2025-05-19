import { DataTableFacetedFilter } from "./data-table-faceted-filter";

import { FilterTable } from "./filter-table";
import { useTable } from "@/stores/table-context";
import { Input } from "../forms/input";

import { Search } from "lucide-react";

interface DataTableToolbarProps<T> {
  readonly filter: FilterTable<T>;
}

export function DataTableToolbar<T>({ filter }: DataTableToolbarProps<T>) {
  const { table } = useTable();

  return (
    <div className="flex justify-between gap-y-2 sm:flex-row sm:items-center sm:space-x-2 w-[90vw]">
      <div className="flex gap-x-2">
        {filter.facedFilter?.map((data) => (
          <DataTableFacetedFilter
            key={data.name}
            column={table?.getColumn(data.name)}
            options={data.list}
            setSelect={data.setSelect}
            setSingleSelect={data.setSingleSelect}
            title={data.name}
          />
        ))}
      </div>
      <div className="relative">
        <Input
          className="w-[25vw] lg:w-[250px] border-gray-400"
          value={filter.search ?? ""}
          onChange={(event) => filter.setSearch?.(event?.target?.value ?? "")}
        />
        <div className="h-full border-l-1 pl-1 absolute top-0 right-3 flex items-center">
          <Search size={16} />
        </div>
      </div>
    </div>
  );
}
