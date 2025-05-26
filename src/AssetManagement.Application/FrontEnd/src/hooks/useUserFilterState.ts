import { useCallback, useState } from 'react';

import { USER_SORT_OPTIONS } from '../constants/user-params';

import { PAGINATION } from '../constants/user-params';

import { useEffect } from 'react';
import { STORAGE_KEYS } from '../constants/user-params';
import { IUserParams } from '../types/user.type';

function useUserFilterState() {
    const getSavedFilterState = useCallback(() => {
        const savedState = localStorage.getItem(STORAGE_KEYS.USER_FILTER_STATE);
        if (savedState) {
            try {
                return JSON.parse(savedState);
            } catch {
                return null;
            }
        }
        return null;
    }, []);

    const [queryParams, setQueryParams] = useState<IUserParams>({
        pageNumber: getSavedFilterState()?.pageNumber || PAGINATION.DEFAULT_PAGE_NUMBER,
        pageSize: PAGINATION.DEFAULT_PAGE_SIZE,
        sortBy: getSavedFilterState()?.sortBy || `${USER_SORT_OPTIONS.STAFF_CODE}:asc`,
        searchTerm: '',
    });

    useEffect(() => {
        localStorage.setItem(
            STORAGE_KEYS.USER_FILTER_STATE,
            JSON.stringify({
                pageNumber: queryParams.pageNumber,
                sortBy: queryParams.sortBy,
            })
        );
    }, [queryParams.pageNumber, queryParams.sortBy]);

    return [queryParams, setQueryParams] as const;
}

export default useUserFilterState;
