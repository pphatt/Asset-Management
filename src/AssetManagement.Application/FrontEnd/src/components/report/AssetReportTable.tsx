import TableSkeleton from "../common/TableSkeleton";
import { IAssetReport } from "@/types/report.type";

const AssetReportTable: React.FC<{
  reportData: IAssetReport[] | undefined;
  isLoading: boolean;
  sortBy: string | undefined;
  sortOrder: string | undefined;
  onSort: (key: string) => void;
}> = ({ reportData, isLoading, sortBy, sortOrder, onSort }) => {
  const columns = [
    { key: "category", label: "Category", sortable: true },
    { key: "total", label: "Total", sortable: true },
    { key: "assigned", label: "Assigned", sortable: true },
    { key: "available", label: "Available", sortable: true },
    { key: "notavailable", label: "Not available", sortable: true },
    { key: "waitingforrecycling", label: "Waiting for recycling", sortable: true },
    { key: "recycled", label: "Recycled", sortable: true },
  ];

  if (isLoading) return <TableSkeleton rows={reportData?.length ?? 10} columns={7} />;

  return (
    <table className="w-full text-sm border-collapse border-spacing-0">
      <thead>
        <tr className="text-quaternary text-sm font-semibold">
          {columns.map((col) => (
            <th
              key={col.key}
              className={`text-left relative py-2 after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[2px] ${sortBy === col.key
                  ? "after:bg-gray-600 font-semibold"
                  : "after:bg-gray-400 font-medium"
                } ${col.sortable ? "cursor-pointer" : ""}`}
              onClick={col.sortable ? () => onSort(col.key) : undefined}
            >
              {col.label}
              {col.sortable && (
                <svg
                  className={`inline-block ml-1 w-3 h-3 ${sortBy === col.key ? "text-primary" : ""
                    }`}
                  viewBox="0 0 24 24"
                  fill="none"
                >
                  {sortBy === col.key &&
                    sortOrder === "desc" ? (
                    <path
                      d="M18 15L12 9L6 15"
                      stroke="currentColor"
                      strokeWidth="2"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  ) : (
                    <path
                      d="M6 9L12 15L18 9"
                      stroke="currentColor"
                      strokeWidth="2"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  )}
                </svg>
              )}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {reportData && reportData.length > 0 ? (
          reportData.map((entry) => (
            <tr
              key={entry.category}
              className="cursor-pointer hover:bg-gray-50"
            >
              <td className="py-2 relative w-[180px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {entry.category}
              </td>
              <td className="py-2 relative w-[90px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {entry.total}
              </td>
              <td className="py-2 relative w-[90px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {entry.assigned}
              </td>
              <td className="py-2 relative w-[90px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {entry.available}
              </td>
              <td className="py-2 relative w-[110px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {entry.notAvailable}
              </td>
              <td className="py-2 relative w-[160px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {entry.waitingForRecycling}
              </td>
              <td className="py-2 relative w-[90px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {entry.recycled}
              </td>
            </tr>
          ))
        ) : (
          <tr>
            <td colSpan={6} className="py-4 text-center text-gray-500">
              Report not found. Please try again later!
            </td>
          </tr>
        )}
      </tbody>
    </table>
  );
};

export default AssetReportTable;
