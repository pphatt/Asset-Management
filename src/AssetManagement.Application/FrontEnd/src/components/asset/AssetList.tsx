import React, { useCallback, useRef, useState } from 'react';
import Pagination from './Pagination';
import {
    ASSET_CATEGORY,
    ASSET_STATE,
    AssetCategory,
    AssetField,
    AssetState,
    getAssetApiField,
} from '@/constants/asset-params';
import useAssetFilterState from '@/hooks/useAssetFilterState';
import AssetCategoryDropdown from './AssetCategoryDropdown';
import useAsset from '@/hooks/useAsset';
import AssetTable from './AssetTable';
import ActiveFilters from './ActiveFilters';
import AssetStateDropdown from './AssetStateDropdown';

const AssetList: React.FC = () => {
    const [searchTerm, setSearchTerm] = useState('');
    const [filterState, setFilterState] = useState<AssetState>(ASSET_STATE.ALL);
    const [filterCategory, setFilterCategory] = useState<AssetCategory>(ASSET_CATEGORY.ALL);
    const [showStateDropDown, setShowStateDropDown] = useState(false);
    const [showCategoryDropdown, setShowCategoryDropdown] = useState(false);
    const stateDropdownRef = useRef<HTMLDivElement>(null) as React.RefObject<HTMLDivElement>;
    const categoryDropdownRef = useRef<HTMLDivElement>(null) as React.RefObject<HTMLDivElement>;

    const [queryParams, setQueryParams] = useAssetFilterState();

    const { useAssetList } = useAsset();
    const {
        data: assetData,
        isLoading: isLoadingAssets,
        isError: isErrorAssets,
        error: errorAssets,
        refetch: refetchAssets,
    } = useAssetList(queryParams);

    const handleSearch = useCallback(() => {
        setQueryParams((prev) => ({
            ...prev,
            pageNumber: 1,
            searchTerm,
        }));
    }, [searchTerm, setQueryParams]);

    const handleFilterState = useCallback(
        (state: AssetState) => {
            setFilterState(state);
            setShowStateDropDown(false);
            setQueryParams((prev) => {
                const newParams = {
                    ...prev,
                    pageNumber: 1,
                };
                if (state !== ASSET_STATE.ALL) {
                    newParams.assetState = state;
                } else {
                    delete newParams.assetState;
                }
                return newParams;
            });
        },
        [setQueryParams]
    );

    const handleFilterCategory = useCallback(
        (category: AssetCategory) => {
            setFilterCategory(category);
            setShowCategoryDropdown(false);
            setQueryParams((prev) => {
                const newParams = {
                    ...prev,
                    pageNumber: 1,
                };
                if (category !== ASSET_CATEGORY.ALL) {
                    newParams.assetCategory = category;
                } else {
                    delete newParams.assetCategory;
                }
                return newParams;
            });
        },
        [setQueryParams]
    );

    const handleSort = useCallback(
        (key: string) => {
            setQueryParams((prev) => {
                const currentSortParts = prev.sortBy?.split(':') || [];
                const currentKey = currentSortParts[0];
                const currentDirection = currentSortParts[1] || 'asc';
                const apiParamKey = getAssetApiField(key as AssetField);
                const newDirection =
                    currentKey === key && currentDirection === 'asc' ? 'desc' : 'asc';

                return {
                    ...prev,
                    sortBy: `${key}:${newDirection}`,
                    _apiSortBy: `${apiParamKey}:${newDirection}`,
                };
            });
        },
        [setQueryParams]
    );

    const handlePageChange = useCallback(
        (page: number) => {
            setQueryParams((prev) => ({
                ...prev,
                pageNumber: page,
            }));
        },
        [setQueryParams]
    );
    const handleClearState = useCallback(() => {
        setFilterState(ASSET_STATE.ALL);
        setQueryParams((prev) => {
            const newParams = {
                ...prev,
                pageNumber: 1,
            };
            delete newParams.assetState;
            return newParams;
        });
    }, [setQueryParams]);

    const handleClearCategory = useCallback(() => {
        setFilterCategory(ASSET_CATEGORY.ALL);
        setQueryParams((prev) => ({
            ...prev,
            pageNumber: 1,
            assetCategory: ASSET_CATEGORY.ALL,
        }));
    }, [setQueryParams]);

    const handleClearSearch = useCallback(() => {
        setSearchTerm('');
        setQueryParams((prev) => ({
            ...prev,
            pageNumber: 1,
            searchTerm: '',
        }));
    }, [setQueryParams]);

    return (
        <div>
            {/* Filter and search */}
            <div className="flex justify-between mb-5">
                <AssetStateDropdown
                    filterState={filterState}
                    setFilterState={handleFilterState}
                    show={showStateDropDown}
                    setShow={setShowStateDropDown}
                    dropdownRef={stateDropdownRef as React.RefObject<HTMLDivElement>}
                />
                <AssetCategoryDropdown
                    filterCategory={filterCategory}
                    setFilterCategory={handleFilterCategory}
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
                            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
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
                        onClick={() => {}}
                    >
                        Create new asset
                    </button>
                </div>
            </div>

            <ActiveFilters
                filterState={filterState}
                filterCategory={filterCategory}
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
                            : 'Unknown error occurred'}
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
        </div>
    );
};

export default AssetList;
