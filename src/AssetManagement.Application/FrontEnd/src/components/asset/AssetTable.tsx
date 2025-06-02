import { IAsset } from "@/types/asset.type";
import TableSkeleton from "../common/TableSkeleton";

const AssetTable: React.FC<{
  assets: IAsset[] | undefined;
  isLoading: boolean;
  sortBy: string | undefined;
  onSort: (key: string) => void;
  onDelete: (assetId: string) => void;
  onViewDetails: (assetCode: string) => void;
}> = ({ assets, isLoading, sortBy, onSort, onDelete, onViewDetails }) => {
  const columns = [
    { key: "code", label: "Asset Code", sortable: true },
    { key: "name", label: "Asset Name", sortable: true },
    { key: "category", label: "Category", sortable: true },
    { key: "state", label: "State", sortable: true },
  ];

  if (isLoading) return <TableSkeleton rows={5} columns={6} />;

  return (
    <table className="w-full text-sm border-collapse border-spacing-0">
      <thead>
        <tr className="text-quaternary text-sm font-semibold">
          {columns.map((col) => (
            <th
              key={col.key}
              className={`text-left relative py-2 after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[2px] ${
                sortBy?.startsWith(`${col.key}:`)
                  ? "after:bg-gray-600 font-semibold"
                  : "after:bg-gray-400 font-medium"
              } ${col.sortable ? "cursor-pointer" : ""}`}
              onClick={col.sortable ? () => onSort(col.key) : undefined}
            >
              {col.label}
              {col.sortable && (
                <svg
                  className={`inline-block ml-1 w-3 h-3 ${
                    sortBy?.startsWith(`${col.key}:`) ? "text-primary" : ""
                  }`}
                  viewBox="0 0 24 24"
                  fill="none"
                >
                  {sortBy?.startsWith(`${col.key}:`) &&
                  sortBy?.endsWith(":desc") ? (
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

          <th className="text-center relative w-16">
            <span className="sr-only">Actions</span>
          </th>
        </tr>
      </thead>
      <tbody>
        {assets && assets.length > 0 ? (
          assets.map((asset) => (
            <tr
              key={asset.code}
              onClick={() => {
                onViewDetails(asset.id);
              }}
              className="cursor-pointer hover:bg-gray-50"
            >
              <td className="py-2 relative w-[100px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {asset.code}
              </td>
              <td className="py-2 relative w-[180px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {asset.name}
              </td>
              <td className="py-2 relative w-[120px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {asset.categoryName}
              </td>
              <td className="py-2 relative w-[80px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {asset.state}
              </td>
              <td className="py-2 relative">
                <div className="flex items-center justify-center space-x-4">
                  <button
                    className="text-quaternary hover:text-gray-700"
                    onClick={() => {}}
                    disabled={false}
                  >
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none">
                      <path
                        d="M11 4H4C3.46957 4 2.96086 4.21071 2.58579 4.58579C2.21071 4.96086 2 5.46957 2 6V20C2 20.5304 2.21071 21.0391 2.58579 21.4142C2.96086 21.7893 3.46957 22 4 22H18C18.5304 22 19.0391 21.7893 19.4142 21.4142C19.7893 21.0391 20 20.5304 20 20V13"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                      <path
                        d="M18.5 2.50001C18.8978 2.10219 19.4374 1.87869 20 1.87869C20.5626 1.87869 21.1022 2.10219 21.5 2.50001C21.8978 2.89784 22.1213 3.4374 22.1213 4.00001C22.1213 4.56262 21.8978 5.10219 21.5 5.50001L12 15L8 16L9 12L18.5 2.50001Z"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                    </svg>
                  </button>
                  <button
                    className="text-primary hover:text-red-700"
                    onClick={(e) => {
                      e.stopPropagation();
                      onDelete(asset.id);
                    }}
                    disabled={false}
                  >
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none">
                      <path
                        d="M18 6L6 18"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                      <path
                        d="M6 6L18 18"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          ))
        ) : (
          <tr>
            <td colSpan={6} className="py-4 text-center text-gray-500">
              No assets found. Try changing your search criteria.
            </td>
          </tr>
        )}
      </tbody>
    </table>
  );
};

export default AssetTable;
