export interface IAssignment {
  no: number;
  id: string;
  assetCode: string;
  assetName: string;
  assignedTo: string;
  assignedBy: string;
  assignedDate: string;
  state: AssignmentState;
  note?: string; // Add optional note field
}

export type AssignmentState =
  | "Accepted"
  | "Declined"
  | "Returned"
  | "Waiting for acceptance";

export interface IAssignmentParams {
  searchTerm?: string;
  sortBy?: string;
  // name:asc,code:desc
  state?: string;
  // Accepted,Declined,Returned,Waiting for acceptance
  date?: string;
  pageNumber?: number;
  pageSize?: number;
}
