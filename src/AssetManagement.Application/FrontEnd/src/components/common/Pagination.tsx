import React from "react";

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  onPageChange: (page: number) => void;
  isLoading?: boolean;
}

const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  hasNextPage,
  hasPreviousPage,
  onPageChange,
  isLoading,
}) => {
  if (totalPages < 1) return null;

  // Tạo mảng số trang sẽ hiển thị dựa trên trang hiện tại và tổng số trang
  const getPageNumbers = () => {
    const pageNumbers = [];

    // Luôn hiển thị trang đầu tiên
    pageNumbers.push(1);

    // Xác định phạm vi các trang ở giữa (gần trang hiện tại)
    let startPage = Math.max(2, currentPage - 1);
    let endPage = Math.min(totalPages - 1, currentPage + 1);

    // Nếu có khoảng cách giữa trang đầu và trang bắt đầu, hiển thị dấu "..."
    if (startPage > 2) {
      pageNumbers.push("...");
    }

    // Thêm các trang ở giữa (liền kề trang hiện tại)
    for (let i = startPage; i <= endPage; i++) {
      pageNumbers.push(i);
    }

    // Nếu có khoảng cách giữa trang kết thúc và trang cuối, hiển thị dấu "..."
    if (endPage < totalPages - 1) {
      pageNumbers.push("...");
    }

    // Luôn hiển thị trang cuối nếu tổng số trang > 1
    if (totalPages > 1) {
      pageNumbers.push(totalPages);
    }

    return pageNumbers;
  };

  return (
    <div className="flex justify-end mt-4">
      <div className="flex items-center space-x-1">
        {" "}
        <button
          type="button"
          aria-label="Previous page"
          className={`px-3 py-1 border border-tertiary rounded-sm ${
            !hasPreviousPage || currentPage <= 1 || isLoading
              ? "opacity-50 cursor-not-allowed"
              : "hover:bg-tertiary text-primary"
          } text-xs mr-1`}
          onClick={() => onPageChange(currentPage - 1)}
          disabled={!hasPreviousPage || currentPage <= 1 || isLoading}
        >
          Previous
        </button>
        {getPageNumbers().map((page, index) =>
          typeof page === "number" ? (
            <button
              type="button"
              key={`page-${page}`}
              className={`px-3 py-1 border border-tertiary rounded-sm text-xs mr-1 ${
                page === currentPage
                  ? "bg-primary text-secondary"
                  : "hover:bg-tertiary"
              }`}
              onClick={() => onPageChange(page)}
              disabled={isLoading}
            >
              {page}
            </button>
          ) : (
            // Hiển thị dấu "..." khi có nhiều trang bị ẩn giữa các trang hiển thị
            <span key={`ellipsis-${index}`} className="px-2 text-xs">
              {page}
            </span>
          ),
        )}{" "}
        <button
          type="button"
          aria-label="Next page"
          className={`px-3 py-1 border border-tertiary rounded-sm ${
            !hasNextPage || currentPage >= totalPages || isLoading
              ? "opacity-50 cursor-not-allowed"
              : "hover:bg-tertiary text-primary"
          } text-xs`}
          onClick={() => onPageChange(currentPage + 1)}
          disabled={!hasNextPage || currentPage >= totalPages || isLoading}
        >
          Next
        </button>
      </div>
    </div>
  );
};

export default Pagination;
