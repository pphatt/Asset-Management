import { IAssignment } from "@/types/assignment.type";
import { format } from "date-fns";

interface AssignmentDetailsPopupProps {
  assignment: IAssignment | null;
  isOpen: boolean;
  onClose: () => void;
}

export default function AssignmentDetailsPopup({
  assignment,
  isOpen,
  onClose,
}: AssignmentDetailsPopupProps) {
  if (!isOpen || !assignment) return null;

  // Format date if it exists
  const formattedDate = assignment.assignedDate
    ? format(new Date(assignment.assignedDate), "dd/MM/yyyy")
    : "";

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
            className="text-primary hover:text-dark-200 focus:outline-none border-red rounded-md border-3"
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
        <div className="p-6 border-1 border-black rounded-b-md">
          <div
            style={{ gridTemplateColumns: "1fr 4fr" }}
            className="grid gap-4"
          >
            <div className="text-gray-600">Asset Code</div>
            <div>{assignment.assetCode}</div>

            <div className="text-gray-600">Asset Name</div>
            <div>{assignment.assetName}</div>

            <div className="text-gray-600">Specification</div>
            <div>Core i5, 8GB RAM, 750 GB HDD, Windows 8</div>

            <div className="text-gray-600">Assigned to</div>
            <div>{assignment.assignedTo}</div>

            <div className="text-gray-600">Assigned by</div>
            <div>{assignment.assignedBy}</div>

            <div className="text-gray-600">Assigned Date</div>
            <div>{formattedDate}</div>

            <div className="text-gray-600">State</div>
            <div>{assignment.state}</div>

            <div className="text-gray-600">Note</div>
            <div>{assignment.note || "Empty"}</div>
          </div>
        </div>
      </div>
    </div>
  );
}
