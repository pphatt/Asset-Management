import { IAssetState } from "@/types/asset.type";

const ActiveFilters: React.FC<{
  filterState: IAssetState | '';
  filterCategory: string;
  searchTerm: string;
  onClearType: () => void;
  onClearCategory: () => void;
  onClearSearch: () => void;
}> = ({ filterState, filterCategory, searchTerm, onClearType, onClearCategory, onClearSearch }) => {
  if (!filterState && !filterCategory && !searchTerm) return null;
  return (
    <div className="flex flex-wrap gap-2 mb-3">
      {filterState && (
        <div className="px-2 py-1 bg-gray-200 text-sm rounded-full flex items-center">
          Type: {filterState}
          <button className="ml-1 text-gray-600 hover:text-gray-800" onClick={onClearType}>
            ×
          </button>
        </div>
      )}
      {filterCategory && (
        <div className="px-2 py-1 bg-gray-200 text-sm rounded-full flex items-center">
          Category: {filterCategory}
          <button className="ml-1 text-gray-600 hover:text-gray-800" onClick={onClearCategory}>
            ×
          </button>
        </div>
      )}
      {searchTerm && (
        <div className="px-2 py-1 bg-gray-200 text-sm rounded-full flex items-center">
          Search: {searchTerm}
          <button
            className="ml-1 text-gray-600 hover:text-gray-800 p-0.5 rounded-full focus:outline-none"
            onClick={onClearSearch}
            aria-label="Clear search"
            type="button"
          >
            <svg width="14" height="14" viewBox="0 0 20 20" fill="none">
              <path
                d="M6 6L14 14M6 14L14 6"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
              />
            </svg>
          </button>
        </div>
      )}
    </div>
  );
};

export default ActiveFilters;
