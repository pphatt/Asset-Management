import React, { useContext } from "react";
import { IUser } from "../../types/user.type";
import { AppContext } from "@/contexts/app.context";
import { X } from "lucide-react";

interface DisableUserPopupProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  targetUser: IUser;
}

const DisableUserPopup: React.FC<DisableUserPopupProps> = ({
  isOpen,
  onClose,
  onConfirm,
  targetUser,
}) => {
  const { profile } = useContext(AppContext);
  // Current logged in users cannot disable themselves
  const isSelfDisable = profile?.staffCode === targetUser?.staffCode;

  if (!isOpen) return null;

  if (targetUser.hasAssignments) {
    return (
      <div className="fixed inset-0 z-50">
        {/* Overlay to prevent interaction */}
        <div
          className="absolute inset-0 backdrop-blur-sm bg-white/50 cursor-not-allowed"
          onClick={(e) => {
            e.stopPropagation();
            onClose();
          }}
        />

        {/* Popup */}
        <div
          className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-white border border-black shadow-xl rounded-md"
          style={{
            width: "500px",
            boxShadow: "0 2px 8px rgba(0, 0, 0, 0.15)",
          }}
        >
          {/* Header section with darker grey background */}
          <div className="flex justify-between items-center py-3 px-10 border-b border-gray-300 bg-gray-300 rounded-t-md">
            <h2 className="text-xl font-semibold text-primary">
              Can not disable user
            </h2>

            <button
              onClick={onClose}
              className="text-primary hover:text-dark-200 focus:outline-none border-red rounded-md border-3 cursor-pointer"
              aria-label="Close"
            >
              <X size={24} />
            </button>
          </div>

          {/* Message section */}
          <div className="px-10 py-5">
            <p className="mb-6">
              There are valid assignments belonging to this user.
            </p>

            <p className="mb-6">
              Please close all assignments before disabling user.
            </p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 z-50">
      {/* Overlay to prevent interaction */}
      <div
        className="absolute inset-0 backdrop-blur-sm bg-white/50 cursor-not-allowed"
        onClick={(e) => e.stopPropagation()}
      />

      {/* Popup */}
      <div
        className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-white border border-black shadow-xl rounded-md"
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
          <p className="mb-6">Do you want to disable this user?</p>
          <div className="flex gap-4">
            <button
              onClick={onConfirm}
              disabled={isSelfDisable}
              className={`py-2 px-4 rounded focus:outline-none ${
                isSelfDisable
                  ? "bg-gray-400 text-gray-300 cursor-not-allowed"
                  : "bg-primary text-white hover:bg-primary/90"
              }`}
            >
              Disable
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
    </div>
  );
};

export default DisableUserPopup;
