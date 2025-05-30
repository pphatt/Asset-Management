import useAssignment from '@/hooks/useAssignment';
import { IAssginmentDetail, IAssignmentCreateUpdateRequest } from '@/types/assingment.type';
import React from 'react';
import { formatDateToString } from '../../utils/formatDate';
import AssignmentForm from './AssignmentForm';

const CreateAssignment: React.FC = () => {
  const { useUpdateAssignment } = useAssignment();
  const { mutate: updateAssignmentMutation, isPending: isSubmitting } = useUpdateAssignment();

  const mockAssingmentDetail: IAssginmentDetail = {
    id: '1be63ec6-fdaf-4c23-3404-08dd9eddf7de',
    no: '12345',
    assetCode: 'A001',
    assetName: 'Laptop',
    assignedTo: 'John Doe',
    assignedBy: 'Jane Smith',
    assignedDate: '2023-10-01',
    state: 'Accepted',
    note: 'Handle with care',
  };

  const mockInitialData: IAssignmentCreateUpdateRequest = {
    assetId: mockAssingmentDetail.assetCode,
    assigneeId: mockAssingmentDetail.assignedTo,
    assignedDate: formatDateToString(new Date(mockAssingmentDetail.assignedDate)),
    note: mockAssingmentDetail.note || '',
  };

  const handleSubmit = (data: IAssignmentCreateUpdateRequest) => {
    updateAssignmentMutation({ id: mockAssingmentDetail.id, payload: data });
  };

  return (
    <div className="container mx-auto p-4">
      <AssignmentForm mode="edit" initialData={mockInitialData} onSubmitForm={handleSubmit} isExternalSubmitting={isSubmitting} />
    </div>
  );
};

export default CreateAssignment;
