/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import React, { useState } from "react";
import { Table } from "@tanstack/react-table";

import useDialogState from "@/hooks/use-dialog-state";

type TableDialogType = "delete";

export interface TableContextType<T> {
  open: TableDialogType | null;
  setOpen: (str: TableDialogType | null) => void;
  currentRow: T | null;
  setCurrentRow: React.Dispatch<React.SetStateAction<T | null>>;
  table?: Table<T>;
  setTable?: React.Dispatch<React.SetStateAction<Table<T> | undefined>>;
}

const TableContext = React.createContext<TableContextType<any> | null>(null);

interface Props {
  children: React.ReactNode;
}

export default function TableProvider<T>({ children }: Props) {
  const [open, setOpen] = useDialogState<TableDialogType>(null);
  const [currentRow, setCurrentRow] = useState<T | null>(null);
  const [table, setTable] = useState<Table<T> | undefined>(undefined);

  return (
    <TableContext
      value={{ open, setOpen, currentRow, setCurrentRow, table, setTable }}
    >
      {children}
    </TableContext>
  );
}

export const useTable = () => {
  const tableContext = React.useContext(TableContext);

  if (!tableContext) {
    throw new Error("useTable has to be used within <UseTable>");
  }

  return tableContext;
};
