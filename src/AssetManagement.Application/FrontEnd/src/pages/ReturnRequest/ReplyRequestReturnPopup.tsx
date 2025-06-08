import React from 'react';
import { useReturnRequest } from '@/hooks/useReturnRequest';

interface CancelRequestReturnPopupProps {
    isOpen: boolean;
    onClose: () => void;
    returnRequestId: string;
    type: 'cancel' | 'accept';
}

const ReplyReturnRequestPopup: React.FC<CancelRequestReturnPopupProps> = ({
    isOpen,
    onClose,
    returnRequestId,
    type
}) => {
    const message = type === 'cancel'
        ? 'Do you want to cancel this returning request?'
        : "Do you want to mark this returning request as 'Completed'?";
    const { useCancelReturnRequest, useAcceptReturnRequest } = useReturnRequest();
    const { mutate: cancelReturnRequest, isPending: isCancelLoading } = useCancelReturnRequest();
    const { mutate: acceptReturnRequest, isPending: isAcceptLoading } = useAcceptReturnRequest();

    const isPending = isCancelLoading || isAcceptLoading;

    const onConfirm = async () => {
        if (type === 'accept') {
            acceptReturnRequest(returnRequestId, {
                onSuccess: () => {
                    onClose();
                },
                onError: () => {
                    onClose();
                },
            });
            return;
        } else {
            cancelReturnRequest(returnRequestId, {
                onSuccess: () => {
                    onClose();
                },
                onError: () => {
                    onClose();
                },
            });
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50">
            <div
                className="absolute inset-0 backdrop-blur-sm bg-white/50 cursor-not-allowed"
                onClick={(e) => e.stopPropagation()}
            />

            <div
                className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-white border border-black shadow-xl rounded-md"
                style={{
                    width: '500px',
                    boxShadow: '0 2px 8px rgba(0, 0, 0, 0.15)',
                }}
            >
                {/* Header section with darker grey background */}
                <div className="py-3 px-8 border-b border-gray-300 bg-gray-300 rounded-t-md">
                    <h2 className="text-xl font-semibold text-primary">Are you sure?</h2>
                </div>

                {/* Message section */}
                <div className="px-8 py-5">
                    <p className="mb-6">{message}</p>
                    <div className="flex gap-4">
                        <button
                            onClick={() => {
                                onConfirm();
                            }}
                            className={`py-2 px-4 rounded focus:outline-none bg-primary text-white hover:bg-primary/90`}
                            disabled={isPending}
                        >
                            {isPending ? 'Processing...' : 'Yes'}
                        </button>
                        <button
                            onClick={onClose}
                            className="border border-gray-300 py-2 px-4 rounded hover:bg-gray-100 focus:outline-none text-gray-400"
                            disabled={isPending}
                        >
                            No
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ReplyReturnRequestPopup;
