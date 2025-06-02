import {
  IAsset,
  IAssetDetails,
  IAssetParams,
  IAssetState,
  ICreateAssetRequest,
} from "@/types/asset.type";
import http from "../utils/http";

const assetApi = {
  getAssets: async (
    params: IAssetParams
  ): Promise<HttpResponse<PaginatedResult<IAsset>>> => {
    const { data } = await http.get("/assets", { params });
    return data;
  },

  getAssetStates: async (): Promise<HttpResponse<IAssetState[]>> => {
    const { data } = await http.get("/assets/states");
    return data;
  },

  getAssetByAssetCode: async (
    assetCode: string
  ): Promise<HttpResponse<IAssetDetails>> => {
    const { data } = await http.get(`/assets/${assetCode}`);
    return data;
  },

  createAsset: async (
    assetData: ICreateAssetRequest,
  ): Promise<HttpResponse<IAsset>> => {
    const { data } = await http.post("/assets", assetData);
    return data;
  },
};

export default assetApi;
