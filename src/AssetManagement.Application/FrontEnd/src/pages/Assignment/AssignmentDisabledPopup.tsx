import useAssignment from "@/hooks/useAssignment";
import React from "react";

interface AssignmentDisabledPopupProps {
  isOpen: boolean;
  onClose: () => void;
  assignmentId: string;
}

const AssignmentDisabledPopup: React.FC<AssignmentDisabledPopupProps> = ({
  isOpen,
  onClose,
  assignmentId,
}) => {
  const { useDeleteAssignment } = useAssignment();
  const deleteAssignment = useDeleteAssignment();

  const onConfirm = async () => {
    await deleteAssignment.mutateAsync(assignmentId);
    onClose();
  };

  if (!isOpen) return null;

  return (
    <>
      <div
        className="absolute inset-0 blur-sm bg-white/50 cursor-not-allowed"
        onClick={(e) => e.stopPropagation()}
      />

      <div
        className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 z-50 bg-white border border-black shadow-xl rounded-md"
        style={{
          width: "500px",
          boxShadow: "0 2px 8px rgba(0, 0, 0, 0.15)",
        }}
      >
        {/* Header section with darker grey background */}
        <div className="py-3 px-10 border-b border-gray-300 bg-gray-300 rounded-t-md">
          <h2 className="text-xl font-semibold text-primary">Are you sure?</h2>
        </div>

        {/* Message section */}
        <div className="px-10 py-5">
          <p className="mb-6">Do you want to delete this assignment?</p>
          <div className="flex gap-4">
            <button
              onClick={onConfirm}
              className={`py-2 px-4 rounded focus:outline-none bg-primary text-white hover:bg-primary/90`}
            >
              Delete
            </button>
            <button
              onClick={onClose}
              className="border border-gray-300 py-2 px-4 rounded hover:bg-gray-100 focus:outline-none text-gray-400"
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    </>
  );
};

export default AssignmentDisabledPopup;
