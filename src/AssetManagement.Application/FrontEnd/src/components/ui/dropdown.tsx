import React from "react";

interface DropdownProps {
    items: {
        label: string;
        icon?: React.ReactNode;
        value?: string;
    }[];
    onChange?: React.Dispatch<React.SetStateAction<string>>;
}

export const DropdownMenu = ({ items, onChange }: DropdownProps) => {
    const handleClick = (value: string) => {
        if (value) {
            if (onChange) {
                onChange(value);
            }
        }
    };

    return <>
        <button id="dropdownDefaultButton" data-dropdown-toggle="dropdown" className="text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center inline-flex items-center dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800" type="button">Dropdown button <svg className="w-2.5 h-2.5 ms-3" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 10 6">
            <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m1 1 4 4 4-4" />
        </svg>
        </button>

        <div id="dropdown" className="z-10 hidden bg-white divide-y divide-gray-100 rounded-lg shadow-sm w-44 dark:bg-gray-700">
            <ul className="py-2 text-sm text-gray-700 dark:text-gray-200" aria-labelledby="dropdownDefaultButton">
                {items.map((item, index) => (
                    <li key={index}>
                        <button onClick={() => handleClick(item.value ?? item.label)} value={item.value} className="flex items-center w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">
                            {item.icon && <span className="mr-2">{item.icon}</span>}
                            {item.label}
                        </button>
                    </li>
                ))}
            </ul>
        </div>

    </>
}