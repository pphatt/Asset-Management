import useAssignment from '@/hooks/useAssignment';
import { IMyAssignment } from '@/types/assignment.type';
import { Check } from 'lucide-react';
import { useState } from 'react';
import useReturnRequest from '../../hooks/useReturnRequest';
import Pagination from '../common/Pagination';
import ReplyAssignmentPopup from './ReplyAssignmentPopup';
import ReturnRequestPopup from './ReturnRequestPopup';
import AssignmentDetailsPopup from './AssignmentDetailsPopup';

export const AssignmentTable: React.FC = () => {
  const columns = [
    { key: 'assetCode', label: 'Asset Code', sortable: true },
    { key: 'assetName', label: 'Asset Name', sortable: true },
    { key: 'category', label: 'Category', sortable: true },
    { key: 'asignedDate', label: 'Assigned Date', sortable: true },
    { key: 'state', label: 'State', sortable: true },
  ];
  const [sortBy, setSortBy] = useState<string>('');
  const [direction, setDirection] = useState<'asc' | 'desc' | ''>('');
  const [isDisablePopupOpen, setIsDisablePopupOpen] = useState(false);
  const [returnRequestState, setReturnRequestState] = useState({
    isPopupOpen: false,
    assignmentId: '',
  });
  const [reply, setReply] = useState<'Accept' | 'Decline'>('Accept');
  const [selectedMyAssignment, setSelectedMyAssignment] = useState<IMyAssignment | null>(null);
  const { useGetMyAssignments, useGetAssignmentDetails } = useAssignment();
  const { useCreateReturnRequest } = useReturnRequest();
  const { data: assignmentDetail } =
    useGetAssignmentDetails(selectedMyAssignment?.assignmentId || '');
  const { mutate: createReturnRequest, isPending: isCreatingReturnRequest } = useCreateReturnRequest();
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const handleClosePopup = () => {
    setIsPopupOpen(false);
    setSelectedMyAssignment(null);
  };

  const [page, setPage] = useState(1);
  const { data: myAssignments, isPending: isLoading } = useGetMyAssignments({
    pageNumber: page,
    pageSize: 5,
    sortBy: sortBy || undefined,
  });

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handleSort = (key: string) => {
    let newDirection = direction;
    switch (direction) {
      case 'asc':
      case '':
        newDirection = 'desc';
        setDirection('desc');
        break;
      case 'desc':
        newDirection = 'asc';
        setDirection('asc');
        break;
    }

    const newSortBy = `${key}:${newDirection}`;
    setSortBy(newSortBy);
  };

  const handleReplyAssignment = (assignment: IMyAssignment, reply: 'Accept' | 'Decline', isOpen: boolean) => {
    setSelectedMyAssignment(assignment);
    setReply(reply);
    setIsDisablePopupOpen(isOpen);
  };

  const handleReturnRequest = (assignmentId: string) => {
    setReturnRequestState({
      isPopupOpen: true,
      assignmentId,
    });
  };

  const confirmReturnRequest = () => {
    createReturnRequest(returnRequestState.assignmentId);
    closeReturnRequestPopup();
  };

  const closeReturnRequestPopup = () => {
    setReturnRequestState((prev) => ({
      ...prev,
      isPopupOpen: false,
    }));
  };

  return (
    <>
      <table className="w-full text-sm border-collapse border-spacing-0">
        <thead>
          <tr className="text-quaternary text-sm font-semibold">
            {columns.map((col) => (
              <th
                key={col.key}
                className={`text-left relative py-2 after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[2px] ${sortBy?.startsWith(`${col.key}:`) ? 'after:bg-gray-600 font-semibold' : 'after:bg-gray-400 font-medium'
                  } ${col.sortable ? 'cursor-pointer select-none' : 'cursor-default select-none'}`}
                onClick={col.sortable ? () => handleSort(col.key) : undefined}
              >
                {col.label}
                {col.sortable && (
                  <svg
                    className={`inline-block ml-1 w-3 h-3 ${sortBy?.startsWith(`${col.key}:`) ? 'text-primary' : ''}`}
                    viewBox="0 0 24 24"
                    fill="none"
                  >
                    {sortBy?.startsWith(`${col.key}:`) && sortBy?.endsWith(':desc') ? (
                      <path d="M18 15L12 9L6 15" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    ) : (
                      <path d="M6 9L12 15L18 9" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    )}
                  </svg>
                )}
              </th>
            ))}

            <th className="text-center relative w-16 select-none">
              <span className="sr-only">Actions</span>
            </th>
          </tr>
        </thead>
        <tbody>
          {myAssignments && myAssignments.items?.length > 0 ? (
            myAssignments?.items.map((assignment) => (
              <tr key={assignment.assignmentId}
                className="hover:bg-gray-50 cursor-pointer"
                onClick={() => {
                  setSelectedMyAssignment(assignment);
                  setIsPopupOpen(true);
                }}
              >
                <td className="py-2 relative w-[100px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300 select-text">
                  {assignment.assetCode}
                </td>
                <td className="py-2 relative w-[180px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300 select-text">
                  {assignment.assetName}
                </td>
                <td className="py-2 relative w-[120px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300 select-text">
                  {assignment.category}
                </td>
                <td className="py-2 relative w-[80px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300 select-text">
                  {assignment.assignedDate}
                </td>
                <td className="py-2 relative w-[80px] after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300 select-text">
                  {assignment.state}
                </td>
                <td className="py-2 relative">
                  <div className="flex items-center justify-center space-x-4">
                    <button
                      className={`text-quaternary hover:text-gray-700 ${assignment.state !== 'Waiting for acceptance' ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'
                        }`}
                      onClick={(e) => {
                        e.stopPropagation();
                        handleReplyAssignment(assignment, 'Accept', true);
                      }}
                      disabled={assignment.state !== 'Waiting for acceptance'}
                      tabIndex={assignment.state !== 'Waiting for acceptance' ? -1 : 0}
                    >
                      <Check className="size-4" color="red" />
                    </button>
                    <button
                      className={`text-primary hover:text-red-700 ${assignment.state !== 'Waiting for acceptance' ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'
                        }`}
                      onClick={(e) => {
                        e.stopPropagation();
                        handleReplyAssignment(assignment, 'Decline', true);
                      }}
                      disabled={assignment.state !== 'Waiting for acceptance'}
                      tabIndex={assignment.state !== 'Waiting for acceptance' ? -1 : 0}
                    >
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" color="black">
                        <path d="M18 6L6 18" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                        <path d="M6 6L18 18" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                      </svg>
                    </button>
                    <button
                      className={`text-blue-600 hover:text-blue-800 ${assignment.state === 'Accepted' ? 'cursor-pointer' : 'cursor-not-allowed opacity-50'
                        }`}
                      onClick={(e) => {
                        e.stopPropagation();
                        if (assignment.state === 'Accepted' && assignment.assignmentId) {
                          handleReturnRequest(assignment.assignmentId);
                        }
                      }}
                      disabled={assignment.state !== 'Accepted' || isCreatingReturnRequest}
                      tabIndex={assignment.state !== 'Accepted' ? -1 : 0}
                    >
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                        strokeWidth="1.5"
                        stroke="currentColor"
                        className="size-4"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0 3.181 3.183a8.25 8.25 0 0 0 13.803-3.7M4.031 9.865a8.25 8.25 0 0 1 13.803-3.7l3.181 3.182m0-4.991v4.99"
                        />
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan={6} className="py-4 text-center text-gray-500">
                No assets found. Try changing your search criteria.
              </td>
            </tr>
          )}
        </tbody>
      </table>

      {/* Pagination */}
      {!isLoading && myAssignments && myAssignments.paginationMetadata && (
        <Pagination
          currentPage={myAssignments.paginationMetadata.currentPage}
          totalPages={myAssignments.paginationMetadata.totalPages}
          hasNextPage={myAssignments.paginationMetadata.hasNextPage}
          hasPreviousPage={myAssignments.paginationMetadata.hasPreviousPage}
          onPageChange={handlePageChange}
          isLoading={isLoading}
        />
      )}

      {/* Assignment Details Popup */}
      {assignmentDetail && (
        <AssignmentDetailsPopup
          assignment={assignmentDetail}
          isOpen={isPopupOpen}
          onClose={handleClosePopup}
        />
      )}

      <ReplyAssignmentPopup
        isOpen={isDisablePopupOpen}
        onClose={() => setIsDisablePopupOpen(false)}
        assignmentId={selectedMyAssignment?.assignmentId || ''}
        reply={reply}
      />
      <ReturnRequestPopup
        isOpen={returnRequestState.isPopupOpen}
        onClose={closeReturnRequestPopup}
        onConfirm={confirmReturnRequest}
        isLoading={isCreatingReturnRequest}
      />
    </>
  );
};
