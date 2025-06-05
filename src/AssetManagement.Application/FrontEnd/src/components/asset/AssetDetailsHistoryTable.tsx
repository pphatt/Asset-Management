import { IAssetDetailsHistory } from "@/types/asset.type";

const AssetDetailsHistoryTable: React.FC<{
  assetDetailsHistory: IAssetDetailsHistory[] | undefined;
}> = ({ assetDetailsHistory }) => {
  const columns = [
    { key: "date", label: "Date", sortable: true },
    { key: "assignedTo", label: "Assigned To", sortable: true },
    { key: "assignedBy", label: "Assigned By", sortable: true },
    { key: "returnedDate", label: "Returned Date", sortable: true },
  ];

  return (
    <table className="w-full text-sm border-collapse border-spacing-0">
      <thead>
        <tr className="text-quaternary text-sm font-semibold">
          {columns.map((col) => (
            <th
              key={col.key}
              className={`text-left relative py-2 after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[2px] after:bg-gray-400 font-bold`}
            >
              {col.label}
            </th>
          ))}
        </tr>
      </thead>

      <tbody>
        {assetDetailsHistory && assetDetailsHistory.length > 0 ? (
          assetDetailsHistory.map((history, index) => (
            <tr key={index} className="cursor-pointer hover:bg-gray-50">
              <td className="py-2 relative w-[100px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {history.date}
              </td>

              <td className="py-2 relative w-[90px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {history.assignedTo}
              </td>

              <td className="py-2 relative w-[90px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {history.assignedBy}
              </td>

              <td className="py-2 relative w-[80px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                {history.returnedDate ?? "-"}
              </td>
            </tr>
          ))
        ) : (
          <tr>
            <td colSpan={6} className="py-4 text-center text-gray-500">
              No available assignment history.
            </td>
          </tr>
        )}
      </tbody>
    </table>
  );
};

export default AssetDetailsHistoryTable;
