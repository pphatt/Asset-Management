import assetApi from "@/apis/asset.api";
import { IAssetReportParams } from "@/types/report.type";
import { useQuery } from "@tanstack/react-query";
import { isUndefined, omitBy } from "lodash";
import { useState } from "react";

export default function useAssetReport(params: IAssetReportParams) {
  return useQuery({
    queryKey: ["assetReport", params],
    queryFn: async () => {
      const apiParams = omitBy(
        { ...params },
        isUndefined
      );
      const response = await assetApi.getReport(apiParams);
      if (response.success && response.data) {
        return response.data;
      }
      throw new Error(response.message || "Failed to fetch asset report");
    },
  });
}

const DEFAULT_PARAMS: IAssetReportParams = {
  sortBy: 'category',
  sortOrder: 'asc',
};

export const useAssetReportParams = (
  initialParams: IAssetReportParams = DEFAULT_PARAMS
) => {
  const [queryParams, setQueryParams] = useState<IAssetReportParams>({
    ...DEFAULT_PARAMS,
    ...initialParams,
  });

  return [queryParams, setQueryParams] as const;
}
