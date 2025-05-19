import { Row } from "@tanstack/react-table";

import { useTable } from "@/stores/table-context";
import { CircleX, Pencil } from "lucide-react";


interface DataTableRowActionsProps<T> {
  row: Row<T>;
}

export function DataTableRowActions<T>({
  row,
}: Readonly<DataTableRowActionsProps<T>>) {
  const { setOpen, setCurrentRow } = useTable();

  return (
    <div className="flex items-center gap-x-4">
      <button
        onClick={() => {
          setCurrentRow(row.original);
          setOpen("delete");
        }}
      >
        <Pencil size={16} />
      </button>
      <button
        className="!text-red-500"
        onClick={() => {
          setCurrentRow(row.original);
          setOpen("delete");
        }}
      >
        <CircleX size={16} />
      </button>
    </div>
  );
}
