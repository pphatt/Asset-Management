import {
  IAssignment,
  IAssignmentDetails,
  IAssignmentParams,
} from "@/types/assignment.type";
import http from "@/utils/http";

const Base_URL = "assignments";

const assignmentApi = {
  getAssignments(params: IAssignmentParams) {
    return http.get<HttpResponse<PaginatedResult<IAssignment>>>(`${Base_URL}`, {
      params,
    });
  },

  async getAssigmentDetails(id: string) {
    const { data } = await http.get<HttpResponse<IAssignmentDetails>>(
      `${Base_URL}/${id}`,
    );

    return data;
  },
};

export default assignmentApi;
