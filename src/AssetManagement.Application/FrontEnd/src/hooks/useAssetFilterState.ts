import { useCallback, useState } from 'react';
import { STORAGE_KEYS } from '../constants/user-params';
import { useEffect } from 'react';
import { ASSET_SORT_OPTIONS, PAGINATION } from '@/constants/asset-params';
import { IAssetParams } from '@/types/asset.type';

function useAssetFilterState() {
    const getSavedFilterState = useCallback(() => {
        const savedState = localStorage.getItem(STORAGE_KEYS.ASSET_FILTER_STATE);
        if (savedState) {
            try {
                return JSON.parse(savedState);
            } catch {
                return null;
            }
        }
        return null;
    }, []);

    const [queryParams, setQueryParams] = useState<IAssetParams>({
        assetStates: ['Assigned', 'Available', 'NotAvailable'],
        assetCategories: [],
        pageNumber: getSavedFilterState()?.pageNumber || PAGINATION.DEFAULT_PAGE_NUMBER,
        pageSize: PAGINATION.DEFAULT_PAGE_SIZE,
        sortBy: getSavedFilterState()?.sortBy || `${ASSET_SORT_OPTIONS.ASSET_CODE}:asc`,
        searchTerm: '',
    });

    useEffect(() => {
        localStorage.setItem(
            STORAGE_KEYS.ASSET_FILTER_STATE,
            JSON.stringify({
                pageNumber: queryParams.pageNumber,
                sortBy: queryParams.sortBy,
            })
        );
    }, [queryParams.pageNumber, queryParams.sortBy]);

    return [queryParams, setQueryParams] as const;
}

export default useAssetFilterState;
