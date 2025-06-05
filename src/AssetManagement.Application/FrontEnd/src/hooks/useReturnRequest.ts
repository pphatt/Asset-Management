import returnRequestApi from '@/apis/returnRequest.api';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

export function useReturnRequest() {
  const queryClient = useQueryClient();

  function useCreateReturnRequest() {
    return useMutation({
      mutationFn: async (assignmentId: string) => returnRequestApi.createReturnRequest(assignmentId),
      onSuccess: (response) => {
        queryClient.invalidateQueries({ queryKey: ['assignments'] });

        queryClient.invalidateQueries({
          queryKey: ['my-assignments'],
          exact: false,
        });

        toast.success(response?.message ?? 'Return request created successfully');
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] ?? 'Error creating return request');
      },
    });
  }

  return {
    useCreateReturnRequest,
  };
}

export default useReturnRequest;
