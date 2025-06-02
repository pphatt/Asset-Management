import { IAsset } from './asset.type';
import { AssignmentState } from './assignment.type';
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
  state: AssignmentState;
}

export interface IAssginmentDetail {
  id: string;
  no: number;
  assetId: string;
  assetCode: string;
  assetName: string;
  assignedTo: string;
  assignedToId: string;
  assignedBy: string;
  assignedById: string;
  assignedDate: string;
  state: AssignmentState;
  specification: string;
  note: string;
}
