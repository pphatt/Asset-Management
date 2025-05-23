import { ASSET_STATE_OPTIONS, AssetState } from '../../constants/asset-params';
import useClickOutside from '../../hooks/useClickOutside';

const AssetStateDropdown: React.FC<{
    filterState: AssetState;
    setFilterState: (state: AssetState) => void;
    show: boolean;
    setShow: (show: boolean) => void;
    dropdownRef: React.RefObject<HTMLDivElement>;
}> = ({ filterState, setFilterState, show, setShow, dropdownRef }) => {
    useClickOutside(dropdownRef, () => setShow(false));

    const getFilterState = () => {
        const option = ASSET_STATE_OPTIONS.find((opt) => opt.value === filterState);
        return option ? option.label : 'State';
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                type="button"
                onClick={() => setShow(!show)}
                className="flex items-center justify-between w-[220px] h-[34px] text-sm py-1.5 px-2 border border-tertiary rounded bg-white hover:bg-gray-50"
            >
                <span className="truncate">{getFilterState()}</span>
                <div className="flex items-center">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" className="mr-1">
                        <path
                            d="M22 3H2l8 9.46V19l4 2v-8.54L22 3z"
                            stroke="currentColor"
                            strokeWidth="2"
                            strokeLinecap="round"
                            strokeLinejoin="round"
                        />
                    </svg>
                </div>
            </button>
            {show && (
                <div className="absolute top-full left-0 mt-1 w-[240px] bg-white border border-gray-200 rounded shadow-lg z-50">
                    <div className="py-1">
                        {ASSET_STATE_OPTIONS.map((option: { value: AssetState; label: string }) => (
                            <button
                                key={option.value}
                                className={`w-full text-left px-3 py-2 text-sm ${
                                    filterState === option.value
                                        ? 'bg-gray-100 font-medium'
                                        : 'hover:bg-gray-50'
                                }`}
                                onClick={() => {
                                    setFilterState(option.value);
                                    setShow(false);
                                }}
                            >
                                {option.label}
                            </button>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};

export default AssetStateDropdown;
