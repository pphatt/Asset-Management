import TableSkeleton from '@/components/common/TableSkeleton';
import useAsset from '@/hooks/useAsset';
import useClickOutside from '@/hooks/useClickOutside';
import { IAssetParams } from '@/types/asset.type';
import { X } from 'lucide-react';
import React, { useEffect, useRef, useState } from 'react';
import Pagination from '../../components/common/Pagination';
import useDebounce from '../../hooks/useDebounce';

interface AssetSearchDropdownProps {
  value: string | null;
  onChange: (value: string | null) => void;
  mode: 'create' | 'edit';
  className?: string;
}

const AssetSearchDropdown: React.FC<AssetSearchDropdownProps> = ({ value, onChange, className }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedAssetId, setSelectedAssetId] = useState<string | null>(value);
  const [tempSelectedAssetId, setTempSelectedAssetId] = useState<string | null>(null);
  const modalRef = useRef<HTMLDivElement>(null!);
  const inputRef = useRef<HTMLInputElement>(null);
  const [sortBy, setSortBy] = useState<string>('code:asc');

  const debouncedSearchTerm = useDebounce(searchTerm, 100);

  // Query params for assets
  const [queryParams, setQueryParams] = useState<IAssetParams>({
    searchTerm: '',
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'code:asc',
    assetStates: ['Available'],
  });

  const { useAssetList } = useAsset();
  const { data: assetData, isLoading } = useAssetList(queryParams);

  // Close modal when clicking outside
  useClickOutside(modalRef, () => setIsModalOpen(false));

  useEffect(() => {
    if (value) {
      setSelectedAssetId(value);
      setTempSelectedAssetId(value);
    }
  }, [value]);

  const handleSearch = () => {
    setQueryParams((prev) => ({
      ...prev,
      searchTerm,
      pageNumber: 1,
    }));
  };

  const handleInputClick = () => {
    setTempSelectedAssetId(selectedAssetId);
    setIsModalOpen(true);
  };

  const handleSort = (key: string) => {
    const currentSortBy = sortBy || 'code:asc';
    const [currentField, currentDirection] = currentSortBy.split(':');

    const direction = currentField === key && currentDirection === 'asc' ? 'desc' : 'asc';
    const newSortBy = `${key}:${direction}`;

    setSortBy(newSortBy);
    setQueryParams((prev) => ({
      ...prev,
      sortBy: newSortBy,
    }));
  };

  const handleSelectAsset = (assetId: string) => {
    setTempSelectedAssetId(assetId);
  };
  const handleSaveSelection = () => {
    if (tempSelectedAssetId) {
      setSelectedAssetId(tempSelectedAssetId);
      onChange(tempSelectedAssetId);
      setIsModalOpen(false);
    }
  };

  const handleClearSelection = () => {
    setSelectedAssetId(null);
    onChange(null);
  };

  const handlePageChange = (page: number) => {
    setQueryParams((prev) => ({
      ...prev,
      pageNumber: page,
    }));
  };

  const selectedAsset = assetData?.items.find((asset) => asset.id === selectedAssetId) || null;
  const displayValue = selectedAsset ? `${selectedAsset.name} (${selectedAsset.code})` : '';

  const columns = [
    { key: 'code', label: 'Asset Code', sortable: true },
    { key: 'name', label: 'Asset Name', sortable: true },
    { key: 'category', label: 'Category', sortable: true },
  ];

  useEffect(() => {
    if (debouncedSearchTerm) {
      setQueryParams((prev) => ({
        ...prev,
        searchTerm: debouncedSearchTerm,
        pageNumber: 1,
      }));
    } else {
      setQueryParams((prev) => ({
        ...prev,
        searchTerm: '',
        pageNumber: 1,
      }));
    }
  }, [debouncedSearchTerm]);

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
        {selectedAssetId && (
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
        </button>
      </div>{' '}
      {isModalOpen && (
        <>
          <div className="fixed inset-0 bg-transparent bg-opacity-10 z-40" onClick={() => setIsModalOpen(false)}></div>
          <div className="absolute top-full left-0 w-[140%] min-w-[500px] mt-1 z-50">
            <div ref={modalRef} className="bg-white rounded-lg shadow-lg w-full p-5 max-h-[80vh] overflow-y-auto border border-gray-200">
              <div className="flex justify-between mb-4">
                <div className="flex-1 mr-2">
                  <h3 className="text-lg font-semibold text-primary">Select Asset</h3>
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
              </div>

              <div className="overflow-x-auto">
                {isLoading ? (
                  <TableSkeleton rows={5} columns={5} />
                ) : (
                  <table className="w-full text-sm border-collapse border-spacing-0">
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
                      {assetData?.items && assetData.items.length > 0 ? (
                        assetData.items.map((asset) => (
                          <tr key={asset.id} onClick={() => handleSelectAsset(asset.id)} className="cursor-pointer hover:bg-gray-50">
                            <td className="py-2 w-[30px]">
                              <input
                                type="radio"
                                checked={tempSelectedAssetId === asset.id}
                                onChange={() => handleSelectAsset(asset.id)}
                                className="w-4 h-4 text-primary focus:ring-primary border-gray-300 accent-primary"
                                onClick={(e) => e.stopPropagation()}
                              />
                            </td>
                            <td className="py-2 relative w-[100px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                              {asset.code}
                            </td>
                            <td className="py-2 relative w-[180px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                              {asset.name}
                            </td>
                            <td className="py-2 relative w-[120px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                              {asset.categoryName}
                            </td>
                          </tr>
                        ))
                      ) : (
                        <tr>
                          <td colSpan={6} className="py-4 text-center text-gray-500">
                            No assets found. Try changing your search criteria.
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                )}
              </div>

              {/* Pagination */}
              {!isLoading && assetData && assetData.paginationMetadata && (
                <div className="mt-3">
                  <Pagination
                    currentPage={assetData.paginationMetadata.currentPage}
                    totalPages={assetData.paginationMetadata.totalPages}
                    hasNextPage={assetData.paginationMetadata.hasNextPage}
                    hasPreviousPage={assetData.paginationMetadata.hasPreviousPage}
                    onPageChange={handlePageChange}
                    isLoading={isLoading}
                  />
                </div>
              )}

              <div className="mt-3 flex justify-end space-x-2">
                <div className="flex justify-start">
                  <button
                    className={`px-3 py-1 border rounded mr-2 text-sm ${
                      tempSelectedAssetId ? 'bg-primary text-white hover:bg-red-900 transition' : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    }`}
                    onClick={handleSaveSelection}
                    disabled={!tempSelectedAssetId}
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

export default AssetSearchDropdown;
