export interface IReturnRequest {
  no: number;
  id: string;
  assetCode: string;
  assetName: string;
  requestedBy: string;
  assignedDate: string;
  acceptedBy?: string;
  returnedDate?: string;
  state: ReturnRequestState;
}

export type ReturnRequestState =
  | "Completed"
  | "Waiting for returning";

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
    returnRequestStatus: string;
}
