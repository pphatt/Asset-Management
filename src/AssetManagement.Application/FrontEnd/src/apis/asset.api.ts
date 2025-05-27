import {
  IAsset,
  IAssetCategory,
  IAssetDetails,
  IAssetParams,
  IAssetState,
} from "@/types/asset.type";
import http from "../utils/http";

const assetApi = {
  getAssets: async (
    params: IAssetParams,
  ): Promise<HttpResponse<PaginatedResult<IAsset>>> => {
    const { data } = await http.get("/assets", { params });
    return data;
  },

  getAssetCategories: async (): Promise<HttpResponse<IAssetCategory[]>> => {
    const { data } = await http.get("/assets/categories");
    return data;
  },

  getAssetStates: async (): Promise<HttpResponse<IAssetState[]>> => {
    const { data } = await http.get("/assets/states");
    return data;
  },

  getAssetByAssetCode: async (
    assetCode: string,
  ): Promise<HttpResponse<IAssetDetails>> => {
    const { data } = await http.get(`/assets/${assetCode}`);
    console.log(data);
    return data;
  },
};

export default assetApi;
