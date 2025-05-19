/* eslint-disable @typescript-eslint/no-unused-expressions */
import * as React from "react";
import { Column } from "@tanstack/react-table";
import { Check } from "lucide-react";
import { useRef, useState, useEffect } from "react";

interface DataTableFacetedFilterProps<TData, TValue> {
  column?: Column<TData, TValue>;
  title?: string;
  options: {
    label: string;
    value: string;
    icon?: React.ComponentType<{ className?: string }>;
  }[];
  setSelect?: (value: string[]) => void;
  setSingleSelect?: (value: string) => void;
}

export function DataTableFacetedFilter<TData, TValue>({
  column,
  title,
  options,
  setSelect,
  setSingleSelect,
}: DataTableFacetedFilterProps<TData, TValue>) {
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const selectedValues = new Set(column?.getFilterValue() as string[]);
  const [isAllSelected, setIsAllSelected] = useState(true);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setShowDropdown(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  const handleAllOption = () => {
    if (!isAllSelected) {
      setIsAllSelected(true);
      column?.setFilterValue(undefined);
      setSelect && setSelect([]);
      setSingleSelect && setSingleSelect("");
    } else {
      setIsAllSelected(false);
    }
  };

  const handleOptionClick = (value: string) => {
    setIsAllSelected(false);
    const isSelected = selectedValues.has(value);

    if (isSelected) {
      selectedValues.delete(value);
    } else {
      selectedValues.add(value);
    }

    const filterValues = Array.from(selectedValues);
    column?.setFilterValue(filterValues?.length ? filterValues : undefined);

    setSelect && setSelect(filterValues);
    setSingleSelect && setSingleSelect(filterValues?.length ? filterValues[0] : "");
  };

  return (
    <div className="relative" ref={dropdownRef}>
      <div className="relative">
        <input
          type="text"
          value={title || "Type"}
          readOnly
          onClick={() => setShowDropdown(!showDropdown)}
          className="w-[110px] h-[34px] text-sm py-1.5 px-2 border border-gray-300 rounded cursor-pointer"
        />
        <button
          className="absolute inset-y-0 right-0 flex items-center pr-2"
          onClick={() => setShowDropdown(!showDropdown)}
        >
          <svg
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M6 9L12 15L18 9"
              stroke="black"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
        </button>
      </div>

      {showDropdown && (
        <div className="absolute top-full left-0 mt-0.5 w-[110px] bg-white border border-gray-300 shadow-sm z-10">
          <div className="p-2">
            <label className="flex items-center space-x-2 mb-1.5 cursor-pointer">
              <div
                className={`flex h-4 w-4 items-center justify-center rounded border border-primary ${isAllSelected ? "bg-red-600" : ""}`}
                onClick={handleAllOption}
              >
                {isAllSelected && <Check className="h-3 w-3 text-white" />}
              </div>
              <span className="text-sm">All</span>
            </label>

            {options.map((option) => {
              const isSelected = selectedValues.has(option.value);
              return (
                <label
                  key={option.value}
                  className="flex items-center space-x-2 mb-1.5 cursor-pointer"
                >
                  <div
                    className={`flex h-4 w-4 items-center justify-center rounded border border-primary ${isSelected ? "bg-red-600" : ""}`}
                    onClick={() => handleOptionClick(option.value)}
                  >
                    {isSelected && <Check className="h-3 w-3 text-white" />}
                  </div>
                  <span className="text-sm">{option.label}</span>
                </label>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}