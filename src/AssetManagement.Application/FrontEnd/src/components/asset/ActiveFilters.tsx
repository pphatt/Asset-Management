import { ASSET_CATEGORY, ASSET_STATE, AssetCategory, AssetState } from '@/constants/asset-params';

const ActiveFilters: React.FC<{
    filterStates: AssetState[];
    filterCategories: AssetCategory[];
    searchTerm: string;
    onClearType: (name: AssetState) => void;
    onClearCategory: (name: AssetCategory) => void;
    onClearSearch: () => void;
}> = ({
    filterStates,
    filterCategories,
    searchTerm,
    onClearType,
    onClearCategory,
    onClearSearch,
}) => {
    if (!filterStates && !filterCategories && !searchTerm) return null;
    return (
        <div className="flex flex-wrap gap-2 mb-3">
            {filterStates &&
                filterStates.length > 0 &&
                filterStates.map((item) => (
                    <div className="px-2 py-1 bg-gray-200 text-sm rounded-full flex items-center">
                        Type: {item === ASSET_STATE.ALL ? 'All' : item}
                        <button
                            className="ml-1 text-gray-600 hover:text-gray-800"
                            onClick={() => onClearType(item)}
                        >
                            ×
                        </button>
                    </div>
                ))}
            {filterCategories &&
                filterCategories.length > 0 &&
                filterCategories.map((item) => (
                    <div className="px-2 py-1 bg-gray-200 text-sm rounded-full flex items-center">
                        Category: {item === ASSET_CATEGORY.ALL ? 'All' : item}
                        <button
                            className="ml-1 text-gray-600 hover:text-gray-800"
                            onClick={() => onClearCategory(item)}
                        >
                            ×
                        </button>
                    </div>
                ))}
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
