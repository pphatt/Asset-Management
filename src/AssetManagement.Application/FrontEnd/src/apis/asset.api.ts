import {
  IAsset,
  IAssetDetails,
  IAssetParams,
  IAssetState,
  ICreateAssetRequest,
  IUpdateAssetRequest,
} from "@/types/asset.type";
import http from "../utils/http";
import { IAssetReport, IAssetReportParams } from "@/types/report.type";

const assetApi = {
  getAssets: async (
    params: IAssetParams,
  ): Promise<HttpResponse<PaginatedResult<IAsset>>> => {
    const { data } = await http.get("/assets", { params });
    return data;
  },

  getAssetStates: async (): Promise<HttpResponse<IAssetState[]>> => {
    const { data } = await http.get("/assets/states");
    return data;
  },

  getAssetByAssetId: async (
    assetId: string,
  ): Promise<HttpResponse<IAssetDetails>> => {
    const { data } = await http.get(`/assets/${assetId}`);
    return data;
  },

  createAsset: async (
    assetData: ICreateAssetRequest,
  ): Promise<HttpResponse<IAsset>> => {
    const { data } = await http.post("/assets", assetData);
    return data;
  },

  updateAsset: async (
    assetCode: string,
    assetData: IUpdateAssetRequest,
  ): Promise<HttpResponse<IAsset>> => {
    const { data } = await http.patch(`/assets/${assetCode}`, assetData);

    return data;
  },

  deleteAsset: async (assetId: string): Promise<HttpResponse<void>> => {
    const { data } = await http.delete(`/assets/${assetId}`);
    return data;
  },

  getReport: async (
    params: IAssetReportParams = {},
  ): Promise<HttpResponse<IAssetReport[]>> => {
    const { data } = await http.get("/assets/report", { params });
    return data;
  },
};

export default assetApi;
