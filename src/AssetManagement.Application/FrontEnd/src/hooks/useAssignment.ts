import assignmentApi from "@/apis/assingment.api";
import path from "@/constants/path";
import { IAssignmentCreateUpdateRequest } from "@/types/assingment.type";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createSearchParams, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import useAssignmentQuery from "@/hooks/useAssignmentQuery.ts";
import { IMyAssignmentParams } from "@/types/assignment.type";

export function useAssignment() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const queryConfig = useAssignmentQuery();

  /**
   * Create a new assignment
   * @returns {UseMutationResult} The mutation result
   * @description Create a new assignment via the API
   * @note Newly created assignments will automatically have "Waiting for acceptance" state
   */
  function useCreateAssignment() {
    return useMutation({
      mutationFn: (payload: IAssignmentCreateUpdateRequest) => {
        return assignmentApi.createAssignment(payload);
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ["assignments"] });
        toast.success("Assignment created successfully");
        navigate({
          pathname: path.assignment,
          search: createSearchParams({
            ...queryConfig,
            sortBy: "created:desc",
            pageNumber: "1",
          }).toString(),
        });
      },
      onError: (err: any) => {
        const errMsg = err?.response?.data?.errors?.[0];
        toast.error(errMsg || "Error creating assignment");
      },
    });
  }
  /**
   * Update an assignment
   * @returns {UseMutationResult} The mutation result
   * @description Update an assignment via the API
   * @note Only assignments with "Waiting for acceptance" state can be edited
   */
  function useUpdateAssignment() {
    return useMutation({
      mutationFn: ({
        id,
        payload,
      }: {
        id: string;
        payload: IAssignmentCreateUpdateRequest;
      }) => {
        return assignmentApi.updateAssignment(id, payload);
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ["assignments"] });
        toast.success("Assignment updated successfully");
        navigate({
          pathname: path.assignment,
          search: createSearchParams({
            ...queryConfig,
            sortBy: "updated:desc",
            pageNumber: "1",
          }).toString(),
        });
      },
      onError: (err: any) => {
        const errMsg = err?.response?.data?.errors?.[0];
        toast.error(errMsg || "Error updating assignment");
      },
    });
  }
  function useGetAssignmentDetails(id: string) {
    return useQuery({
      queryKey: ["assignment", id],
      queryFn: async () => {
        if (!id) throw new Error("Assignment ID is required");
        const response = await assignmentApi.getAssignmentDetails(id);
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(
          response.message || "Failed to fetch assignment details",
        );
      },
      enabled: !!id,
      staleTime: 0, // Always fetch fresh data
      refetchOnWindowFocus: true,
      refetchOnMount: true,
    });
  }

  function useDeleteAssignment() {
    return useMutation({
      mutationFn: (id: string) => {
        return assignmentApi.deleteAssignment(id);
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['assignments'] });
        toast.success('Assignment deleted successfully');
      },
      onError: (err: any) => {
        const errMsg = err?.response?.data?.errors?.[0];
        toast.error(errMsg || 'Error deleting assignment');
      },
    });
  }

  function useReplyAssignment() {
    return useMutation({
      mutationFn: async ({
        id,
        reply,
      }: {
        id: string;
        reply: 'Accept' | 'Decline';
      }) => {
        if (reply == 'Accept') {
          return assignmentApi.acceptAssignment(id);
        } else if (reply == 'Decline') {
          return assignmentApi.declineAssignment(id);
        }
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['assignments'] });
        toast.success('Assignment accepted successfully');
      },
      onError: (err: any) => {
        const errMsg = err?.response?.data?.errors?.[0];
        toast.error(errMsg || 'Error accepting assignment');
      },
    });
  }

  function useGetMyAssignments(params: IMyAssignmentParams) {
    return useQuery({
      queryKey: ['my-assignments', params],
      queryFn: () => assignmentApi.getMyAssignments(params),
    })
  }

  return {
    useCreateAssignment,
    useUpdateAssignment,
    useDeleteAssignment,
    useReplyAssignment,
    useGetMyAssignments,
    useGetAssignmentDetails
  };
}

export default useAssignment;
