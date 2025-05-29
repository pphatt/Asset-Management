import useClickOutside from "../../hooks/useClickOutside";
import { ICategory } from "@/types/category.type.ts";
import { Input } from "@/components/ui/forms/Input.tsx";
import { Button } from "@/components/ui/Button.tsx";
import { ChevronDown } from "lucide-react";
import * as React from "react";
import { classNames } from "@/libs/classNames.ts";

const AssetCreateCategoryDropdown: React.FC<{
  categories: ICategory[];
  selectedCategory: ICategory | undefined;
  setSelectedCategory: (category: ICategory) => void;
  show: boolean;
  setShow: (show: boolean) => void;
  dropdownRef: React.RefObject<HTMLDivElement>;
}> = ({
  categories,
  selectedCategory,
  setSelectedCategory,
  show,
  setShow,
  dropdownRef,
}) => {
  useClickOutside(dropdownRef, () => setShow(false));

  const handleCategorySelect = (category: ICategory) => {
    setSelectedCategory(category);
    setShow(false);
  };

  return (
    <div className="relative" ref={dropdownRef}>
      <div className="flex items-center justify-between">
        <Input
          type="text"
          value={selectedCategory?.name || ""}
          className={classNames(
            "w-full h-[34px] text-sm py-1.5 px-2 cursor-pointer border-input placeholder:text-muted-foreground flex shadow-xs disabled:cursor-not-allowed disabled:opacity-50 transition-colors bg-white text-gray-900 border-gray-300 hover:border-gray-400",
            show ? "rounded-t-md!" : "rounded-md!",
          )}
          style={{
            borderColor: "var(--color-gray-300)",
            paddingRight: 36,
            borderRadius: 0,
          }}
          readOnly
          onClick={() => setShow(!show)}
        />

        <Button
          type="button"
          variant="ghost"
          className="h-[34px] text-muted-foreground absolute top-1/2 right-1 w-9 -translate-y-1/2 rounded-md"
          onClick={() => setShow(!show)}
        >
          <ChevronDown className="h-4 w-4" />
        </Button>
      </div>

      {show && (
        <div
          style={{
            backgroundColor: "#f5f5f5",
            border: "2px solid",
            borderColor: "var(--color-gray-300)",
            borderTop: "0px",
          }}
          className="absolute top-full left-0 w-full bg-white border border-gray-300 border-t-0 rounded-b-md shadow-lg z-50 max-h-[200px] overflow-y-auto"
        >
          <div style={{ padding: "10px" }} className="py-1">
            {categories.map((category) => (
              <div
                key={category.id}
                className="relative flex cursor-pointer gap-2 select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none hover:bg-gray-200 transition-colors"
                onClick={() => handleCategorySelect(category)}
              >
                <span className="truncate">{category.name}</span>
                {selectedCategory?.id === category.id && (
                  <span className="ml-auto text-blue-600">✓</span>
                )}
              </div>
            ))}

            {categories.length === 0 && (
              <div className="px-3 py-2 text-sm text-gray-500">
                No categories available
              </div>
            )}
          </div>

          <div
            style={{
              borderTop: "2px solid",
              borderTopColor: "var(--color-gray-300)",
            }}
            className="border-t"
          >
            <Button
              style={{
                backgroundColor: "#eff1f5",
                textDecoration: "underline",
                fontStyle: "italic",
                padding: "0 10px",
              }}
              variant="ghost"
              className="w-full h-10 text-sm text-red-600 hover:bg-red-50 rounded-none rounded-b-md justify-start px-3"
              onClick={() => {
                setShow(false);
              }}
            >
              Add new category
            </Button>
          </div>
        </div>
      )}
    </div>
  );
};

export default AssetCreateCategoryDropdown;
