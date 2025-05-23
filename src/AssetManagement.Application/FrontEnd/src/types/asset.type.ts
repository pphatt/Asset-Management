// Define asset states for the application

import { AssetState } from "../constants/asset-params";

export type IAssetState = "Assigned" | "Available" | "NotAvailable" | "WaitingRecycling" | "Recycled";

export interface IAsset {
  assetCode: string;
  name: string;
  categoryName: string;
  state: IAssetState;
}

export interface IAssetCategory {
  id: string;
  name: string;
}

export interface IAssetParams {
  searchTerm: string;
  sortBy?: string;
  _apiSortBy?: string;
  assetCategory?: string;
  assetState?: AssetState;
  pageNumber?: number;
  pageSize?: number;
  location?: string; // Add location for filtering by admin location
}
