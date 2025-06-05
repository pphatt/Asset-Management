/* eslint-disable @typescript-eslint/no-empty-object-type */
import { AssetCategory, AssetState } from "../constants/asset-params";
import { LocationEnum, UserTypeEnum } from "@/types/user.type.ts";

export type IAssetState =
  | "Assigned"
  | "Available"
  | "Not available"
  | "Waiting for recycling"
  | "Recycled";

export const GetAssetState = (state: IAssetState | undefined) => {
  switch (state) {
    case "Assigned":
      return 0;
    case "Available":
      return 1;
    case "Not available":
      return 2;
    case "Waiting for recycling":
      return 3;
    case "Recycled":
      return 4;
    default:
      return undefined;
  }
};
export interface IAsset {
  id: string;
  code: string;
  name: string;
  categoryName: string;
  state: IAssetState;
  hasAssignments: boolean;
}

export interface IAssetCategory {
  id: string;
  name: string;
}

export interface IAssetParams {
  searchTerm: string;
  sortBy?: string;
  _apiSortBy?: string;
  assetCategories?: AssetCategory[];
  assetStates?: AssetState[];
  pageNumber?: number;
  pageSize?: number;
  location?: string; // Add location for filtering by admin location
}

export interface IAssetDetails {
  id: string;
  name: string;
  code: string;
  installedDate: string;
  type: UserTypeEnum;
  categoryName: string;
  specification: string;
  location: LocationEnum;
  state: IAssetState;
  assignments: IAssetDetailsHistory[];
  categoryId?: string;
  hasAssignments: boolean;
}

export interface IAssetDetailsHistory {
  date: string;
  assignedTo: string;
  assignedBy: string;
  returnedDate: string;
}

export interface ICreateAssetRequest {
  name: string;
  categoryId: string;
  specifications: string;
  installedDate: string; // Format: YYYY-MM-DD
  state: number;
}

export interface IUpdateAssetRequest extends ICreateAssetRequest {}
