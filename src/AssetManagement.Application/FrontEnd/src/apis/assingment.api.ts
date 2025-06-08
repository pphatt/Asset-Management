import http from '@/utils/http';
import { IMyAssignment, IMyAssignmentParams } from '@/types/assignment.type';
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

  deleteAssignment: async (id: string): Promise<HttpResponse<void>> => {
    const { data } = await http.delete(`/assignments/${id}`);
    return data;
  },

  acceptAssignment: async (id: string): Promise<HttpResponse<void>> => {
    const { data } = await http.post(`/assignments/${id}/accept`, null);
    return data;
  },

  declineAssignment: async (id: string): Promise<HttpResponse<void>> => {
    const { data } = await http.post(`/assignments/${id}/decline`, null);
    return data;
  },

  getMyAssignments: async (params: IMyAssignmentParams): Promise<PaginatedResult<IMyAssignment>> => {
    const { data } = await http.get('/assignments/my-assignments', { params });

    return data.data;
  },

};

export default assignmentApi;
