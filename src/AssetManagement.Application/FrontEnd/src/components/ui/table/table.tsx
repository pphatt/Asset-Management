"use client";

import React, { useEffect, useState } from "react";
import {
  ColumnDef,
  ColumnFiltersState,
  RowData,
  SortingState,
  VisibilityState,
  flexRender,
  getCoreRowModel,
  getFacetedRowModel,
  getFacetedUniqueValues,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
} from "@tanstack/react-table";

import { DataTableToolbar } from "./data-table-toolbar";
import { FilterTable } from "./filter-table";
import { useTable } from "@/stores/table-context";
import { DataPagination } from "./data-pagination";
import { Button } from "../button";

declare module "@tanstack/react-table" {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  interface ColumnMeta<TData extends RowData, TValue> {
    className: string;
  }
}

interface DataTableProps<T> {
  columns: ColumnDef<T>[];
  filter: FilterTable<T>;
  title?: string;
  extraButton?: React.ReactNode;
  toolbar?: React.ReactNode;
}

export function DataTable<T>({
  columns,
  filter,
  title,
  extraButton,
  toolbar,
}: DataTableProps<T>) {
  const [rowSelection, setRowSelection] = useState({});
  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({});
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [sorting, setSorting] = useState<SortingState>([]);

  const table = useReactTable({
    data: filter.data,
    columns,
    state: {
      sorting,
      columnVisibility,
      rowSelection,
      columnFilters,
    },
    manualFiltering: true,
    enableRowSelection: true,
    onRowSelectionChange: setRowSelection,
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    onColumnVisibilityChange: setColumnVisibility,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFacetedRowModel: getFacetedRowModel(),
    getFacetedUniqueValues: getFacetedUniqueValues(),
  });
  const { setTable } = useTable();

  useEffect(() => {
    if (setTable) {
      setTable(table);
    }
  }, [table, setTable]);


  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-primary text-2xl font-bold">{title}</h1>
        </div>
        {extraButton}
      </div>
      <div className="bg-white p-4 rounded-md shadow-sm">
        <div className="flex justify-between items-center mb-4 gap-x-6">
          {toolbar ?? <DataTableToolbar filter={filter} />}
          <Button className="w-[10rem]">Create new user</Button>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full bg-white border-collapse">
            <thead className="text-left">
              {table.getHeaderGroups().map((headerGroup) => (
                <tr key={headerGroup.id}>
                  {headerGroup.headers.map((header) => {
                    const text = header.isPlaceholder
                      ? null
                      : flexRender(
                        header.column.columnDef.header,
                        header.getContext(),
                      );

                    return (
                      <th
                        key={header.id}
                        className={`text-left relative font-bold
                            ${header.id !== "actions" ? "after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-900" : ""}
                          `}
                      >
                        {text}
                      </th>
                    );
                  })}
                </tr>
              ))}
            </thead>
            <tbody>
              {table.getRowModel().rows?.length ? (
                table.getRowModel().rows.map((row) => (
                  <tr
                    key={row.id}
                    className="hover:bg-gray-50"
                  >
                    {row.getVisibleCells().map((cell) => {
                      return (
                        <td
                          key={cell.id}
                          className={`py-2 relative
                            ${cell.column.id !== "actions" ? "after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300" : "w-[5vw]"}
                            `}
                        >
                          {flexRender(
                            cell.column.columnDef.cell,
                            cell.getContext(),
                          )}
                        </td>
                      )
                    })}
                  </tr>
                ))
              ) : (
                <tr>
                  <td
                    className="p-4 text-center text-sm text-gray-500"
                    colSpan={columns.length}
                  >
                    No results.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <DataPagination pagination={filter.pagination} />
      </div>
    </div>
  );
}