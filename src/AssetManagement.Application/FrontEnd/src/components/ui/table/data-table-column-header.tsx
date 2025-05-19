import { Column } from "@tanstack/react-table";
import { ChevronDown, ChevronsUpDown, ChevronUp } from "lucide-react";

interface DataTableColumnHeaderProps<TData, TValue>
  extends React.HTMLAttributes<HTMLDivElement> {
  column: Column<TData, TValue>;
  title: string;
}

export function DataTableColumnHeader<TData, TValue>({
  column,
  title,
  className,
}: DataTableColumnHeaderProps<TData, TValue>) {
  if (!column.getCanSort()) {
    return <div className={`${className}`}>{title}</div>;
  }

  return (
    <button
      className="data-[state=open]:bg-accent flex h-8 items-center gap-x-2"
      onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
    >
      <span>{title}</span>
      {column.getIsSorted() === "desc" ? (
        <ChevronDown className="size-4" />
      ) : column.getIsSorted() === "asc" ? (
        <ChevronUp className="size-4" />
      ) : (
        <ChevronsUpDown className="size-4" />
      )}
    </button>
  );
}
