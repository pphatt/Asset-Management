import TableSkeleton from '@/components/common/TableSkeleton';
import useClickOutside from '@/hooks/useClickOutside';
import useDebounce from '@/hooks/useDebounce';
import useUser from '@/hooks/useUser';
import { IUser, IUserParams } from '@/types/user.type';
import { X } from 'lucide-react';
import React, { useEffect, useRef, useState } from 'react';
import Pagination from '../../components/common/Pagination';

interface UserSearchDropdownProps {
  value: string | null;
  onChange: (value: string | null) => void;
  mode: 'create' | 'edit';
  className?: string;
}

const UserSearchDropdown: React.FC<UserSearchDropdownProps> = ({ value, onChange, className }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedUserId, setSelectedUserId] = useState<string | null>(value);
  const [tempSelectedUserId, setTempSelectedUserId] = useState<string | null>(null);
  const modalRef = useRef<HTMLDivElement>(null!);
  const inputRef = useRef<HTMLInputElement>(null);
  const [sortBy, setSortBy] = useState<string>('staffCode:asc');

  const debouncedSearchTerm = useDebounce(searchTerm, 100);

  const [queryParams, setQueryParams] = useState<IUserParams>({
    searchTerm: '',
    pageNumber: 1,
    pageSize: 10,
    sortBy: 'staffCode:asc',
  });

  const { useUsersList } = useUser();
  const { data: userData, isLoading } = useUsersList(queryParams); // Close modal when clicking outside
  useClickOutside(modalRef, () => setIsModalOpen(false));
  useEffect(() => {
    if (value) {
      setSelectedUserId(value);
      setTempSelectedUserId(value);
    }
  }, [value]);

  useEffect(() => {
    setQueryParams((prev) => ({
      ...prev,
      searchTerm: debouncedSearchTerm,
      pageNumber: 1,
    }));
  }, [debouncedSearchTerm]);

  const handleSearch = () => {
    setQueryParams((prev) => ({
      ...prev,
      searchTerm,
      pageNumber: 1,
    }));
  };
  const handleInputClick = () => {
    setTempSelectedUserId(selectedUserId);
    setIsModalOpen(true);
  };

  const handleSort = (key: string) => {
    const currentSortBy = sortBy || 'staffCode:asc';
    const [currentField, currentDirection] = currentSortBy.split(':');

    const direction = currentField === key && currentDirection === 'asc' ? 'desc' : 'asc';
    const newSortBy = `${key}:${direction}`;

    setSortBy(newSortBy);
    setQueryParams((prev) => ({
      ...prev,
      sortBy: newSortBy,
    }));
  };
  const handleSelectUser = (userId: string) => {
    setTempSelectedUserId(userId);
  };
  const handleSaveSelection = () => {
    if (tempSelectedUserId) {
      setSelectedUserId(tempSelectedUserId);
      onChange(tempSelectedUserId);
      setIsModalOpen(false);
    }
  };

  const handleClearSelection = () => {
    setSelectedUserId(null);
    onChange(null);
  };

  const handlePageChange = (page: number) => {
    setQueryParams((prev) => ({
      ...prev,
      pageNumber: page,
    }));
  };

  const selectedUser = userData?.items.find((user) => user.id === selectedUserId) || null;

  const getFullName = (user: IUser) => `${user.firstName} ${user.lastName}`;
  const displayValue = selectedUser ? `${getFullName(selectedUser)} (${selectedUser.staffCode})` : '';

  const columns = [
    { key: 'staffCode', label: 'Staff Code', sortable: true },
    { key: 'fullName', label: 'Full Name', sortable: true },
    { key: 'type', label: 'Type', sortable: true },
  ];
  return (
    <div className="relative">
      <div className="flex items-center">
        <input
          type="text"
          ref={inputRef}
          value={displayValue}
          onClick={handleInputClick}
          readOnly
          className={`w-full py-2 px-3 border border-quaternary rounded ${className}`}
        />
        {selectedUser && (
          <button
            className="absolute right-10 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
            onClick={handleClearSelection}
            type="button"
          >
            <X size={18} />
          </button>
        )}
        <button type="button" onClick={handleInputClick} className="absolute right-2 top-1/2 transform -translate-y-1/2">
          <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-4.35-4.35M17 10a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </button>{' '}
      </div>
      {isModalOpen && (
        <>
          <div className="fixed inset-0 bg-transparent bg-opacity-10 z-40" onClick={() => setIsModalOpen(false)}></div>{' '}
          <div className="absolute top-full left-0 w-[140%] min-w-[500px] mt-1 z-50">
            <div ref={modalRef} className="bg-white rounded-lg shadow-lg w-full p-5 max-h-[80vh] overflow-y-auto border border-gray-200">
              <div className="flex justify-between mb-4">
                <div className="flex-1 mr-2">
                  <h3 className="text-lg font-semibold text-primary">Select User</h3>
                </div>
                <div className="flex-1">
                  <div className="flex">
                    <input
                      type="text"
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter') handleSearch();
                      }}
                      className="w-full border border-quaternary rounded-l p-1 text-sm"
                    />
                    <button
                      onClick={handleSearch}
                      className="flex items-center justify-center h-[30px] w-[30px] border border-l-0 border-quaternary rounded-r bg-white hover:bg-gray-50"
                    >
                      <svg
                        width="14"
                        height="14"
                        viewBox="0 0 24 24"
                        fill="none"
                        xmlns="http://www.w3.org/2000/svg"
                        stroke="currentColor"
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
                </div>
              </div>{' '}
              <div className="overflow-x-auto">
                {isLoading ? (
                  <TableSkeleton rows={5} columns={5} />
                ) : (
                  <table className="w-full text-sm border-collapse border-spacing-0 user-table-container">
                    {' '}
                    <thead>
                      <tr className="text-quaternary text-sm font-semibold">
                        <th></th>
                        {columns.map((col) => (
                          <th
                            key={col.key}
                            className={`text-left relative py-2 after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[2px] ${
                              sortBy?.startsWith(`${col.key}:`) ? 'after:bg-gray-600 font-semibold' : 'after:bg-gray-400 font-medium'
                            } ${col.sortable ? 'cursor-pointer' : ''}`}
                            onClick={col.sortable ? () => handleSort(col.key) : undefined}
                          >
                            {col.label}
                            {col.sortable && (
                              <svg
                                className="inline-block ml-1 w-4 h-4"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="2"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                              >
                                {sortBy === `${col.key}:desc` ? <path d="M18 15L12 9L6 15" /> : <path d="M6 9L12 15L18 9" />}
                              </svg>
                            )}
                          </th>
                        ))}
                      </tr>
                    </thead>{' '}
                    <tbody>
                      {userData?.items && userData.items.length > 0 ? (
                        userData.items.map((user) => (
                          <tr key={user.staffCode} onClick={() => handleSelectUser(user.id)} className="cursor-pointer hover:bg-gray-50">
                            <td className="py-2 w-[30px]">
                              {' '}
                              <input
                                type="radio"
                                checked={tempSelectedUserId === user.id}
                                onChange={() => handleSelectUser(user.id)}
                                className="w-4 h-4 text-primary focus:ring-primary border-gray-300 accent-primary"
                                onClick={(e) => e.stopPropagation()}
                              />
                            </td>
                            <td className="py-2 relative w-[100px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                              {user.staffCode}
                            </td>
                            <td className="py-2 relative w-[180px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                              {getFullName(user)}
                            </td>
                            <td className="py-2 relative w-[80px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                              {user.type}
                            </td>
                          </tr>
                        ))
                      ) : (
                        <tr>
                          <td colSpan={6} className="py-4 text-center text-gray-500">
                            No users found. Try changing your search criteria.
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                )}
              </div>
              {/* Pagination */}
              {!isLoading && userData && userData.paginationMetadata && (
                <div className="mt-3">
                  <Pagination
                    currentPage={userData.paginationMetadata.currentPage}
                    totalPages={userData.paginationMetadata.totalPages}
                    hasNextPage={userData.paginationMetadata.hasNextPage}
                    hasPreviousPage={userData.paginationMetadata.hasPreviousPage}
                    onPageChange={handlePageChange}
                    isLoading={isLoading}
                  />
                </div>
              )}
              <div className="mt-3 flex justify-end space-x-2">
                <div className="flex justify-start">
                  <button
                    className={`px-3 py-1 border rounded mr-2 text-sm ${
                      tempSelectedUserId ? 'bg-primary text-white hover:bg-red-900 transition' : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    }`}
                    onClick={handleSaveSelection}
                    disabled={!tempSelectedUserId}
                  >
                    Save
                  </button>
                  <button
                    className="px-3 py-1 border border-gray-300 rounded text-gray-700 hover:bg-gray-50 text-sm cursor-pointer"
                    onClick={() => setIsModalOpen(false)}
                  >
                    Cancel
                  </button>
                </div>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default UserSearchDropdown;
