import React from 'react';

interface ReturnRequestPopupProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  isLoading?: boolean;
}

const ReturnRequestPopup: React.FC<ReturnRequestPopupProps> = ({ isOpen, onClose, onConfirm, isLoading = false }) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-transparent bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-md shadow-lg w-full max-w-[30rem] relative">
        {/* Header */}
        <div className="flex justify-between items-center p-4 border bg-tertiary rounded-t-md border-black text-white">
          <h2 className="text-xl font-semibold text-primary">Are you sure?</h2>
          <button
            onClick={onClose}
            disabled={isLoading}
            className={`text-primary hover:text-dark-200 focus:outline-none border-red rounded-md border-3 ${
              isLoading ? 'cursor-not-allowed' : 'cursor-pointer'
            }`}
            aria-label="Close"
          >
            <svg className="w-6 h-6" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M18 6L6 18" strokeLinecap="round" strokeLinejoin="round" />
              <path d="M6 6L18 18" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </button>
        </div>

        {/* Content */}
        <div className="p-6 border-1 border-black rounded-b-md">
          <p className="text-lg mb-6">Do you want to create a returning request for this asset?</p>

          <div className="flex justify-end gap-4">
            <button
              onClick={onConfirm}
              disabled={isLoading}
              className={`px-4 py-2 bg-primary text-white rounded focus:outline-none hover:bg-red-800 transition ${
                isLoading ? 'opacity-70 cursor-not-allowed' : 'hover:bg-primary-dark cursor-pointer'
              }`}
            >
              {isLoading ? 'Processing...' : 'Yes'}
            </button>
            <button
              onClick={onClose}
              disabled={isLoading}
              className={`px-4 py-2 bg-gray-200 text-gray-800 rounded focus:outline-none hover:bg-gray-400 transition ${
                isLoading ? 'opacity-70 cursor-not-allowed' : 'hover:bg-gray-300 cursor-pointer'
              }`}
            >
              No
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ReturnRequestPopup;
