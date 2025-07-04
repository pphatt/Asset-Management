import returnRequestApi from '@/apis/returnRequest.api';
import Pagination from '@/components/assignment/Pagination';
import TableSkeleton from '@/components/common/TableSkeleton';
import { ASSIGNMENT_STATE } from '@/constants/assignment-params';
import path from '@/constants/path';
import useAssignmentDateFilter from '@/hooks/useAssignmentDateFilter';
import useAssignmentQuery from '@/hooks/useAssignmentQuery';
import useAssignmentSearch from '@/hooks/useAssignmentSearch';
import useAssignmentStateFilter from '@/hooks/useAssignmentStateFilter';
import '@/styles/datepicker.css'; // Import custom styles
import { IReturnRequest, IReturnRequestParams } from '@/types/returnRequest.type';
import { isReturnRequestReturn } from '@/utils/enumConvert';
import { useQuery } from '@tanstack/react-query';
import { isSameDay } from 'date-fns';
import { useEffect, useRef, useState } from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { createSearchParams, useNavigate } from 'react-router-dom';
import ReplyReturnRequestPopup from './ReplyRequestReturnPopup';
import { Check } from 'lucide-react';

export type QueryConfig = {
    [key in keyof IReturnRequestParams]: string;
};

export default function ReturnRequest() {
    const [isStateDropdownOpen, setIsStateDropdownOpen] = useState(false);
    const [selectedReturnRequest, setSelectedReturnRequest] =
        useState<IReturnRequest | null>(null);
    const [isReplyReturnRequestPopupOpen, setIsReplyReturnRequestPopupOpen] = useState(false);
    const [type, setType] = useState<'cancel' | 'accept'>('cancel');
    const queryConfig = useAssignmentQuery();
    const navigate = useNavigate();
    const { onSubmitSearch, register: searchRegister } = useAssignmentSearch({
        path: path.request,
    });

    const stateOptions = [
        { value: 'All', label: 'All' },
        { value: 'Completed', label: 'Completed' },
        {
            value: ASSIGNMENT_STATE.WAITING_FOR_RETURNING,
            label: 'Waiting for returning',
        },
    ];

    const { handleStateChange, isStateSelected, selectedStates } = useAssignmentStateFilter({
        path: path.request,
    });

    const {
        selectedDate,
        displayDate,
        handleDateChange,
        isDatePickerOpen,
        setIsDatePickerOpen,
        toggleDatePicker,
    } = useAssignmentDateFilter({
        path: path.request,
        paramName: 'returnedDate',
    });

    const datePickerRef = useRef<HTMLDivElement>(null);
    const stateDropdownRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (datePickerRef.current && !datePickerRef.current.contains(event.target as Node)) {
                setIsDatePickerOpen(false);
            }
        }

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [setIsDatePickerOpen]);

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (
                stateDropdownRef.current &&
                !stateDropdownRef.current.contains(event.target as Node)
            ) {
                setIsStateDropdownOpen(false);
            }
        }

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const { data, isLoading } = useQuery({
        queryKey: ['returnRequests', queryConfig],
        queryFn: () => {
            return returnRequestApi.getRequests(queryConfig as IReturnRequestParams);
        },
        placeholderData: (prevData) => prevData,
        staleTime: 3 * 60 * 1000,
    });

    const returnRequests = data?.data.data.items || [];

    // Parse current sort information from query config
    const parseSortCriteria = (sortString: string | undefined) => {
        if (!sortString) return {};

        // Create an object with column keys and their sort directions
        const sortCriteria: Record<string, string> = {};
        sortString.split(',').forEach((criteria) => {
            const [field, direction] = criteria.split(':');
            if (field && direction) {
                sortCriteria[field] = direction;
            }
        });

        return sortCriteria;
    };

    const sortCriteria = parseSortCriteria(queryConfig.sortBy);

    // Handle column sorting
    const handleSort = (key: string) => {
        // Single column sorting implementation
        let newSortDirection = 'asc';

        // If we're already sorting by this column, toggle the direction
        if (sortCriteria[key]) {
            newSortDirection = sortCriteria[key] === 'asc' ? 'desc' : 'asc';
        }

        // Create the new sort parameter with only this column
        const newSortBy = `${key}:${newSortDirection}`;

        /* Multi-column sorting implementation (commented out)
    // Copy existing sort criteria
    const newSortCriteria = { ...sortCriteria };

    // Toggle or add the clicked column's sort direction
    if (newSortCriteria[key]) {
      // Toggle direction if column already exists
      newSortCriteria[key] = newSortCriteria[key] === "asc" ? "desc" : "asc";
    } else {
      // Add new column with default 'asc' direction
      newSortCriteria[key] = "asc";
    }

    // Convert back to string format: field1:direction1,field2:direction2
    const newSortBy = Object.entries(newSortCriteria)
      .map(([field, direction]) => `${field}:${direction}`)
      .join(",");
    */

        navigate({
            pathname: path.request,
            search: createSearchParams({
                ...queryConfig,
                sortBy: newSortBy,
                pageNumber: '1', // Reset to first page when sorting changes
            }).toString(),
        });
    };

    const columns = [
        { key: 'no', label: 'No.', sortable: true },
        { key: 'assetcode', label: 'Asset Code', sortable: true },
        { key: 'assetname', label: 'Asset Name', sortable: true },
        { key: 'requestedby', label: 'Requested by', sortable: true },
        { key: 'assigneddate', label: 'Assigned Date', sortable: true },
        { key: 'acceptedby', label: 'Accepted by', sortable: true },
        { key: 'returneddate', label: 'Returned Date', sortable: true },
        { key: 'state', label: 'State', sortable: true },
    ];

    if (isLoading) return <TableSkeleton rows={5} columns={10} />;

    // Function to render sort icon based on current sort state
    const renderSortIcon = (columnKey: string) => {
        if (sortCriteria[columnKey]) {
            return sortCriteria[columnKey] === 'asc' ? (
                <svg className="inline-block ml-1 w-3 h-3" viewBox="0 0 24 24" fill="none">
                    <path
                        d="M18 15L12 9L6 15"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                    />
                </svg>
            ) : (
                <svg className="inline-block ml-1 w-3 h-3" viewBox="0 0 24 24" fill="none">
                    <path
                        d="M6 9L12 15L18 9"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                    />
                </svg>
            );
        }

        return (
            <svg className="inline-block ml-1 w-3 h-3 opacity-30" viewBox="0 0 24 24" fill="none">
                <path
                    d="M6 9L12 15L18 9"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                />
            </svg>
        );
    };

    return (
        <div className="w-full max-w-6xl mx-auto px-4 py-6">
            <h2 className="text-primary text-xl font-normal mb-5">Request List</h2>

            <div className="flex justify-between mb-4">
                <div className="flex gap-2">
                    {/* State dropdown (updated for multiple selection) */}
                    <div className="relative" ref={stateDropdownRef}>
                        <div className="flex items-center justify-between w-[240px]">
                            <input
                                type="text"
                                readOnly
                                placeholder="State"
                                value={
                                    selectedStates.length > 0
                                        ? `Selected (${selectedStates.length})`
                                        : 'All'
                                }
                                className="w-full h-[34px] text-sm py-1.5 px-2 border border-quaternary rounded-l bg-white cursor-pointer"
                                onClick={() => setIsStateDropdownOpen(!isStateDropdownOpen)}
                            />
                            <button
                                type="button"
                                onClick={() => setIsStateDropdownOpen(!isStateDropdownOpen)}
                                className="flex items-center justify-center h-[34px] w-[34px] border border-l-0 border-quaternary rounded-r bg-white hover:bg-gray-100 cursor-pointer"
                            >
                                <svg
                                    width="16"
                                    height="16"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                    stroke="black"
                                    strokeWidth="2"
                                >
                                    <path
                                        d="M22 3H2l8 9.46V19l4 2v-8.54L22 3z"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    />
                                </svg>
                            </button>
                        </div>

                        {isStateDropdownOpen && (
                            <div className="absolute top-full left-0 mt-1 w-[240px] bg-white border border-gray-200 rounded shadow-lg z-50">
                                <div className="py-1">
                                    {stateOptions.map((option) => (
                                        <label
                                            key={option.value}
                                            className="flex items-center w-full px-3 py-2 text-sm hover:bg-gray-50 cursor-pointer"
                                            onClick={(e) => {
                                                e.preventDefault(); // Prevent default behavior
                                                handleStateChange(option.value);
                                            }}
                                        >
                                            <input
                                                type="checkbox"
                                                className="mr-2 h-4 w-4 accent-red-600"
                                                checked={isStateSelected(option.value)}
                                                onChange={() => { }} // Empty handler as we're handling it in the label
                                                readOnly
                                            />
                                            <span>{option.label}</span>
                                        </label>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>

                    {/* Date picker */}
                    <div className="relative" ref={datePickerRef}>
                        <div className="flex items-center justify-between w-[240px]">
                            <input
                                type="text"
                                readOnly
                                placeholder="Returned Date"
                                value={displayDate}
                                className="w-full h-[34px] text-sm py-1.5 px-2 border border-quaternary rounded-l bg-white cursor-pointer"
                                onClick={toggleDatePicker}
                            />
                            <button
                                type="button"
                                onClick={toggleDatePicker}
                                className="flex items-center justify-center h-[34px] w-[34px] border border-l-0 border-quaternary rounded-r bg-white hover:bg-gray-100 cursor-pointer"
                            >
                                <svg
                                    width="16"
                                    height="16"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    strokeWidth="2"
                                >
                                    <path
                                        d="M19 4H5C3.89543 4 3 4.89543 3 6V20C3 21.1046 3.89543 22 5 22H19C20.1046 22 21 21.1046 21 20V6C21 4.89543 20.1046 4 19 4Z"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    />
                                    <path
                                        d="M16 2V6"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    />
                                    <path d="M8 2V6" strokeLinecap="round" strokeLinejoin="round" />
                                    <path
                                        d="M3 10H21"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    />
                                </svg>
                            </button>

                            {/* Clear date button */}
                            {displayDate && (
                                <button
                                    type="button"
                                    className="absolute right-12 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        handleDateChange(null);
                                    }}
                                >
                                    <svg
                                        width="12"
                                        height="12"
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
                            )}
                        </div>

                        {isDatePickerOpen && (
                            <div className="absolute z-50 top-full left-0 mt-1">
                                <DatePicker
                                    selected={selectedDate}
                                    onChange={handleDateChange}
                                    inline
                                    monthsShown={1}
                                    showMonthDropdown
                                    showYearDropdown
                                    dropdownMode="select"
                                    formatWeekDay={(nameOfDay) => nameOfDay.substr(0, 1)}
                                    dayClassName={(date) =>
                                        selectedDate && isSameDay(date, selectedDate)
                                            ? 'selected-day'
                                            : ''
                                    }
                                    renderCustomHeader={({
                                        date,
                                        decreaseMonth,
                                        increaseMonth,
                                        prevMonthButtonDisabled,
                                        nextMonthButtonDisabled,
                                    }) => (
                                        <div className="flex items-center justify-between px-4 py-2">
                                            <div className="flex">
                                                <button
                                                    onClick={() => {
                                                        const newDate = new Date(date);
                                                        newDate.setFullYear(date.getFullYear() - 1);
                                                        decreaseMonth();
                                                    }}
                                                    disabled={prevMonthButtonDisabled}
                                                    className="px-2"
                                                >
                                                    {'<<'}
                                                </button>
                                                <button
                                                    onClick={decreaseMonth}
                                                    disabled={prevMonthButtonDisabled}
                                                    className="px-2"
                                                >
                                                    {'<'}
                                                </button>
                                            </div>

                                            <div className="text-lg">
                                                {date.toLocaleString('default', {
                                                    month: 'long',
                                                    year: 'numeric',
                                                })}
                                            </div>

                                            <div className="flex">
                                                <button
                                                    onClick={increaseMonth}
                                                    disabled={nextMonthButtonDisabled}
                                                    className="px-2"
                                                >
                                                    {'>'}
                                                </button>
                                                <button
                                                    onClick={() => {
                                                        const newDate = new Date(date);
                                                        newDate.setFullYear(date.getFullYear() + 1);
                                                        increaseMonth();
                                                    }}
                                                    disabled={nextMonthButtonDisabled}
                                                    className="px-2"
                                                >
                                                    {'>>'}
                                                </button>
                                            </div>
                                        </div>
                                    )}
                                />
                            </div>
                        )}
                    </div>
                </div>

                <div className="flex gap-2 items-center">
                    <form className="relative" onSubmit={onSubmitSearch}>
                        <input
                            type="text"
                            className="border rounded px-3 py-2 min-w-[200px]"
                            placeholder="Search..."
                            {...searchRegister('searchName')}
                        />
                        <button className="absolute right-1 top-1/2 -translate-y-1/2 bg-white p-2 hover:bg-gray-100 cursor-pointer rounded">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                                <path
                                    d="M11 19C15.4183 19 19 15.4183 19 11C19 6.58172 15.4183 3 11 3C6.58172 3 3 6.58172 3 11C3 15.4183 6.58172 19 11 19Z"
                                    stroke="currentColor"
                                    strokeWidth="2"
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                />
                                <path
                                    d="M21 21L16.65 16.65"
                                    stroke="currentColor"
                                    strokeWidth="2"
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                />
                            </svg>
                        </button>
                    </form>
                </div>
            </div>

            <table className="w-full text-sm border-collapse border-spacing-0">
                <thead>
                    <tr className="text-quaternary text-sm font-semibold">
                        {columns.map((col) => (
                            <th
                                key={col.key}
                                className={`text-left relative py-2 after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[2px] after:bg-gray-300 
                    ${col.sortable ? 'cursor-pointer hover:bg-gray-50' : ''}`}
                                onClick={() => col.sortable && handleSort(col.key)}
                            >
                                {col.label}
                                {col.sortable && renderSortIcon(col.key)}
                            </th>
                        ))}
                        <th className="text-center relative w-16">
                            <span className="sr-only">Actions</span>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {returnRequests && returnRequests.length > 0 ? (
                        returnRequests.map((returnRequest, index) => (
                            <tr className="hover:bg-gray-50" key={index}>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.no}
                                </td>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.assetCode}
                                </td>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.assetName}
                                </td>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.requestedBy}
                                </td>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.assignedDate}
                                </td>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.acceptedBy || ''}
                                </td>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.returnedDate || ''}
                                </td>
                                <td className="py-2 relative after:absolute after:bottom-0 after:left-0 after:w-[calc(100%-20px)] after:h-[1px] after:bg-gray-300">
                                    {returnRequest.state}
                                </td>{' '}
                                <td className="py-2 relative">
                                    {' '}
                                    <div className="flex items-center justify-center space-x-2">
                                        <button
                                            className={`p-1 rounded ${isReturnRequestReturn(returnRequest.state)
                                                ? 'cursor-pointer hover:bg-red-100 hover:scale-110 transition-all duration-150'
                                                : 'opacity-50 cursor-not-allowed'
                                                }`}
                                            disabled={!isReturnRequestReturn(returnRequest.state)}
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                // Delete action
                                                setIsReplyReturnRequestPopupOpen(true);
                                                setSelectedReturnRequest(returnRequest);
                                                setType('accept');
                                            }}
                                        >
                                            <Check className="size-4" color="red" strokeWidth={3} />
                                        </button>
                                        <button
                                            className={`text-black p-1 rounded ${isReturnRequestReturn(returnRequest.state)
                                                ? 'hover:text-gray-700 cursor-pointer hover:bg-gray-300 hover:scale-110 transition-all duration-150'
                                                : 'opacity-50 cursor-not-allowed'
                                                }`}
                                            disabled={!isReturnRequestReturn(returnRequest.state)}
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                // Delete action
                                                setIsReplyReturnRequestPopupOpen(true);
                                                setSelectedReturnRequest(returnRequest);
                                                setType('cancel');
                                            }}
                                        >
                                            <svg
                                                width="14"
                                                height="14"
                                                viewBox="0 0 24 24"
                                                fill="none"
                                            >
                                                <path
                                                    d="M18 6L6 18"
                                                    stroke="currentColor"
                                                    strokeWidth="3"
                                                    strokeLinecap="round"
                                                    strokeLinejoin="round"
                                                />
                                                <path
                                                    d="M6 6L18 18"
                                                    stroke="currentColor"
                                                    strokeWidth="3"
                                                    strokeLinecap="round"
                                                    strokeLinejoin="round"
                                                />
                                            </svg>
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan={8} className="py-4 text-center text-gray-500">
                                No assignments found. Try changing your search criteria.
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>

            {/* Pagination */}
            {data && data.data.data.items.length > 0 && data.data.data.paginationMetadata && (
                <Pagination
                    queryConfig={queryConfig}
                    pathName={path.request}
                    totalPage={data?.data.data.paginationMetadata.totalPages}
                />
            )}

            <ReplyReturnRequestPopup
                returnRequestId={selectedReturnRequest?.id ?? ''}
                isOpen={isReplyReturnRequestPopupOpen}
                onClose={() => setIsReplyReturnRequestPopupOpen(false)}
                type={type}
            />
        </div>
    );
}
