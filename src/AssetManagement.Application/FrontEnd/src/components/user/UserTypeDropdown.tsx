import { USER_TYPE_OPTIONS, UserType } from '../../constants/user-params';
import useClickOutside from '../../hooks/useClickOutside';

const UserTypeDropdown: React.FC<{
  filterType: UserType;
  setFilterType: (type: UserType) => void;
  show: boolean;
  setShow: (show: boolean) => void;
  dropdownRef: React.RefObject<HTMLDivElement>;
}> = ({ filterType, setFilterType, show, setShow, dropdownRef }) => {
  useClickOutside(dropdownRef, () => setShow(false));

  return (
    <div className="relative" ref={dropdownRef}>
      {' '}
      <div className="flex items-center justify-between w-[120px]">
        <input
          type="text"
          readOnly
          placeholder='Type'
          className="w-full h-[34px] text-sm py-1.5 px-2 border border-quaternary rounded-l bg-white cursor-pointer"
          onClick={() => setShow(!show)}
        />
        <button
          type="button"
          onClick={() => setShow(!show)}
          className="flex items-center justify-center h-[34px] w-[34px] border border-l-0 border-quaternary rounded-r bg-white hover:bg-gray-50"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" stroke="black" strokeWidth="2">
            <path d="M22 3H2l8 9.46V19l4 2v-8.54L22 3z" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        </button>
      </div>
      {show && (
        <div className="absolute top-full left-0 mt-1 w-[140px] bg-white border border-gray-200 rounded shadow-lg z-50">
          <div className="py-1">
            {USER_TYPE_OPTIONS.map((option) => (
              <label key={option.value} className="flex items-center w-full px-3 py-2 text-sm hover:bg-gray-50 cursor-pointer">
                <input
                  type="checkbox"
                  className="mr-2 h-4 w-4 accent-red-600"
                  checked={filterType === option.value}
                  onChange={() => {
                    setFilterType(option.value);
                    // Don't close the dropdown when option is clicked
                  }}
                />
                {option.label}
              </label>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default UserTypeDropdown;
