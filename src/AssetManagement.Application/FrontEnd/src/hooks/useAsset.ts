import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useCallback } from "react";
import {
  IAssetParams,
  ICreateAssetRequest,
  IUpdateAssetRequest,
} from "@/types/asset.type";
import { AssetField, getAssetApiField } from "@/constants/asset-params";
import assetApi from "@/apis/asset.api";
import { STORAGE_KEYS } from "@/constants/user-params";
import { useNavigate } from "react-router-dom";
import useAssetFilterState from "@/hooks/useAssetFilterState.ts";
import { toast } from "react-toastify";
import path from "@/constants/path.ts";

export function useAsset() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [_, setQueryParams] = useAssetFilterState();

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

  function useAssetByAssetId(assetId: string) {
    return useQuery({
      queryKey: ["asset", assetId],
      queryFn: async () => {
        const response = await assetApi.getAssetByAssetId(assetId);
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || "Failed to fetch asset details");
      },
      staleTime: 24 * 60 * 60 * 1000,
    });
  }

  /**
   * Create a new asset
   * @returns {UseMutationResult<IAsset>} The new asset
   * @description Create a new asset from the API
   * @technique {useMutation} -> Mutate the new asset from the API
   */
  function useCreateAsset() {
    return useMutation({
      mutationFn: async (assetData: ICreateAssetRequest) =>
        assetApi.createAsset(assetData),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ["assets"], exact: false });
        setQueryParams((prev) => ({
          ...prev,
          // currently keep this as a fixed string (TODO: refactor this)
          sortBy: "created:desc",
          pageNumber: 1,
        }));
        toast.success("Asset created successfully");
        navigate(path.asset);
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] || "Error creating asset");
      },
    });
  }

  /**
   * Delete a asset
   * @returns {UseMutationResult<void>} The deleted asset
   * @description Delete a asset from the API
   * @technique {useMutation} -> Mutate the deleted asset from the API
   */
  function useDeleteAsset() {
    return useMutation({
      mutationFn: async (assetId: string) => {
        const response = await assetApi.deleteAsset(assetId);

        if (!response.success) {
          throw new Error(response.message || "Failed to delete asset");
        }
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] || "Error deleting asset");
      },
    });
  }

  function useUpdateAsset() {
    return useMutation({
      mutationFn: async ({
        assetCode: assetId,
        assetData,
      }: {
        assetCode: string;
        assetData: IUpdateAssetRequest;
      }) => assetApi.updateAsset(assetId, assetData),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ["asset"] });
        queryClient.invalidateQueries({ queryKey: ["assets"], exact: false });
        setQueryParams((prev) => ({
          ...prev,
          // currently keep this as a fixed string (TODO: refactor this)
          sortBy: "created:desc",
          pageNumber: 1,
        }));
        toast.success("Asset updated successfully");
        navigate(path.asset);
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] || "Error updating asset");
      },
    });
  }

  return {
    useAssetList,
    useAssetStates,
    useAssetByAssetCode: useAssetByAssetId,
    getCurrentAdminLocation,
    useCreateAsset,
    useDeleteAsset,
    useUpdateAsset,
  };
}

export default useAsset;
