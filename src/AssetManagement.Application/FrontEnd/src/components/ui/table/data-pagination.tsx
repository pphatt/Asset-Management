import { Pagination } from "@/hooks/use-pagination"

interface DataPaginationProps {
    pagination: Pagination;
}

export const DataPagination = ({ pagination }: DataPaginationProps) => {
    return <div className="flex justify-end mt-4">
        <button className={`px-3 py-1 border border-gray-300 rounded hover:bg-gray-50 ${pagination.page == 1 ? "opacity-50" : ""}`}
            onClick={() => pagination.setPage(pagination.page - 1)}
            disabled={pagination.page == 1}
        >
            Previous
        </button>
        {
            // Array.from({ length: pagination.pageCount }, (_, i) => (
            Array.from({ length: 3 }, (_, i) => (
                <button
                    key={i}
                    className={`px-[14px] py-2 border border-gray-300 ${pagination.page === i + 1 ? "btn-primary" : "text-primary"}`}
                    onClick={() => pagination.setPage(i + 1)}
                >
                    {i + 1}
                </button>
            ))
        }
        <button className="px-3 py-1 border rounded border-gray-300 hover:bg-gray-50 text-primary"
            onClick={() => pagination.setPage(pagination.page + 1)}
            disabled={pagination.page === pagination.pageCount}
        >
            Next
        </button>
    </div>
}