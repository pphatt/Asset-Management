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
import { IAssetDetails } from "@/types/asset.type.ts";
import { IAssetParams } from "@/types/asset.type";
import path from "@/constants/path.ts";
import { useNavigate } from "react-router-dom";

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
  const [selectedAssetCode, setSelectedAssetCode] = useState<string>("");
  const [selectedAsset, setSelectedAsset] = useState<IAssetDetails | null>(
    null,
  );
  const [isDetailsPopupOpen, setIsDetailsPopupOpen] = useState(false);

  const [queryParams, setQueryParams] = useAssetFilterState();

  const { useAssetList, useAssetByAssetCode } = useAsset();
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
  } = useAssetByAssetCode(selectedAssetCode);

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
        console.log(updatedCategories);
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

  const handleViewAssetDetails = useCallback((assetCode: string) => {
    setSelectedAssetCode(assetCode);
  }, []);

  /**
   * Create new asset
   * @returns void
   * @description Create a new asset
   * @technique UseCallback
   */
  const handleCreateNewAsset = useCallback(() => {
    console.log("Create new asset");
    navigate(path.assetCreate);
  }, []);

  useEffect(() => {
    if (fetchedAssetDetails && selectedAssetCode) {
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
        history: fetchedAssetDetails.history,
      };

      setSelectedAsset(assetDetails);
      setIsDetailsPopupOpen(true);

      // Reset the selectedAssetCode to prevent re-fetching
      setSelectedAssetCode("");
    }
  }, [fetchedAssetDetails, selectedAssetCode]);

  // Handle API errors
  useEffect(() => {
    if (isAssetDetailsError && selectedAssetCode) {
      console.error("Error fetching user details:", assetDetailsError);
      setSelectedAssetCode(""); // Reset on error
    }
  }, [isAssetDetailsError, assetDetailsError, selectedAssetCode]);

  return (
    <div>
      {/* Filter and search */}
      <div className="flex justify-between mb-5">
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
        <div className="flex items-center">
          <div className="relative">
            <input
              type="text"
              placeholder="Search"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="w-[200px] h-[34px] text-sm py-1.5 px-2 border border-tertiary rounded"
            />
            <button
              className="absolute inset-y-0 right-0 flex items-center pr-2"
              onClick={handleSearch}
            >
              <svg
                width="16"
                height="16"
                viewBox="0 0 24 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M21 21L15 15M17 10C17 13.866 13.866 17 10 17C6.13401 17 3 13.866 3 10C3 6.13401 6.13401 3 10 3C13.866 3 17 6.13401 17 10Z"
                  stroke="currentColor"
                  strokeWidth="2"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
            </button>
          </div>
          <button
            className="ml-4 bg-primary text-secondary px-4 py-1.5 text-sm rounded"
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
          onViewDetails={handleViewAssetDetails}
        />
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
