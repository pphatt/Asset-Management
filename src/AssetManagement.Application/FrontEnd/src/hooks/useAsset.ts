import { useQuery } from "@tanstack/react-query";
import { useCallback } from "react";
import { IAssetParams } from "@/types/asset.type";
import { AssetField, getAssetApiField } from "@/constants/asset-params";
import assetApi from "@/apis/asset.api";
import { STORAGE_KEYS } from "@/constants/user-params";

export function useAsset() {
  const getCurrentAdminLocation = useCallback(() => {
    const user = JSON.parse(
      localStorage.getItem(STORAGE_KEYS.CURRENT_USER) || "{}",
    );
    return user?.location || "";
  }, []);

  function useAssetList(params: IAssetParams) {
    const adminLocation = getCurrentAdminLocation();

    return useQuery({
      queryKey: ["assets", params],
      queryFn: async () => {
        const apiParams = { ...params };
        if (!apiParams.location) {
          apiParams.location = adminLocation;
        }
        if (apiParams.sortBy) {
          const [field, direction] = apiParams.sortBy.split(":");
          const apiField = getAssetApiField(field as AssetField) || field;
          apiParams.sortBy = `${apiField}:${direction}`;
        }
        if (apiParams._apiSortBy) {
          apiParams.sortBy = apiParams._apiSortBy;
          delete apiParams._apiSortBy;
        }
        console.log("API Params:", apiParams);
        const response = await assetApi.getAssets(apiParams);
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || "Failed to fetch assets");
      },
    });
  }

  function useAssetStates() {
    return useQuery({
      queryKey: ["assetStates"],
      queryFn: async () => {
        const response = await assetApi.getAssetStates();
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || "Failed to fetch asset states");
      },
      staleTime: 24 * 60 * 60 * 1000,
    });
  }

  function useAssetCategories() {
    return useQuery({
      queryKey: ["assetCategories"],
      queryFn: async () => {
        const response = await assetApi.getAssetCategories();
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || "Failed to fetch asset categories");
      },
      staleTime: 24 * 60 * 60 * 1000,
    });
  }

  function useAssetByAssetCode(assetCode: string) {
    return useQuery({
      queryKey: ["asset", assetCode],
      queryFn: async () => {
        const response = await assetApi.getAssetByAssetCode(assetCode);
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || "Failed to fetch asset details");
      },
      staleTime: 24 * 60 * 60 * 1000,
    });
  }

  return {
    useAssetList,
    useAssetStates,
    useAssetCategories,
    useAssetByAssetCode,
    getCurrentAdminLocation,
  };
}

export default useAsset;
