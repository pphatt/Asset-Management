export interface IAssignment {
  no: number;
  id: string;
  assetCode: string;
  assetName: string;
  assignedTo: string;
  assignedBy: string;
  assignedDate: string;
  state: AssignmentState;
}

export interface IAssignmentDetails extends IAssignment {
  specifications: string;
  note: string;
}

export type AssignmentState =
  | "Accepted"
  | "Declined"
  | "Returned"
  | "Waiting for acceptance"
  | "Waiting for returning";

export interface IAssignmentParams {
  searchTerm?: string;
  sortBy?: string;
  // name:asc,code:desc
  states?: string[];
  // Accepted,Declined,Returned,Waiting for acceptance
  date?: string;
  returnedDate?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface IMyAssignment {
  assignmentId?: string;
  assetCode: string;
  assetName: string;
  category: string;
  assignedDate: string;
  state: AssignmentState;
}

export interface IMyAssignmentParams {
  sortBy?: string;
  pageNumber?: number;
  pageSize?: number;
}
