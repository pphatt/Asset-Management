import { DataTableColumnHeader } from "@/components/ui/table/data-table-column-header";
import { ColumnDef } from "@tanstack/react-table";
import { User } from "./test";
import { DataTableRowActions } from "@/components/ui/table/data-table-row-actions";

export const columns: ColumnDef<User>[] = [
    {
        id: "fullName",
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="id" />
        ),
        cell: ({ row }) => {
            return (
                <div className="flex items-center gap-x-2">
                    {row.getValue("name")}
                </div>
            )
        },
        meta: { className: "w-36" },
    },
    {
        id: "email",
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Email" />
        ),
        cell: ({ row }) => (
            <div className="w-fit text-nowrap">{row.original.email}</div>
        ),
        enableSorting: true,
    },
    {
        accessorKey: "name",
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Name" />
        ),
        cell: ({ row }) => <div>{row.original.name}</div>,
        enableSorting: true,
        sortDescFirst: true,
        sortingFn: "alphanumeric"
    },
    {
        id: "status",
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Status" />
        ),
        cell: ({ row }) => {
            return (
                <div className="flex items-center gap-x-2">
                    <span className="text-sm capitalize">{row.original.id}</span>
                </div>
            )
        }
    },
    {
        id: "actions",
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="" />
        ),
        cell: DataTableRowActions
    },
];