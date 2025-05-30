import { IAssignment, IAssignmentParams } from "@/types/assignment.type";
import http from "@/utils/http";

const Base_URL = "assignments";

const assignmentApi = {
  getAssignments(params: IAssignmentParams) {
    return http.get<HttpResponse<PaginatedResult<IAssignment>>>(`${Base_URL}`, {
      params,
    });
  },
};

export default assignmentApi;
