import React, { useCallback, useEffect, useRef, useState } from "react";
import Pagination from "../common/Pagination";
import {
  ASSET_CATEGORY,
  ASSET_STATE,
  AssetCategory,
  AssetField,
  AssetState,
  getAssetApiField,
} from "@/constants/asset-params";
import useAssetFilterState from "@/hooks/useAssetFilterState";
import AssetCategoryDropdown from "./AssetCategoryDropdown";
import useAsset from "@/hooks/useAsset";
import AssetTable from "./AssetTable";
import ActiveFilters from "./ActiveFilters";
import AssetStateDropdown from "./AssetStateDropdown";
import AssetDetailsPopup from "@/components/asset/AssetDetailsPopup.tsx";
import { IAsset, IAssetDetails } from "@/types/asset.type.ts";
import { IAssetParams } from "@/types/asset.type";
import path from "@/constants/path.ts";
import { useNavigate } from "react-router-dom";
import DisableAssetPopup from "@/components/asset/DeleteAssetPopup.tsx";

const AssetList: React.FC = () => {
  const navigate = useNavigate();

  const [searchTerm, setSearchTerm] = useState("");
  const [filterStates, setFilterStates] = useState<AssetState[]>([
    "Assigned",
    "Available",
    "NotAvailable",
  ]);
  const [filterCategories, setFilterCategories] = useState<AssetCategory[]>([
    ASSET_CATEGORY.ALL,
  ]);
  const [showStateDropDown, setShowStateDropDown] = useState(false);
  const [showCategoryDropdown, setShowCategoryDropdown] = useState(false);
  const stateDropdownRef = useRef<HTMLDivElement>(
    null,
  ) as React.RefObject<HTMLDivElement>;
  const categoryDropdownRef = useRef<HTMLDivElement>(
    null,
  ) as React.RefObject<HTMLDivElement>;

  // View asset details
  const [selectedAssetId, setSelectedAssetId] = useState<string>("");
  const [selectedAsset, setSelectedAsset] = useState<IAssetDetails | null>(
    null,
  );
  const [isDetailsPopupOpen, setIsDetailsPopupOpen] = useState(false);

  // Disable asset
  const [confirmDeleteModal, setConfirmDeleteModal] = useState(false);
  const [assetToDelete, setAssetToDelete] = useState<IAsset | null>(null);

  const [queryParams, setQueryParams] = useAssetFilterState();

  const { useAssetList, useDeleteAsset, useAssetByAssetCode } = useAsset();
  const {
    data: assetData,
    isLoading: isLoadingAssets,
    isError: isErrorAssets,
    error: errorAssets,
    refetch: refetchAssets,
  } = useAssetList(queryParams);

  const {
    data: fetchedAssetDetails,
    isError: isAssetDetailsError,
    error: assetDetailsError,
  } = useAssetByAssetCode(selectedAssetId);

  const { mutate: deleteAssetMutation, isPending: isDeleting } =
    useDeleteAsset();

  const handleSearch = useCallback(() => {
    setQueryParams((prev) => ({
      ...prev,
      pageNumber: 1,
      searchTerm,
    }));
  }, [searchTerm, setQueryParams]);

  const handleFilterStates = useCallback(
    (states: AssetState[]) => {
      setFilterStates(states);
      setShowStateDropDown(false);
      setQueryParams((prev) => {
        const newParams = {
          ...prev,
          pageNumber: 1,
        };
        if (states.length > 0) {
          newParams.assetStates = states.includes(ASSET_STATE.ALL)
            ? []
            : [...states];
        }
        return newParams;
      });
    },
    [setQueryParams, filterStates],
  );

  const handleFilterCategories = useCallback(
    (categories: AssetCategory[]) => {
      setFilterCategories(categories);
      setShowCategoryDropdown(false);
      setQueryParams((prev) => {
        const newParams = {
          ...prev,
          pageNumber: 1,
        };
        if (categories.length > 0) {
          newParams.assetCategories = categories.includes(ASSET_CATEGORY.ALL)
            ? []
            : [...categories];
        }
        return newParams;
      });
    },
    [setQueryParams, filterCategories],
  );

  const handleSort = useCallback(
    (key: string) => {
      setQueryParams((prev) => {
        const currentSortParts = prev.sortBy?.split(":") || [];
        const currentKey = currentSortParts[0];
        const currentDirection = currentSortParts[1] || "asc";
        const apiParamKey = getAssetApiField(key as AssetField);
        const newDirection =
          currentKey === key && currentDirection === "asc" ? "desc" : "asc";

        return {
          ...prev,
          sortBy: `${key}:${newDirection}`,
          _apiSortBy: `${apiParamKey}:${newDirection}`,
        };
      });
    },
    [setQueryParams],
  );

  const handlePageChange = useCallback(
    (page: number) => {
      setQueryParams((prev) => ({
        ...prev,
        pageNumber: page,
      }));
    },
    [setQueryParams],
  );

  const handleClearState = useCallback(
    (name: AssetState) => {
      const updatedStates = [...filterStates.filter((state) => state !== name)];
      setQueryParams((prev) => {
        const newParams: IAssetParams = {
          ...prev,
          assetStates: updatedStates.includes(ASSET_STATE.ALL)
            ? []
            : updatedStates,
          pageNumber: 1,
        };
        return newParams;
      });
      setFilterStates([...updatedStates]);
    },
    [setQueryParams, filterStates, setFilterStates],
  );

  const handleClearCategory = useCallback(
    (name: AssetCategory) => {
      const updatedCategories = [
        ...filterCategories.filter((category) => category !== name),
      ];
      setQueryParams((prev) => {
        const newParams: IAssetParams = {
          ...prev,
          assetCategories: updatedCategories.includes(ASSET_CATEGORY.ALL)
            ? []
            : updatedCategories,
          pageNumber: 1,
        };
        return newParams;
      });
      setFilterCategories([...updatedCategories]);
    },
    [setQueryParams, filterCategories, setFilterCategories],
  );

  const handleClearSearch = useCallback(() => {
    setSearchTerm("");
    setQueryParams((prev) => ({
      ...prev,
      pageNumber: 1,
      searchTerm: "",
    }));
  }, [setQueryParams]);

  const handleViewAssetDetails = useCallback((assetId: string) => {
    setSelectedAssetId(assetId);
  }, []);

  /**
   * Open delete confirmation modal
   * @param assetId - The id of the asset to delete
   * @returns void
   * @description Open the delete confirmation modal
   * @technique UseCallback
   */
  const handleDeleteAsset = useCallback(
    (asset: IAsset) => {
      console.log(asset);
      setAssetToDelete(asset);
      setConfirmDeleteModal(true);
    },
    [assetData?.items],
  );

  /**
   * Delete asset
   * @returns void
   * @description Delete a asset and refetch the assets list
   * @technique UseCallback
   */
  const confirmDeleteAsset = useCallback(() => {
    if (assetToDelete) {
      deleteAssetMutation(assetToDelete.id, {
        onSuccess: () => {
          refetchAssets();
          setConfirmDeleteModal(false);
          setAssetToDelete(null);
        },
      });
    }
  }, [assetToDelete, deleteAssetMutation, refetchAssets]);

  /**
   * Close delete confirmation modal
   * @returns void
   * @description Close the delete confirmation modal
   * @technique UseCallback
   */
  const closeDeleteModal = useCallback(() => {
    setConfirmDeleteModal(false);
    setAssetToDelete(null);
  }, []);

  /**
   * Create new asset
   * @returns void
   * @description Create a new asset
   * @technique UseCallback
   */
  const handleCreateNewAsset = useCallback(() => {
    navigate(path.assetCreate);
  }, []);

  useEffect(() => {
    if (fetchedAssetDetails && selectedAssetId) {
      const assetDetails: IAssetDetails = {
        id: fetchedAssetDetails.id,
        name: fetchedAssetDetails.name,
        code: fetchedAssetDetails.code,
        installedDate: fetchedAssetDetails.installedDate,
        type: fetchedAssetDetails.type,
        categoryName: fetchedAssetDetails.categoryName,
        specification: fetchedAssetDetails.specification,
        location: fetchedAssetDetails.location,
        state: fetchedAssetDetails.state,
        assignments: fetchedAssetDetails.assignments,
        hasAssignments: fetchedAssetDetails.hasAssignments,
      };

      setSelectedAsset(assetDetails);
      setIsDetailsPopupOpen(true);

      // Reset the selectedAssetCode to prevent re-fetching
      setSelectedAssetId("");
    }
  }, [fetchedAssetDetails, selectedAssetId]);

  // Handle API errors
  useEffect(() => {
    if (isAssetDetailsError && selectedAssetId) {
      console.error("Error fetching user details:", assetDetailsError);
      setSelectedAssetId(""); // Reset on error
    }
  }, [isAssetDetailsError, assetDetailsError, selectedAssetId]);

  return (
    <div>
      {/* Filter and search */}
      <div className="flex justify-between mb-5">
        <div className="flex gap-1.5">
          <AssetStateDropdown
            filterStates={filterStates}
            handleFilterByStates={handleFilterStates}
            handleClearState={handleClearState}
            show={showStateDropDown}
            setShow={setShowStateDropDown}
            dropdownRef={stateDropdownRef as React.RefObject<HTMLDivElement>}
          />
          <AssetCategoryDropdown
            filterCategories={filterCategories}
            handleFilterByCategories={handleFilterCategories}
            handleClearCategory={handleClearCategory}
            show={showCategoryDropdown}
            setShow={setShowCategoryDropdown}
            dropdownRef={categoryDropdownRef as React.RefObject<HTMLDivElement>}
          />
        </div>
        <div className="flex items-center">
          <div className="flex items-center justify-between">
            <input
              type="text"
              placeholder="Search..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="w-[200px] h-[34px] text-sm py-1.5 px-2 border border-quaternary rounded-l bg-white"
            />
            <button
              type="button"
              onClick={handleSearch}
              className="flex items-center justify-center h-[34px] w-[34px] border border-l-0 border-quaternary rounded-r cursor-pointer bg-white hover:bg-gray-100"
              aria-label="Search Button"
            >
              <svg
                width="16"
                height="16"
                viewBox="0 0 24 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
                stroke="black"
                strokeWidth="2"
              >
                <path
                  d="M21 21L15 15M17 10C17 13.866 13.866 17 10 17C6.13401 17 3 13.866 3 10C3 6.13401 6.13401 3 10 3C13.866 3 17 6.13401 17 10Z"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
            </button>
          </div>
          <button
            className="ml-4 bg-primary text-secondary px-4 py-1.5 text-sm rounded cursor-pointer hover:scale-110 transition-all duration-150"
            onClick={handleCreateNewAsset}
          >
            Create new asset
          </button>
        </div>
      </div>

      <ActiveFilters
        filterStates={filterStates}
        filterCategories={filterCategories}
        searchTerm={searchTerm}
        onClearType={handleClearState}
        onClearCategory={handleClearCategory}
        onClearSearch={handleClearSearch}
      />

      {/* Error Messages */}
      {isErrorAssets && (
        <div className="bg-red-100 text-red-700 p-3 mb-4 rounded border border-red-300">
          <p className="font-semibold">Error loading assets:</p>
          <p>
            {errorAssets instanceof Error
              ? errorAssets.message
              : "Unknown error occurred"}
          </p>
          <button
            className="text-sm text-red-600 underline mt-1"
            onClick={() => refetchAssets()}
          >
            Try again
          </button>
        </div>
      )}

      {/* Table */}
      <div className="overflow-x-auto">
        <AssetTable
          assets={assetData?.items}
          isLoading={isLoadingAssets}
          sortBy={queryParams.sortBy}
          onSort={handleSort}
          onDelete={handleDeleteAsset}
          onViewDetails={handleViewAssetDetails}
        />

        {/* Disable Asset Popup */}
        {assetToDelete && (
          <DisableAssetPopup
            asset={{
              id: assetToDelete.id,
              code: assetToDelete.code,
              name: assetToDelete.name,
              categoryName: assetToDelete.categoryName,
              state: assetToDelete.state,
              hasAssignments: assetToDelete.hasAssignments,
            }}
            isOpen={confirmDeleteModal}
            isDisabled={isDeleting}
            onClose={closeDeleteModal}
            onConfirm={confirmDeleteAsset}
          />
        )}
      </div>

      {/* Pagination */}
      {!isLoadingAssets && assetData && assetData.paginationMetadata && (
        <Pagination
          currentPage={assetData.paginationMetadata.currentPage}
          totalPages={assetData.paginationMetadata.totalPages}
          hasNextPage={assetData.paginationMetadata.hasNextPage}
          hasPreviousPage={assetData.paginationMetadata.hasPreviousPage}
          onPageChange={handlePageChange}
          isLoading={isLoadingAssets}
        />
      )}

      <AssetDetailsPopup
        isOpen={isDetailsPopupOpen}
        asset={selectedAsset}
        onClose={() => setIsDetailsPopupOpen(false)}
      />
    </div>
  );
};

export default AssetList;
