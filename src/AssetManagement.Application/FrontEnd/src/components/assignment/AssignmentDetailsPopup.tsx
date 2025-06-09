import { IAssignment } from "@/types/assignment.type";
import { useQuery } from "@tanstack/react-query";
import assignmentApi from "@/apis/assigment.api.ts";

interface AssignmentDetailsPopupProps {
  assignment: IAssignment;
  isOpen: boolean;
  onClose: () => void;
}

export default function AssignmentDetailsPopup({
  assignment,
  isOpen,
  onClose,
}: AssignmentDetailsPopupProps) {
  const { data: fetchedAssignment, isLoading } = useQuery({
    queryKey: ["assigment", assignment.id],
    queryFn: async () => {
      if (!assignment.id) throw new Error("Assignment Id is required");

      const response = await assignmentApi.getAssigmentDetails(assignment.id);

      if (response.success && response.data) {
        return response.data;
      }

      throw new Error(response.message || "Failed to fetch assigment");
    },
  });

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-opacity-5 border-black-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-md shadow-lg w-full max-w-[40rem] relative">
        {/* Header */}
        <div className="flex justify-between items-center p-4 border bg-tertiary rounded-t-md border-black text-white">
          <h2 className="text-xl font-semibold text-primary">
            Detailed Assignment Information
          </h2>
          <button
            onClick={onClose}
            className="text-primary hover:text-dark-200 focus:outline-none border-red rounded-md border-3 cursor-pointer"
            aria-label="Close"
          >
            <svg
              className="w-6 h-6"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <path
                d="M18 6L6 18"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M6 6L18 18"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </button>
        </div>

        {/* Content */}
        {isLoading || !fetchedAssignment ? (
          <div>Loading...</div>
        ) : (
          <div className="p-6 border-1 border-black rounded-b-md">
            <div
              style={{ gridTemplateColumns: "1fr 4fr" }}
              className="grid gap-4"
            >
              <div className="text-gray-600">Asset Code</div>
              <div>{fetchedAssignment.assetCode}</div>

              <div className="text-gray-600">Asset Name</div>
              <div>{fetchedAssignment.assetName}</div>

              <div className="text-gray-600">Specification</div>
              <div>Core i5, 8GB RAM, 750 GB HDD, Windows 8</div>

              <div className="text-gray-600">Assigned to</div>
              <div>{fetchedAssignment.assignedTo}</div>

              <div className="text-gray-600">Assigned by</div>
              <div>{fetchedAssignment.assignedBy}</div>

              <div className="text-gray-600">Assigned Date</div>
              <div>{fetchedAssignment.assignedDate}</div>

              <div className="text-gray-600">State</div>
              <div>{fetchedAssignment.state}</div>

              <div className="text-gray-600">Note</div>
              <div>{fetchedAssignment.note || "Empty"}</div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
