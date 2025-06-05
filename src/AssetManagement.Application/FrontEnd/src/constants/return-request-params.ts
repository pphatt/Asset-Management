import { ReturnRequestState } from "@/types/returnRequest.type";

export const RETURN_REQUEST_STATE: Record<string, ReturnRequestState> = {
  COMPLETED: "Completed",
  WAITING_FOR_RETURNING: "Waiting for returning",
};

export const RETURN_REQUEST_STATE_OPTIONS = [
  { value: "All", label: "All" },
  { value: RETURN_REQUEST_STATE.COMPLETED, label: "Completed" },
  {
    value: RETURN_REQUEST_STATE.WAITING_FOR_RETURNING,
    label: "Waiting for returning",
  }
];
