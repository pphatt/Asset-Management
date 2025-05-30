import useAssignment from '@/hooks/useAssignment';
import { IAssignmentCreateUpdateRequest } from '@/types/assingment.type';
import React from 'react';
import AssignmentForm from './AssignmentForm';

const CreateAssignment: React.FC = () => {
  const { useCreateAssignment } = useAssignment();
  const { mutate: createAssignmentMutation, isPending: isSubmitting } = useCreateAssignment();

  const handleSubmit = (data: IAssignmentCreateUpdateRequest) => {
    createAssignmentMutation(data);
  };

  return (
    <div className="container mx-auto p-4">
      <AssignmentForm mode="create" onSubmitForm={handleSubmit} isExternalSubmitting={isSubmitting} />
    </div>
  );
};

export default CreateAssignment;
