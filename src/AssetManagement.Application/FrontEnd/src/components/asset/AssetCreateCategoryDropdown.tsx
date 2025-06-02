import useClickOutside from "../../hooks/useClickOutside";
import { ICategory, ICreateCategoryRequest } from "@/types/category.type.ts";
import { Input } from "@/components/ui/forms/Input.tsx";
import { Button } from "@/components/ui/Button.tsx";
import { ChevronDown } from "lucide-react";
import * as React from "react";
import { classNames } from "@/libs/classNames.ts";
import { useState } from "react";
import useCategory from "@/hooks/useCategory.ts";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";

// Validation schema for category creation
const categorySchema = yup.object().shape({
  name: yup
    .string()
    .required("Category's Name is required")
    .max(20, "Category's Name can't be more than 20 characters long"),
  prefix: yup
    .string()
    .required("Category's Prefix is required")
    .length(2, "Category's Prefix must be exactly 2 characters long"),
});

type CategoryFormData = yup.InferType<typeof categorySchema>;

const AssetCreateCategoryDropdown: React.FC<{
  categories: ICategory[];
  selectedCategory: ICategory | undefined;
  setSelectedCategory: (category: ICategory) => void;
  show: boolean;
  setShow: (show: boolean) => void;
  dropdownRef: React.RefObject<HTMLDivElement>;
  disabled?: boolean;
}> = ({
  categories,
  selectedCategory,
  setSelectedCategory,
  show,
  setShow,
  dropdownRef,
  disabled = false,
}) => {
    const { useCreateCategory } = useCategory();
    const createCategoryMutation = useCreateCategory();
    const [showCreateCategoryForm, setShowCreateCategoryForm] = useState(false);

    const {
      register: registerCategory,
      handleSubmit: handleCategorySubmit,
      formState: {
        errors: categoryErrors,
        isValid: isCategoryInputValid,
        isSubmitting: isCategorySubmitting,
      },
      reset: resetCategoryForm,
    } = useForm<CategoryFormData>({
      resolver: yupResolver(categorySchema),
      mode: "all",
      defaultValues: {
        name: "",
        prefix: "",
      },
    });

    useClickOutside(dropdownRef, () => {
      setShow(false);
      setShowCreateCategoryForm(false);
      resetCategoryForm();
    });

    const handleCategorySelect = (category: ICategory) => {
      setSelectedCategory(category);
      setShow(false);
    };

    const onCategorySubmit = async (data: CategoryFormData) => {
      const categoryData: ICreateCategoryRequest = {
        name: data.name,
        prefix: data.prefix,
      };

      createCategoryMutation.mutate(categoryData);

      resetCategoryForm();
    };

    const handleCreateCategoryClick = () => {
      setShowCreateCategoryForm(true);
      resetCategoryForm();
    };

    const handleCancelCreateCategory = () => {
      setShowCreateCategoryForm(false);
      resetCategoryForm();
    };

    // Check if form has errors
    const hasErrors = Object.keys(categoryErrors).length > 0;
    const isSubmitDisabled =
      !isCategoryInputValid ||
      hasErrors ||
      isCategorySubmitting ||
      createCategoryMutation.isPending;

    return (
      <div className="relative" ref={dropdownRef}>
        <div className="flex items-center justify-between">
          <Input
            type="text"
            value={selectedCategory?.name || ""}
            className={classNames(
              "w-full h-[34px] text-sm py-1.5 px-2 cursor-pointer border-input placeholder:text-muted-foreground flex shadow-xs disabled:cursor-not-allowed disabled:opacity-50 transition-colors hover:border-gray-400",
              show ? "rounded-t-md!" : "rounded-md!",
              disabled ? "bg-gray-100 text-gray-500 border-gray-200" : "bg-white text-gray-900 border-gray-300"
            )}
            style={{
              borderColor: "var(--color-gray-300)",
              paddingRight: 36,
              borderRadius: 0,
              borderWidth: "1px",
            }}
            readOnly
            onClick={() => setShow(!show)}
            disabled={disabled}
          />

          <Button
            type="button"
            variant="ghost"
            className="h-[34px] text-muted-foreground absolute top-1/2 right-1 w-9 -translate-y-1/2 rounded-md"
            onClick={() => setShow(!show)}
            disabled={disabled}
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
              borderWidth: "1px",
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
                borderTop: "1px solid",
                borderTopColor: "var(--color-gray-300)",
              }}
              className="border-t"
            >
              {showCreateCategoryForm ? (
                <div className="px-2 py-1">
                  <div>
                    <div className="flex gap-1">
                      <div className="flex flex-[8]">
                        <Input
                          {...registerCategory("name")}
                          style={{
                            borderWidth: "1px",
                            borderRightWidth: "0.5px",
                          }}
                          type="text"
                          placeholder="Bluetooth Mouse"
                          className="flex-[7] rounded-tr-none rounded-br-none border-r-[0.5px] w-full h-[34px] text-sm py-1.5 px-2 border-input placeholder:text-muted-foreground flex shadow-xs disabled:cursor-not-allowed disabled:opacity-50 transition-colors bg-white text-gray-900 border-gray-300 hover:border-gray-400"
                        />

                        <Input
                          {...registerCategory("prefix")}
                          style={{
                            borderWidth: "1px",
                            borderLeftWidth: "0.5px",
                          }}
                          type="text"
                          placeholder="BM"
                          maxLength={2}
                          className="flex-[3] rounded-tl-none rounded-bl-none border-l-[0.5px] w-full h-[34px] text-sm py-1.5 px-2 border-input placeholder:text-muted-foreground flex shadow-xs disabled:cursor-not-allowed disabled:opacity-50 transition-colors bg-white text-gray-900 border-gray-300 hover:border-gray-400"
                        />
                      </div>

                      <div className="flex items-center flex-[2]">
                        <button
                          type="button"
                          disabled={isSubmitDisabled}
                          onClick={handleCategorySubmit(onCategorySubmit)}
                          className="w-full bg-transparent text-red-600 cursor-pointer disabled:cursor-not-allowed disabled:opacity-50"
                        >
                          {isCategorySubmitting ||
                            createCategoryMutation.isPending
                            ? "..."
                            : "✓"}
                        </button>

                        <button
                          type="button"
                          className="w-full bg-transparent text-black cursor-pointer"
                          onClick={handleCancelCreateCategory}
                        >
                          X
                        </button>
                      </div>
                    </div>
                  </div>

                  {/* Display validation errors */}
                  <div className="flex flex-col gap-0.5">
                    {categoryErrors.name && (
                      <p className="mt-1 text-sm text-red-600">
                        {categoryErrors.name.message}
                      </p>
                    )}

                    {categoryErrors.prefix && (
                      <p className="mt-1 text-sm text-red-600">
                        {categoryErrors.prefix.message}
                      </p>
                    )}
                  </div>
                </div>
              ) : (
                <Button
                  type="button"
                  style={{
                    backgroundColor: "#eff1f5",
                    textDecoration: "underline",
                    fontStyle: "italic",
                    padding: "0 10px",
                  }}
                  variant="ghost"
                  className="w-full h-10 text-sm text-red-600 hover:bg-red-50 rounded-none rounded-b-md justify-start px-3"
                  onClick={handleCreateCategoryClick}
                >
                  Add new category
                </Button>
              )}
            </div>
          </div>
        )}
      </div>
    );
  };

export default AssetCreateCategoryDropdown;
