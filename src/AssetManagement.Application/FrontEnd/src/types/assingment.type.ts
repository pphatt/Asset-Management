import { IAsset } from './asset.type';
import { IUser } from './user.type';

export interface IAssignmentFormData {
  user: IUser | null;
  asset: IAsset | null;
  assignedDate: Date;
  note?: string;
}

export interface IAssignmentCreateUpdateRequest {
  assetId: string;
  assigneeId: string;
  assignedDate: string;
  note: string;
}

export interface IAssignmentCreateUpdateResponse {
  id: string;
  assetCode: string;
  assetName: string;
  assignedTo: string;
  assignedBy: string;
  assignedDate: Date;
  state: string;
}

export interface IAssginmentDetail {
  id: string;
  no: string;
  assetCode: string;
  assetName: string;
  assignedTo: string;
  assignedBy: string;
  assignedDate: string;
  state: string;
  note: string;
}
