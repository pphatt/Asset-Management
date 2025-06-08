import returnRequestApi from '@/apis/returnRequest.api';
import { IReturnRequestParams } from '@/types/returnRequest.type';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';
import useAssignmentQuery from './useAssignmentQuery';

export function useReturnRequest() {
  const queryClient = useQueryClient();
  const queryConfig = useAssignmentQuery();

  function useCreateReturnRequest() {
    return useMutation({
      mutationFn: async (assignmentId: string) => returnRequestApi.createReturnRequest(assignmentId),
      onSuccess: (response) => {
        queryClient.invalidateQueries({ queryKey: ['assignments'] });

        queryClient.invalidateQueries({
          queryKey: ['my-assignments'],
          exact: false,
        });

        queryClient.invalidateQueries({
          queryKey: ['returnRequests', queryConfig]
        });

        toast.success(response?.message ?? 'Return request created successfully');
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] ?? 'Error creating return request');
      },
    });
  }

  function useCancelReturnRequest() {
    return useMutation({
      mutationFn: async (returnRequestId: string) => returnRequestApi.cancelReturnRequest(returnRequestId),
      onSuccess: (response) => {
        queryClient.invalidateQueries({ queryKey: ['returnRequests', queryConfig], exact: true });
        toast.success(response?.message ?? 'Return request cancelled successfully');
      },
      onError: (err: any) => {
        queryClient.invalidateQueries({ queryKey: ['returnRequests', queryConfig], exact: true });
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] ?? 'Error cancelling return request');
      },
    });
  }

  function useAcceptReturnRequest() {
    return useMutation({
      mutationFn: async (id: string) => returnRequestApi.acceptReturnRequest(id),
      onSuccess: (response) => {
        queryClient.invalidateQueries({ queryKey: ['returnRequests', queryConfig], exact: true });
        toast.success(response?.message ?? 'Return request accepted successfully');
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] ?? 'Error accepting return request');
      },
    });
  }

  function useGetReturnRequest(params: IReturnRequestParams) {
    return useQuery({
      queryKey: ['returnRequests', params],
      queryFn: () => returnRequestApi.getRequests(params),
    });
  }

  return {
    useCreateReturnRequest,
    useCancelReturnRequest,
    useGetReturnRequest,
    useAcceptReturnRequest
  };
}

export default useReturnRequest;
