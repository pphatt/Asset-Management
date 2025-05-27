import { AssetCategory, AssetState } from "../constants/asset-params";
import { LocationEnum, UserTypeEnum } from "@/types/user.type.ts";

export type IAssetState =
    | "Assigned"
    | "Available"
    | "Not available"
    | "Waiting for recycling"
    | "Recycled";

export interface IAsset {
    id: string;
    code: string;
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
    history: IAssetDetailsHistory[];
}

export interface IAssetDetailsHistory {
    date: string;
    assignedTo: string;
    assignedBy: string;
    returnedDate: string;
}
