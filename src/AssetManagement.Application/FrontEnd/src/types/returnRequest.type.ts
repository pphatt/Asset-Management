export interface IReturnRequest {
  no: number;
  id: string;
  assetCode: string;
  assetName: string;
  requestedBy: string;
  assignedDate: string;
  acceptedBy?: string;
  returnedDate?: string;
  state: AssignmentState;
}

export type AssignmentState =
  | "Accepted"
  | "Declined"
  | "Returned"
  | "Waiting for acceptance";

export interface IReturnRequestParams {
  searchTerm?: string;
  sortBy?: string;
  // name:asc,code:desc
  states?: string[];
  // Completed,Waiting for acceptance
  date?: string;
  returnedDate?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface ICreateReturnRequestResponse {
    assetCode: string;
    assignmentStatus: string;
}
