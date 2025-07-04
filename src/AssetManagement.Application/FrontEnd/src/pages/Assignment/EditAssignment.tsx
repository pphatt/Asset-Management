﻿import path from '@/constants/path';
import useAssignment from '@/hooks/useAssignment';
import { IAssignmentCreateUpdateRequest } from '@/types/assingment.type';
import { parseAndFormatDate } from '@/utils/formatDate';
import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import AssignmentForm from './AssignmentForm';

const EditAssignment: React.FC = () => {
  const { assignmentId } = useParams<{ assignmentId: string }>();
  const navigate = useNavigate();
  const [initialData, setInitialData] = useState<IAssignmentCreateUpdateRequest | undefined>(undefined);

  const [selectedAssetInfo, setSelectedAssetInfo] = useState<{ id: string; name: string; code: string } | undefined>(undefined);
  const [selectedUserInfo, setSelectedUserInfo] = useState<{ id: string; username: string; staffCode: string } | undefined>(undefined);

  const { useUpdateAssignment, useGetAssignmentDetails } = useAssignment();
  const { mutate: updateAssignmentMutation, isPending: isSubmitting } = useUpdateAssignment();
  const { data: assignmentData, isLoading: isLoadingAssignment, error, refetch } = useGetAssignmentDetails(assignmentId || '');

  useEffect(() => {
    if (assignmentId) {
      refetch();
    }
  }, [assignmentId, refetch]);

  useEffect(() => {
    if (assignmentData) {
      try {
        if (assignmentData.state !== 'Waiting for acceptance') {
          toast.error("This assignment cannot be edited because it's not in 'Waiting for acceptance' state");
          navigate(path.assignment);
          return;
        }

        const formattedDate = parseAndFormatDate(assignmentData.assignedDate);

        setInitialData({
          assetId: assignmentData.assetId,
          assigneeId: assignmentData.assigneeId,
          assignedDate: formattedDate,
          note: assignmentData.note || '',
        });

        setSelectedAssetInfo({
          id: assignmentData.assetId,
          name: assignmentData.assetName,
          code: assignmentData.assetCode,
        });

        // Set selected user info for pre-selection in dropdown
        setSelectedUserInfo({
          id: assignmentData.assigneeId,
          username: assignmentData.assignedTo,
          staffCode: assignmentData.assigneeStaffCode,
        });
      } catch (error) {
        console.error('Error formatting assignment data:', error);
        toast.error('Error processing assignment data');
      }
    }
  }, [assignmentData]);

  useEffect(() => {
    if (error) {
      toast.error('Failed to load assignment details');
      navigate(path.assignment);
    }
  }, [error, navigate]);

  const handleSubmit = (data: IAssignmentCreateUpdateRequest) => {
    if (!assignmentId) {
      toast.error('Assignment ID is missing');
      return;
    }
    updateAssignmentMutation({ id: assignmentId, payload: data });
  };

  if (isLoadingAssignment || !initialData) {
    return (
      <div className="container mx-auto p-4">
        <div className="flex justify-center items-center h-40">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
          <p className="ml-2">Loading assignment details...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-4">
      <AssignmentForm
        mode="edit"
        initialData={initialData}
        onSubmitForm={handleSubmit}
        isExternalSubmitting={isSubmitting}
        selectedAssetInfo={selectedAssetInfo}
        selectedUserInfo={selectedUserInfo}
      />
    </div>
  );
};

export default EditAssignment;
