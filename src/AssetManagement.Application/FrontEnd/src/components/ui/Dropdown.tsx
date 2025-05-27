import React, { useState } from "react";

interface DropdownProps {
  items: {
    label: string;
    icon?: React.ReactNode;
    value: string; // Make value required to avoid fallback issues
  }[];
  onChange?: (value: string) => void; // Simplify to a function taking a string,
  title: string
}

export const DropdownMenu = ({ items, onChange, title }: DropdownProps) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleClick = (value: string) => {
    onChange?.(value);
    setIsOpen(false); // Close dropdown after selection
  };

  const toggleDropdown = () => {
    setIsOpen((prev) => !prev);
  };

  return (
    <div className="relative">
      <button
        id="dropdownDefaultButton"
        onClick={toggleDropdown}
        className="cursor-pointer text-white bg-transparent focus:ring-0 focus:outline-none font-medium rounded-lg text-sm px-5 py-2.5 text-center inline-flex items-center"
        type="button"
        aria-expanded={isOpen}
        aria-controls="dropdown"
      >
        {title}
        <svg
          className="w-2.5 h-2.5 ms-3"
          aria-hidden="true"
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 10 6"
        >
          <path
            stroke="currentColor"
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="2"
            d="m1 1 4 4 4-4"
          />
        </svg>
      </button>

      <div
        id="dropdown"
        className={`z-10 ${isOpen ? "block" : "hidden"} bg-tertiary divide-y divide-gray-100 rounded-lg shadow-sm absolute mt-2 w-[9rem]`}
        role="menu"
      >
        <ul
          className="py-2 text-sm text-quaternary"
          aria-labelledby="dropdownDefaultButton"
        >
          {items.map((item, index) => (
            <li key={index}>
              <button
                onClick={() => handleClick(item.value)}
                className="flex items-center w-full px-4 py-2 hover:bg-[#dc1a32] hover:text-white text-left cursor-pointer"
                role="menuitem"
              >
                {item.icon && <span className="mr-3">{item.icon}</span>}
                {item.label}
              </button>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};