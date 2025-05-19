type PaginationProps = {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
};

export default function Pagination({ currentPage, totalPages, onPageChange }: PaginationProps) {
  return (
    <div className="flex justify-end items-center space-x-2 mt-4">
      <button
        disabled={currentPage === 1}
        className="px-3 py-1 text-sm rounded bg-muted text-neutral-700 disabled:opacity-50"
        onClick={() => onPageChange(currentPage - 1)}
      >
        Previous
      </button>
      {[...Array(totalPages)].map((_, i) => {
        const page = i + 1;
        return (
          <button
            key={page}
            className={`px-3 py-1 text-sm rounded ${
              page === currentPage ? 'bg-primary text-white' : 'bg-muted text-neutral-700'
            }`}
            onClick={() => onPageChange(page)}
          >
            {page}
          </button>
        );
      })}
      <button
        disabled={currentPage === totalPages}
        className="px-3 py-1 text-sm rounded bg-muted text-neutral-700 disabled:opacity-50"
        onClick={() => onPageChange(currentPage + 1)}
      >
        Next
      </button>
    </div>
  );
}
