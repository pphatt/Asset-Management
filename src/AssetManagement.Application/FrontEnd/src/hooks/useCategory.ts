import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "react-toastify";
import categoryApi from "@/apis/category.api.ts";
import { ICreateCategoryRequest } from "@/types/category.type.ts";

export function useCategory() {
  const queryClient = useQueryClient();

  function useCategories() {
    return useQuery({
      queryKey: ["categories"],
      queryFn: async () => {
        const response = await categoryApi.getCategories();
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || "Failed to fetch asset categories");
      },
      staleTime: 24 * 60 * 60 * 1000,
    });
  }

  /**
   * Create a new category
   * @returns {UseMutationResult<ICategory>} The new category
   * @description Create a new category from the API
   * @technique {useMutation} -> Mutate the new category from the API
   */
  function useCreateCategory() {
    return useMutation({
      mutationFn: async (categoryData: ICreateCategoryRequest) =>
        categoryApi.createCategory(categoryData),
      onSuccess: () => {
        queryClient.invalidateQueries({
          queryKey: ["categories"],
          exact: false,
        });
        toast.success("Category created successfully");
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] || "Error creating category");
      },
    });
  }

  return {
    useCategories,
    useCreateCategory,
  };
}

export default useCategory;
