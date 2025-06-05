import path from '@/constants/path';
import { IAssignmentCreateUpdateRequest } from '@/types/assingment.type';
import { formatDateToString, getStartOfToday } from '@/utils/formatDate';
import { yupResolver } from '@hookform/resolvers/yup';
import React, { useEffect, useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import * as yup from 'yup';
import AssetSearchDropdown from './AssetSearchDropdown';
import FormRow from './FormRow';
import UserSearchDropdown from './UserSearchDropdown';

/**
 * Props cho AssignmentForm
 * @property mode - 'create' hoặc 'edit'
 * @property initialData - Dữ liệu khởi tạo khi edit
 * @property onSubmitForm - Callback khi submit form thành công
 * @property isExternalSubmitting - Trạng thái submit từ bên ngoài (ví dụ: đang gọi API)
 */
export interface AssignmentFormProps {
  mode: 'create' | 'edit';
  initialData?: IAssignmentCreateUpdateRequest;
  onSubmitForm?: (data: IAssignmentCreateUpdateRequest) => void;
  isExternalSubmitting?: boolean;
  selectedAssetInfo?: {
    id: string;
    code: string;
    name: string;
  };
  selectedUserInfo?: {
    id: string;
    username: string;
    staffCode: string;
  };
}

const assignmentSchema = yup.object().shape({
  assetId: yup.string().required('Asset ID is required'),
  assigneeId: yup.string().required('Assignee ID is required'),
  assignedDate: yup.string().required('Assigned Date is required'),
  note: yup.string().default(''),
});

/**
 * AssignmentForm component
 */
const AssignmentForm: React.FC<AssignmentFormProps> = ({
  mode,
  initialData,
  onSubmitForm,
  isExternalSubmitting = false,
  selectedAssetInfo,
  selectedUserInfo,
}) => {
  const navigate = useNavigate();
  const [isFormSubmitted, setIsFormSubmitted] = useState(false);

  // Default values cho form
  const defaultValues: IAssignmentCreateUpdateRequest = {
    assetId: '',
    assigneeId: '',
    assignedDate: formatDateToString(getStartOfToday()),
    note: '',
  };

  // Khởi tạo form với react-hook-form
  const {
    control,
    handleSubmit,
    formState: { errors, isValid, isSubmitting: formIsSubmitting },
    reset,
    setValue,
    watch,
  } = useForm<IAssignmentCreateUpdateRequest>({
    resolver: yupResolver(assignmentSchema),
    defaultValues: mode === 'create' ? defaultValues : { ...defaultValues, ...initialData },
    mode: 'onSubmit',
    reValidateMode: 'onSubmit',
  });

  // Theo dõi giá trị assignedDate để debug
  const assignedDateValue = watch('assignedDate');
  useEffect(() => {}, [assignedDateValue]);

  /**
   * Xử lý submit form
   * @param data - Dữ liệu form đã validate
   */
  const onSubmit = (data: IAssignmentCreateUpdateRequest) => {
    setIsFormSubmitted(true);
    try {
      if (onSubmitForm) {
        onSubmitForm(data);
      } else {
        toast.info(
          mode === 'create'
            ? 'Assignment created with state "Waiting for acceptance". Redirecting to Manage Assignment page...'
            : 'Assignment updated. Redirecting to Manage Assignment page...'
        );
      }
    } catch (error) {
      toast.error(`${error instanceof Error ? error.message : 'Unknown error'}`);
    }
  };

  /** Quay lại trang assignment */
  const onCancel = () => navigate(path.assignment);

  // Trạng thái submit tổng hợp
  const isSubmitting = formIsSubmitting || isExternalSubmitting;
  const isSubmitDisabled = !isValid || isSubmitting;

  // Reset form khi chuyển sang edit hoặc initialData thay đổi
  useEffect(() => {
    if (mode === 'edit' && initialData) {
      reset(initialData);

      // Đảm bảo giá trị assignedDate được set đúng
      if (initialData.assignedDate) {
        setValue('assignedDate', initialData.assignedDate);
      }
    }
  }, [mode, initialData, reset, setValue]);

  return (
    <div className="max-w-md mx-auto p-6 bg-white border-gray-200">
      <h2 className="text-primary text-2xl font-bold mb-6">{mode === 'create' ? 'Create New Assignment' : 'Edit Assignment'}</h2>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="space-y-5">
          {' '}
          {/* User */}
          <FormRow label="User" error={isFormSubmitted ? errors.assigneeId?.message : undefined}>
            <Controller
              name="assigneeId"
              control={control}
              render={({ field }) => (
                <UserSearchDropdown
                  value={field.value}
                  onChange={field.onChange}
                  mode={mode}
                  selectedUserInfo={selectedUserInfo}
                  className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary hover:border-gray-400 transition-colors"
                />
              )}
            />
          </FormRow>
          {/* Asset */}
          <FormRow label="Asset" error={isFormSubmitted ? errors.assetId?.message : undefined}>
            <Controller
              name="assetId"
              control={control}
              render={({ field }) => (
                <AssetSearchDropdown
                  value={field.value}
                  onChange={field.onChange}
                  mode={mode}
                  selectedAssetInfo={selectedAssetInfo}
                  className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary hover:border-gray-400 transition-colors"
                />
              )}
            />
          </FormRow>
          {/* Assigned Date */}
          <FormRow label="Assigned Date" error={isFormSubmitted ? errors.assignedDate?.message : undefined}>
            <Controller
              name="assignedDate"
              control={control}
              render={({ field }) => (
                <input
                  id="assignedDate"
                  type="date"
                  value={field.value ?? ''}
                  onChange={(e) => {
                    const selectedDate = e.target.value; // string yyyy-MM-dd

                    // Nếu đang ở chế độ tạo mới, kiểm tra ngày có trước ngày hiện tại không
                    if (mode === 'create') {
                      const today = formatDateToString(getStartOfToday());
                      if (selectedDate < today) {
                        console.warn('Selected date is before today, ignoring');
                        return;
                      }
                    }

                    field.onChange(selectedDate);
                  }}
                  min={mode === 'create' ? formatDateToString(new Date()) : undefined}
                  className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary hover:border-gray-400 transition-colors"
                />
              )}
            />
          </FormRow>
          {/* Note */}
          <FormRow label="Note" error={isFormSubmitted ? errors.note?.message : undefined}>
            <Controller
              name="note"
              control={control}
              render={({ field }) => (
                <textarea
                  id="note"
                  {...field}
                  value={field.value ?? ''}
                  onChange={(e) => field.onChange(e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary hover:border-gray-400 transition-colors"
                  rows={3}
                />
              )}
            />
          </FormRow>
        </div>

        {/* Buttons */}
        <div className="mt-8 flex justify-end space-x-4">
          <button
            type="submit"
            disabled={isSubmitDisabled}
            className={`px-4 py-2 rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 transition-colors ${
              isSubmitDisabled ? 'bg-gray-300 text-gray-500 cursor-not-allowed' : 'bg-primary text-white hover:bg-primary-dark focus:ring-primary'
            }`}
          >
            {isSubmitting ? 'Saving...' : 'Save'}
          </button>
          <button
            type="button"
            className="px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-400 transition-colors"
            onClick={onCancel}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default AssignmentForm;
