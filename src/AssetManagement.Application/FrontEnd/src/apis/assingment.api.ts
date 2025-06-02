import http from '@/utils/http';
import { IAssginmentDetail, IAssignmentCreateUpdateRequest, IAssignmentCreateUpdateResponse } from '../types/assingment.type';

const assignmentApi = {
  createAssignment: async (payload: IAssignmentCreateUpdateRequest): Promise<HttpResponse<IAssignmentCreateUpdateResponse>> => {
    const { data } = await http.post('/assignments', payload);
    return data;
  },
  updateAssignment: async (id: string, payload: IAssignmentCreateUpdateRequest): Promise<HttpResponse<IAssignmentCreateUpdateResponse>> => {
    const { data } = await http.patch(`/assignments/${id}`, payload);
    return data;
  },
  getAssignmentDetails: async (id: string): Promise<HttpResponse<IAssginmentDetail>> => {
    const { data } = await http.get(`/assignments/${id}`);
    return data;
  },
};
export default assignmentApi;
