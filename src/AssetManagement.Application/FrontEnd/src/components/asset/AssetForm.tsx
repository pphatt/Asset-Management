import { yupResolver } from "@hookform/resolvers/yup";
import { Controller, useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import * as yup from "yup";
import useAsset from "@/hooks/useAsset.ts";
import { GetAssetState, ICreateAssetRequest, IUpdateAssetRequest } from "@/types/asset.type.ts";
import AssetCreateCategoryDropdown from "@/components/asset/AssetCreateCategoryDropdown.tsx";
import useCategory from "@/hooks/useCategory.ts";
import React, { useEffect, useRef, useState } from "react";
import { ICategory } from "@/types/category.type.ts";
import { formatDateToString } from "@/utils/formatDate";

const createSchema = (mode: "create" | "edit") => {
  const baseSchema = {
    name: yup
      .string()
      .required("Asset's Name is required")
      .max(30, "Asset's Name can't be more than 30 characters long"),
    category: yup.string().required("Asset's Category is required"),
    specification: yup.string().required("Asset's Specification is required"),
    installedDate: yup
      .string()
      .required("Please Select Installed Date")
      .test(
        "valid-date",
        "Invalid date format",
        (value) => !isNaN(Date.parse(value)),
      )
      .test(
        "not-future-date",
        "Installed date cannot be in the future. Please select a different date",
        (value) => {
          const selectedDate = new Date(value);
          const today = new Date();
          return selectedDate <= today;
        },
      ),
  };

  if (mode === "create") {
    return yup.object().shape({
      ...baseSchema,
      state: yup
        .number()
        .oneOf([1, 2], "Invalid state")
        .required("State is required"),
    });
  } else {
    return yup.object().shape({
      ...baseSchema,
      state: yup
        .number()
        .oneOf([0, 1, 2, 3, 4], "Invalid state")
        .required("State is required"),
    });
  }
};


interface AssetFormProps {
  mode: "create" | "edit";
}

export const AssetForm: React.FC<AssetFormProps> = ({
  mode,
}: AssetFormProps) => {
  const schema = createSchema(mode);
  type FormData = yup.InferType<typeof schema>;

  const navigate = useNavigate();
  const { assetId } = useParams<{ assetId: string }>();

  const isEditMode = mode === "edit";

  const [selectedCategory, setSelectedCategory] = useState<ICategory>();
  const [showCategoryDropdown, setShowCategoryDropdown] = useState(false);
  const categoryDropdownRef = useRef<HTMLDivElement>(
    null,
  ) as React.RefObject<HTMLDivElement>;

  const { useCreateAsset, useAssetByAssetCode, useUpdateAsset } = useAsset();
  const createAssetMutation = useCreateAsset();
  const updateAsset = useUpdateAsset();

  const { useCategories } = useCategory();
  const {
    data: categoryData,
  } = useCategories();

  const { data: assetData } = useAssetByAssetCode(assetId ?? "");

  const {
    register,
    handleSubmit,
    control,
    reset,
    setValue,
    formState: { errors, isSubmitting, isValid },
  } = useForm<FormData>({
    resolver: yupResolver(schema),
    mode: "onChange",
    defaultValues: {
      name: "",
      category: "",
      specification: "",
      installedDate: undefined,
      state: undefined,
    },
  });

  useEffect(() => {
    if (isEditMode && assetData) {
      reset({
        name: assetData.name,
        category: assetData.categoryId,
        specification: assetData.specification,
        installedDate: formatDateToString(new Date(assetData.installedDate)),
        state: GetAssetState(assetData.state),
      });
    }

    if (categoryData && assetData?.categoryId) {
      const category = categoryData.find(cat => cat.id === assetData.categoryId);
      if (category) {
        setSelectedCategory(category);
      }
    }
  }, [assetData, isEditMode, reset, categoryData]);

  const onSubmit = (data: FormData) => {
    if (isEditMode) {
      const assetUpdateData: IUpdateAssetRequest = {
        name: data.name,
        categoryId: data.category,
        specifications: data.specification,
        installedDate: data.installedDate,
        state: data.state,
      };

      updateAsset.mutate({
        assetCode: assetData?.code ?? "",
        assetData: assetUpdateData
      });

    } else {
      const assetData: ICreateAssetRequest = {
        name: data.name,
        categoryId: data.category,
        specifications: data.specification,
        installedDate: data.installedDate,
        state: data.state,
      };

      createAssetMutation.mutate(assetData);
    }
  };

  const onCancel = () => {
    navigate(-1);
  };

  // Check if form has errors
  const hasErrors = Object.keys(errors).length > 0;
  const isSubmitDisabled =
    !isValid || hasErrors || isSubmitting || createAssetMutation.isPending;

  const getStateOptions = () => {
    if (mode === "create") {
      return [
        { value: 1, label: "Available", id: "available" },
        { value: 2, label: "Not Available", id: "not-available" },
      ];
    } else {
      return [
        { value: 1, label: "Available", id: "available" },
        { value: 2, label: "Not Available", id: "not-available" },
        { value: 3, label: "Waiting for Recycling", id: "waiting-recycling" },
        { value: 4, label: "Recycled", id: "recycled" },
      ];
    }
  };

  return (
    <div className="max-w-md mx-auto p-6 bg-white border-gray-200">
      <h2 className="text-primary text-2xl font-bold mb-6">
        {isEditMode ? "Edit Asset" : "Create New Asset"}
      </h2>

      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="space-y-5">
          {/* Name */}
          <div className="grid grid-cols-12 items-center">
            <label className="col-span-4 font-medium text-gray-700">
              Name
              <span className="ml-0.5 text-red-500">*</span>
            </label>
            <div className="col-span-8">
              <input
                type="text"
                {...register("name")}
                className={`w-full p-2 border rounded-md focus:ring-2 focus:ring-primary focus:border-primary transition-colors bg-white text-gray-900 border-gray-300 hover:border-gray-400}`}
              />

              {errors.name && (
                <p className="mt-1 text-sm text-red-600">
                  {errors.name.message}
                </p>
              )}
            </div>
          </div>

          {/* Category */}
          <div className="grid grid-cols-12 items-center">
            <label className="col-span-4 font-medium text-gray-700">
              Category
              <span className="ml-0.5 text-red-500">*</span>
            </label>
            <div className="col-span-8">
              <AssetCreateCategoryDropdown
                categories={categoryData ?? []}
                dropdownRef={categoryDropdownRef}
                selectedCategory={selectedCategory}
                setSelectedCategory={(category: ICategory) => {
                  setSelectedCategory(category);
                  setValue("category", category.id);
                }}
                show={showCategoryDropdown}
                setShow={setShowCategoryDropdown}
                disabled={isEditMode}
              />

              {errors.category && (
                <p className="mt-1 text-sm text-red-600">
                  {errors.category.message}
                </p>
              )}
            </div>
          </div>

          {/* Specification */}
          <div className="grid grid-cols-12 items-center">
            <label className="col-span-4 font-medium text-gray-700">
              Specification
              <span className="ml-0.5 text-red-500">*</span>
            </label>
            <div className="col-span-8">
              <textarea
                {...register("specification")}
                className={`w-full p-2 border rounded-md focus:ring-2 focus:ring-primary focus:border-primary transition-colors bg-white text-gray-900 border-gray-300 hover:border-gray-400`}
              />

              {errors.specification && (
                <p className="mt-1 text-sm text-red-600">
                  {errors.specification.message}
                </p>
              )}
            </div>
          </div>

          {/* Installed Date */}
          <div className="grid grid-cols-12 items-center">
            <label
              htmlFor="installedDate"
              className="col-span-4 font-medium text-gray-700"
            >
              Installed Date
              <span className="ml-0.5 text-red-500">*</span>
            </label>
            <div className="col-span-8">
              <input
                id="installedDate"
                type="date"
                min="1000-01-01"
                max="9999-12-31"
                // RHF stores this as a string; Yup will parse it
                {...register("installedDate")}
                className="w-full p-2 border rounded-md focus:ring-2 focus:ring-primary focus:border-primary transition-colors"
              />

              {errors.installedDate && (
                <p className="mt-1 text-sm text-red-600">
                  {errors.installedDate.message}
                </p>
              )}
            </div>
          </div>

          {/* State */}
          <div className="grid grid-cols-12 items-center">
            <label className="col-span-4 font-medium text-gray-700">
              State
              <span className="ml-0.5 text-red-500">*</span>
            </label>
            <Controller
              name="state"
              defaultValue={1}
              control={control}
              render={({ field }) => (
                <div className={`col-span-8 flex space-x-6 ${mode === 'edit' ? 'flex-col space-y-2' : ''}`}>
                  {getStateOptions().map((option) => (
                    <div key={option.value} className="flex items-center">
                      <input
                        type="radio"
                        id={option.id}
                        value={option.value}
                        checked={field.value === option.value}
                        onChange={() => field.onChange(option.value)}
                        className="h-4 w-4 accent-primary focus:ring-primary border-gray-300"
                      />
                      <label htmlFor={option.id} className="ml-2 text-gray-700">
                        {option.label}
                      </label>
                    </div>
                  ))}
                </div>
              )}
            />

            {errors.state && (
              <p className="col-start-5 col-span-8 mt-1 text-sm text-red-600">
                {errors.state.message}
              </p>
            )}
          </div>
        </div>

        {/* Buttons */}
        <div className="mt-8 flex justify-end space-x-4">
          <button
            type="submit"
            disabled={isSubmitDisabled}
            className={`px-4 py-2 rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 transition-colors ${isSubmitDisabled
              ? "bg-gray-300 text-gray-500 cursor-not-allowed"
              : "bg-primary text-white hover:bg-primary-dark focus:ring-primary"
              }`}
          >
            {isSubmitting || createAssetMutation.isPending
              ? "Saving..."
              : "Save"}
          </button>

          <button
            type="button"
            className="px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-400 transition-colors"
            onClick={onCancel}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};
