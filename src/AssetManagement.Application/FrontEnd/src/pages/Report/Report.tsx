import AssetReportTable from "@/components/report/AssetReportTable";
import useAssetReport, { useAssetReportParams } from "@/hooks/useAssetReport";
import { useCallback, useState } from "react";
import { toast } from "react-toastify";
import * as XLSX from 'xlsx';

export default function Report() {
  const [queryParams, setQueryParams] = useAssetReportParams();
  const {
    data,
    isLoading,
    isError,
    error,
    refetch,
    isFetching,
  } = useAssetReport(queryParams);
  const [isExporting, setIsExporting] = useState(false);

  const handleSort = useCallback(
    (key: string) => {
      setQueryParams((prev) => ({
        ...prev,
        sortBy: key,
        sortOrder: (prev.sortBy === key && prev.sortOrder === "asc") ? "desc" : "asc",
      }));
    },
    [setQueryParams],
  );

  const handleExport = useCallback(async () => {
    if (!data || data.length === 0) {
      toast.error("No data to export");
      return;
    }

    try {
      setIsExporting(true);

      // ensure the latest data is exported
      const result = await refetch();
      const latest = Array.isArray(result.data) && result.data.length > 0
        ? result.data
        : data;

      // Create a new workbook
      const wb = XLSX.utils.book_new();

      // Convert data to worksheet
      const ws = XLSX.utils.json_to_sheet(latest);

      // Add the worksheet to the workbook
      XLSX.utils.book_append_sheet(wb, ws, "Asset Report");

      // Generate filename with current date
      const currentDate = new Date().toISOString().split('T')[0];
      const filename = `asset-report-${currentDate}.xlsx`;

      // Write the file
      XLSX.writeFile(wb, filename);
    } catch (error) {
      console.error('Export failed: ', error);
      toast.error("Failed to export report. Please try again later.");
    } finally {
      setIsExporting(false);
    }
  }, [data]);

  return (
    <div className="w-full max-w-5xl mx-auto px-4 py-6">
      <h2 className="text-primary text-xl font-normal mb-5">Report</h2>
      <div className="flex justify-end mb-4">
        <button
          className="ml-4 bg-primary text-secondary px-4 py-1.5 text-sm rounded hover:scale-110 transition-all duration-150 hover:cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
          onClick={handleExport}
          disabled={
            isExporting ||
            isFetching ||
            isLoading ||
            !data ||
            data.length === 0
          }
        >
          {isExporting ? 'Exporting...' : 'Export'}
        </button>
      </div>
      <div className="overflow-x-auto">
        <AssetReportTable
          reportData={data}
          isLoading={isLoading}
          sortBy={queryParams.sortBy}
          sortOrder={queryParams.sortOrder}
          onSort={handleSort}
        />
      </div>

      {/* Error Messages */}
      {isError && (
        <div className="bg-red-100 text-red-700 p-3 mb-4 rounded border border-red-300">
          <p className="font-semibold">Error loading assets:</p>
          <p>
            {error instanceof Error
              ? error.message
              : "Unknown error occurred"}
          </p>
          <button
            className="text-sm text-red-600 underline mt-1"
            onClick={() => refetch()}
          >
            Try again
          </button>
        </div>
      )}
    </div>
  )
}