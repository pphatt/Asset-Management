import { Button } from "@/components/ui/button";

import { DataTableFacetedFilter } from "./data-table-faceted-filter";

import { FilterTable } from "./filter-table";
import { useTable } from "@/stores/table-context";
import { Input } from "../forms/input";

import { Cross, Search } from "lucide-react";

interface DataTableToolbarProps<T> {
  readonly filter: FilterTable<T>;
}

export function DataTableToolbar<T>({ filter }: DataTableToolbarProps<T>) {
  const { table } = useTable();
  const isFiltered = table ? table.getState().columnFilters.length > 0 : false;

  const resetFilter = () => {
    table?.resetColumnFilters();
    filter.facedFilter?.forEach((data) => {
      data.setSelect?.([]);
      data.setSingleSelect?.("");
    });
  };

  return (
    <div className="flex items-center justify-between">
      <div className="flex flex-1 flex-col-reverse items-start gap-y-2 sm:flex-row sm:items-center sm:space-x-2">
        <div className="relative">
          <Input
            className="h-8 w-[25vw] lg:w-[250px] border-gray-400"
            value={filter.search ?? ""}
            onChange={(event) => filter.setSearch?.(event?.target?.value ?? "")}
          />
          <div className="h-full border-l-1 pl-1 absolute top-0 right-3 flex items-center">
            <Search size={16} />
          </div>
        </div>
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
        {isFiltered && (
          <Button
            className="h-8 px-2 lg:px-3"
            onClick={resetFilter}
          >
            Reset
            <Cross className="ml-2 size-4" />
          </Button>
        )}
      </div>
    </div>
  );
}
