import http from "../utils/http";
import { ICategory, ICreateCategoryRequest } from "@/types/category.type.ts";

const categoryApi = {
  getCategories: async (): Promise<HttpResponse<ICategory[]>> => {
    const { data } = await http.get("/categories");
    return data;
  },

  createCategory: async (
    assetData: ICreateCategoryRequest,
  ): Promise<HttpResponse<ICategory>> => {
    const { data } = await http.post("/categories", assetData);
    return data;
  },
};

export default categoryApi;
