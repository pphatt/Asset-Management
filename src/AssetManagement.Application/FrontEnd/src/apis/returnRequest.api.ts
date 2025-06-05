import {
  IReturnRequest,
  IReturnRequestParams, 
  ICreateReturnRequestResponse
} from "@/types/returnRequest.type";
import http from "@/utils/http";

const Base_URL = "returnrequests";

const returnRequestApi = {
  getRequests(params: IReturnRequestParams) {
      return http.get<HttpResponse<PaginatedResult<IReturnRequest>>>(
          `${Base_URL}`,
          {
              params,
          }
      );
  },
    createReturnRequest: async (assignmentId: string): Promise<HttpResponse<ICreateReturnRequestResponse>> => {
        const response = await http.post('/returnrequests', { assignmentId });
        return response.data;
    },
};

export default returnRequestApi;
