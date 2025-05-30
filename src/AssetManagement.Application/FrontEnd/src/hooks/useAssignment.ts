import assignmentApi from '@/apis/assingment.api';
import path from '@/constants/path';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { IAssignmentCreateUpdateRequest } from '../types/assingment.type';

export function useAssignment() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  /**
   * Create a new assignment
   * @returns {UseMutationResult} The mutation result
   * @description Create a new assignment via the API
   */
  function useCreateAssignment() {
    return useMutation({
      mutationFn: (payload: IAssignmentCreateUpdateRequest) => {
        return assignmentApi.createAssignment(payload);
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['assignments'] });
        toast.success('Assignment created successfully');
        navigate(path.assignment);
      },
      onError: (err: any) => {
        const errMsg = err?.response?.data?.errors?.[0];
        toast.error(errMsg || 'Error creating assignment');
      },
    });
  }

  /**
   * Update an assignment
   * @returns {UseMutationResult} The mutation result
   * @description Update an assignment via the API
   */
  function useUpdateAssignment() {
    return useMutation({
      mutationFn: ({ id, payload }: { id: string; payload: IAssignmentCreateUpdateRequest }) => {
        return assignmentApi.updateAssignment(id, payload);
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['assignments'] });
        toast.success('Assignment updated successfully');
        navigate(path.assignment);
      },
      onError: (err: any) => {
        const errMsg = err?.response?.data?.errors?.[0];
        toast.error(errMsg || 'Error updating assignment');
      },
    });
  }

  return {
    useCreateAssignment,
    useUpdateAssignment,
  };
}

export default useAssignment;
