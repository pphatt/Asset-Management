import path from '@/constants/path';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { USER_TYPES, UserType } from '../../constants/user-params';
import { useUser } from '../../hooks/useUser';
import useUserFilterState from '../../hooks/useUserFilterState';
import { IUserDetails, IUser } from '../../types/user.type';
import ActiveFilters from './ActiveFilters';
import DisableUserPopup from './DisableUserPopup';
import Pagination from '../common/Pagination';
import UserDetailsPopup from './UserDetailsPopup';
import UserTable from './UserTable';
import UserTypeDropdown from './UserTypeDropdown';

const UserList: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterType, setFilterType] = useState<UserType>(USER_TYPES.ALL);
  const [showTypeDropdown, setShowTypeDropdown] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  // View user details
  const [selectedStaffCode, setSelectedStaffCode] = useState<string>('');
  const [selectedUser, setSelectedUser] = useState<IUserDetails | null>(null);
  const [isDetailsPopupOpen, setIsDetailsPopupOpen] = useState(false);

  // Disable user
  const [confirmDeleteModal, setConfirmDeleteModal] = useState(false);
  const [userToDelete, setUserToDelete] = useState<string | null>(null);
  const [targetUser, setTargetUser] = useState<IUser | null>(null);

  const navigate = useNavigate();
  const [queryParams, setQueryParams] = useUserFilterState();
  const { useUsersList, useDeleteUser, useUserByStaffCode } = useUser();
  const {
    data: usersData,
    isLoading: isLoadingUsers,
    isError: isErrorUsers,
    error: errorUsers,
    refetch: refetchUsers,
  } = useUsersList(queryParams);
  const { mutate: deleteUserMutation, isPending: isDeleting } = useDeleteUser();
  const {
    data: fetchedUserDetails,
    isError: isUserDetailsError,
    error: userDetailsError,
  } = useUserByStaffCode(selectedStaffCode);

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
        const newDirection = currentKey === key && currentDirection === 'asc' ? 'desc' : 'asc';
        return {
          ...prev,
          sortBy: `${key}:${newDirection}`,
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

  const handleViewUserDetails = useCallback((staffCode: string) => {
    setSelectedStaffCode(staffCode);
  }, []);

  /**
   * Create new user
   * @returns void
   * @description Create a new user
   * @technique UseCallback
   */
  const handleCreateNewUser = useCallback(() => {
    navigate(path.userCreate);
  }, []);

  /**
   * Edit user
   * @param staffCode - The staff code of the user to edit
   * @returns void
   * @description Edit a user
   * @technique UseCallback
   */
  const handleEditUser = useCallback((staffCode: string) => {
    navigate(path.userEdit.replace(':staffCode', staffCode));
  }, []);

  /**
   * Open delete confirmation modal
   * @param staffCode - The staff code of the user to delete
   * @returns void
   * @description Open the delete confirmation modal
   * @technique UseCallback
   */
  const handleDeleteUser = useCallback(
    (staffCode: string) => {
      setUserToDelete(staffCode);
      // Find the user in the current list
      if (usersData?.items) {
        const user = usersData.items.find((user) => user.staffCode === staffCode);
        if (user) {
          setTargetUser(user as IUser);
        }
      }
      setConfirmDeleteModal(true);
    },
    [usersData?.items]
  );

  /**
   * Delete user
   * @param staffCode - The staff code of the user to delete
   * @returns void
   * @description Delete a user and refetch the users list
   * @technique UseCallback
   */
  const confirmDeleteUser = useCallback(() => {
    if (userToDelete) {
      deleteUserMutation(userToDelete, {
        onSuccess: () => {
          refetchUsers();
          setConfirmDeleteModal(false);
          setUserToDelete(null);
          setTargetUser(null);
        },
      });
    }
  }, [userToDelete, deleteUserMutation, refetchUsers]);

  /**
   * Close delete confirmation modal
   * @returns void
   * @description Close the delete confirmation modal
   * @technique UseCallback
   */
  const closeDeleteModal = useCallback(() => {
    setConfirmDeleteModal(false);
    setUserToDelete(null);
    setTargetUser(null);
  }, []);

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

  useEffect(() => {
    if (fetchedUserDetails && selectedStaffCode) {
      const userDetails: IUserDetails = {
        staffCode: fetchedUserDetails.staffCode,
        firstName: fetchedUserDetails.firstName,
        lastName: fetchedUserDetails.lastName,
        username: fetchedUserDetails.username,
        dateOfBirth: fetchedUserDetails.dateOfBirth || '',
        gender: fetchedUserDetails.gender,
        joinedDate: fetchedUserDetails.joinedDate,
        type: fetchedUserDetails.type,
        location: fetchedUserDetails.location,
      };

      setSelectedUser(userDetails);
      setIsDetailsPopupOpen(true);

      // Reset the selectedStaffCode to prevent re-fetching
      setSelectedStaffCode('');
    }
  }, [fetchedUserDetails, selectedStaffCode]);

  // Handle API errors
  useEffect(() => {
    if (isUserDetailsError && selectedStaffCode) {
      console.error('Error fetching user details:', userDetailsError);
      setSelectedStaffCode(''); // Reset on error
    }
  }, [isUserDetailsError, userDetailsError, selectedStaffCode]);

  return (
    <div className="relative user-list-container">
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
          {' '}
          <div className="flex items-center justify-between">
            <input
              type="text"
              placeholder='Search...'
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
              className="w-[200px] h-[34px] text-sm py-1.5 px-2 border border-quaternary rounded-l bg-white"
            />
            <button
              type="button"
              onClick={handleSearch}
              className="flex items-center justify-center h-[34px] w-[34px] border border-l-0 border-quaternary rounded-r bg-white hover:bg-gray-50"
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
      <div className="overflow-x-auto relative">
        <UserTable
          users={usersData?.items}
          isLoading={isLoadingUsers}
          sortBy={queryParams.sortBy}
          onSort={handleSort}
          onEdit={handleEditUser}
          onDelete={handleDeleteUser}
          onViewDetails={handleViewUserDetails}
          isDeleting={isDeleting}
        />

        {/* Disable User Popup */}
        {confirmDeleteModal && targetUser && (
          <DisableUserPopup
            isOpen={true}
            onClose={closeDeleteModal}
            onConfirm={confirmDeleteUser}
            targetUser={targetUser}
          />
        )}
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
      <UserDetailsPopup
        isOpen={isDetailsPopupOpen}
        user={selectedUser}
        onClose={() => setIsDetailsPopupOpen(false)}
      />
    </div>
  );
};

export default UserList;
