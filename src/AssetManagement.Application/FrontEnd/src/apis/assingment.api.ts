import http from '@/utils/http';
import { IAssignmentCreateUpdateRequest, IAssignmentCreateUpdateResponse } from '../types/assingment.type';

const assignmentApi = {
  createAssignment: async (payload: IAssignmentCreateUpdateRequest): Promise<HttpResponse<IAssignmentCreateUpdateResponse>> => {
    const { data } = await http.post('/assignments', payload);
    return data;
  },
  updateAssignment: async (id: string, payload: IAssignmentCreateUpdateRequest): Promise<HttpResponse<IAssignmentCreateUpdateResponse>> => {
    const { data } = await http.put(`/assignments/${id}`, payload);
    return data;
  },
};
export default assignmentApi;
