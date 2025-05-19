import React, { useCallback, useRef, useState } from 'react';
import { getUserApiField, USER_TYPES, UserField, UserType } from '../../constants/user-params';
import { useUser } from '../../hooks/useUser';
import useUserFilterState from '../../hooks/useUserFilterState';
import ActiveFilters from './ActiveFilters';
import Pagination from './Pagination';
import UserTable from './UserTable';
import UserTypeDropdown from './UserTypeDropdown';

const UserList: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterType, setFilterType] = useState<UserType>(USER_TYPES.ALL);
  const [showTypeDropdown, setShowTypeDropdown] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const [queryParams, setQueryParams] = useUserFilterState();

  const { useUsersList, useDeleteUser } = useUser();
  const {
    data: usersData,
    isLoading: isLoadingUsers,
    isError: isErrorUsers,
    error: errorUsers,
    refetch: refetchUsers,
  } = useUsersList(queryParams);
  const { mutate: deleteUserMutation, isPending: isDeleting } = useDeleteUser();

  /**
   * Handle search
   * @returns void
   * @description Search the users list by the search term
   * @technique UseCallback
   */
  const handleSearch = useCallback(() => {
    setQueryParams((prev) => ({
      ...prev,
      pageNumber: 1,
      searchTerm,
    }));
  }, [searchTerm, setQueryParams]);

  /**
   * Handle filter type
   * @param type - The type to filter by
   * @returns void
   * @description Filter the users list by the type
   * @technique UseCallback
   */
  const handleFilterType = useCallback(
    (type: UserType) => {
      setFilterType(type);
      setShowTypeDropdown(false);
      setQueryParams((prev) => ({
        ...prev,
        pageNumber: 1,
        userType: type,
      }));
    },
    [setQueryParams]
  );

  /**
   * Handle sort
   * @param key - The key to sort by
   * @returns void
   * @description Sort the users list by the key
   * @technique UseCallback
   */
  const handleSort = useCallback(
    (key: string) => {
      setQueryParams((prev) => {
        const currentSortParts = prev.sortBy?.split(':') || [];
        const currentKey = currentSortParts[0];
        const currentDirection = currentSortParts[1] || 'asc';
        const apiParamKey = getUserApiField(key as UserField);
        const newDirection = currentKey === key && currentDirection === 'asc' ? 'desc' : 'asc';
        return {
          ...prev,
          sortBy: `${key}:${newDirection}`,
          _apiSortBy: `${apiParamKey}:${newDirection}`,
        };
      });
    },
    [setQueryParams]
  );

  /**
   * Handle page change
   * @param page - The page number to change to
   * @returns void
   * @description Change the page number and refetch the users list
   * @technique UseCallback
   */
  const handlePageChange = useCallback(
    (page: number) => {
      setQueryParams((prev) => ({
        ...prev,
        pageNumber: page,
      }));
    },
    [setQueryParams]
  );

  /**
   * Create new user
   * @returns void
   * @description Create a new user
   * @technique UseCallback
   */
  const handleCreateNewUser = useCallback(() => {
    console.log('Create new user');
  }, []);

  /**
   * Edit user
   * @param staffCode - The staff code of the user to edit
   * @returns void
   * @description Edit a user
   * @technique UseCallback
   */
  const handleEditUser = useCallback((staffCode: string) => {
    console.log('Edit user', staffCode);
  }, []);

  /**
   * Delete user
   * @param staffCode - The staff code of the user to delete
   * @returns void
   * @description Delete a user and refetch the users list
   * @technique UseCallback
   */
  const handleDeleteUser = useCallback(
    (staffCode: string) => {
      if (window.confirm('Are you sure you want to delete this user?')) {
        deleteUserMutation(staffCode, {
          onSuccess: () => {
            refetchUsers();
          },
        });
      }
    },
    [deleteUserMutation, refetchUsers]
  );

  /**
   * Clear filter type
   * @returns void
   * @description Clear the filter type and reset the filter type to all
   * @technique UseCallback
   */
  const handleClearType = () => handleFilterType(USER_TYPES.ALL);

  /**
   * Clear search
   * @returns void
   * @description Clear the search and reset the search to empty
   * @technique UseCallback
   */
  const handleClearSearch = () => {
    setSearchTerm('');
    setQueryParams((prev) => ({ ...prev, searchTerm: '' }));
  };

  return (
    <div>
      {/* Filter and search */}
      <div className="flex justify-between mb-5">
        <UserTypeDropdown
          filterType={filterType}
          setFilterType={handleFilterType}
          show={showTypeDropdown}
          setShow={setShowTypeDropdown}
          dropdownRef={dropdownRef as React.RefObject<HTMLDivElement>}
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
            onClick={handleCreateNewUser}
          >
            Create new user
          </button>
        </div>
      </div>

      {/* Active filters display */}
      <ActiveFilters
        filterType={filterType}
        searchTerm={searchTerm}
        onClearType={handleClearType}
        onClearSearch={handleClearSearch}
      />

      {/* Error Messages */}
      {isErrorUsers && (
        <div className="bg-red-100 text-red-700 p-3 mb-4 rounded border border-red-300">
          <p className="font-semibold">Error loading users:</p>
          <p>{errorUsers instanceof Error ? errorUsers.message : 'Unknown error occurred'}</p>
          <button className="text-sm text-red-600 underline mt-1" onClick={() => refetchUsers()}>
            Try again
          </button>
        </div>
      )}

      {/* Table */}
      <div className="overflow-x-auto">
        <UserTable
          users={usersData?.items}
          isLoading={isLoadingUsers}
          sortBy={queryParams.sortBy}
          onSort={handleSort}
          onEdit={handleEditUser}
          onDelete={handleDeleteUser}
          isDeleting={isDeleting}
        />
      </div>

      {/* Pagination */}
      {!isLoadingUsers && usersData && usersData.paginationMetadata && (
        <Pagination
          currentPage={usersData.paginationMetadata.currentPage}
          totalPages={usersData.paginationMetadata.totalPages}
          hasNextPage={usersData.paginationMetadata.hasNextPage}
          hasPreviousPage={usersData.paginationMetadata.hasPreviousPage}
          onPageChange={handlePageChange}
          isLoading={isLoadingUsers}
        />
      )}
    </div>
  );
};

export default UserList;
