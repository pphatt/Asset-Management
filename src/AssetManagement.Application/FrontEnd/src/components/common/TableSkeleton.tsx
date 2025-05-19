import React from 'react';

interface TableSkeletonProps {
  rows: number;
  columns: number;
}

const TableSkeleton: React.FC<TableSkeletonProps> = ({ rows, columns }) => {
  return (
    <div className="w-full animate-pulse">
      {/* Header skeleton */}
      <div className="flex mb-4">
        {Array.from({ length: columns }).map((_, colIndex) => (
          <div
            key={`header-${colIndex}`}
            className="h-8 bg-gray-200 rounded mr-2"
            style={{ width: `${100 / columns - 2}%` }}
          />
        ))}
      </div>

      {/* Rows skeleton */}
      {Array.from({ length: rows }).map((_, rowIndex) => (
        <div key={`row-${rowIndex}`} className="flex mb-4">
          {Array.from({ length: columns }).map((_, colIndex) => (
            <div
              key={`cell-${rowIndex}-${colIndex}`}
              className="h-6 bg-gray-100 rounded mr-2"
              style={{ width: `${100 / columns - 2}%` }}
            />
          ))}
        </div>
      ))}
    </div>
  );
};

export default TableSkeleton;
