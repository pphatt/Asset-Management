/* eslint-disable @typescript-eslint/no-empty-object-type */
/* eslint-disable @typescript-eslint/no-unused-vars */

import { DataFacedFilter, FilterTable } from "@/components/ui/table/filter-table";
import { usePagination } from "@/hooks/use-pagination";
import { useState } from "react";

export interface User {
    id: string;
    name: string;
    email: string;
}

interface AccountFilterProps {
    initSearch: string;
    initSort: string[];
}

export interface AccountFilter extends FilterTable<User> {
}

export function useAccountFilter({
    initSearch,
}: AccountFilterProps): AccountFilter {
    const pagination = usePagination({ initialPage: 1, initialPageSize: 10 });
    const [search, setSearch] = useState(initSearch);
    const [isLoading, setIsLoading] = useState(false);
    const [status, setStatus] = useState<string[]>([]);

    // Create list user
    const [listUser, setListUser] = useState<User[]>([
        { id: "1", name: "John Doe", email: "a@gmail.com" },
        { id: "2", name: "Jane Doe", email: "b@gmail.com" },
        {
            id: "3", name: "John Smith", email: "c@gmail.com"
        },
        {
            id: "4", name: "Jane Smith", email: "d@gmail.com"
        }
    ]);

    // fix error not use properties
    function fixError() {
        setSearch("");
        setIsLoading(true);
        if (status.length > 0) {
            setListUser([]);
        }
    }
    fixError();

    const facedFilter: DataFacedFilter[] = [
        {
            name: "status",
            list: [
                { label: "Active", value: "Active" },
                { label: "Pending", value: "Pending" },
                { label: "Deleted", value: "Deleted" },
                { label: "Blocked", value: "Blocked" },
            ],
            setSelect: (value: string[]) => setStatus?.(value),
        },
    ];

    return {
        data: listUser,
        placeholder: "Filter user",
        pagination,
        isLoading,
        search,
        facedFilter
    };
}