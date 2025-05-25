import useUser from '@/hooks/useUser';
import { ICreateUserRequest, IUpdateUserRequest } from '@/types/user.type';
import { yupResolver } from '@hookform/resolvers/yup';
import { useEffect } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useNavigate, useParams } from 'react-router-dom';
import * as yup from 'yup';

const schema = yup.object().shape({
    firstName: yup.string().required('First name is required').max(30, "First name can't be more than 30 characters long"),
    lastName: yup.string().required('Last name is required').max(30, "Last name can't be more than 30 characters long"),
    dateOfBirth: yup
        .string()
        .required('Please Select Date of Birth')
        .test('valid-date', 'Invalid date format', value => !isNaN(Date.parse(value)))
        .test('age', 'User is under 18. Please select a different date', value => {
            const dob = new Date(value);
            const today = new Date();
            const age = today.getFullYear() - dob.getFullYear();
            const monthDiff = today.getMonth() - dob.getMonth();
            const dayDiff = today.getDate() - dob.getDate();

            // More precise age calculation
            if (age < 18) return false;
            if (age === 18 && monthDiff < 0) return false;
            if (age === 18 && monthDiff === 0 && dayDiff < 0) return false;

            return true;
        }),
    joinedDate: yup
        .string()
        .required('Please Select Joined Date')
        .test('valid-date', 'Invalid date format', value => !isNaN(Date.parse(value)))
        .test('dob-required', 'Please Select Date of Birth', function () {
            // Access dateOfBirth from the same validation context
            const { dateOfBirth } = this.parent;
            if (!dateOfBirth) return false;
            return true;
        })
        .test('not-weekend', 'Joined date is Saturday or Sunday. Please select a different date', value => {
            const day = new Date(value).getDay();
            return day !== 0 && day !== 6;
        })
        .test('age-at-join', 'User under the age of 18 may not join company. Please select a different date', function (value) {
            const { dateOfBirth } = this.parent;
            if (!dateOfBirth || !value) return true; // Skip if either date is missing

            const dob = new Date(dateOfBirth);
            const joinDate = new Date(value);

            // Calculate age at join date
            const ageAtJoin = joinDate.getFullYear() - dob.getFullYear();
            const monthDiff = joinDate.getMonth() - dob.getMonth();
            const dayDiff = joinDate.getDate() - dob.getDate();

            // Check if user is at least 18 at join date
            if (ageAtJoin < 18) return false;
            if (ageAtJoin === 18 && monthDiff < 0) return false;
            if (ageAtJoin === 18 && monthDiff === 0 && dayDiff < 0) return false;

            return true;
        }),
    gender: yup
        .number()
        .oneOf([1, 2], 'Invalid gender')
        .required('Gender is required'),
    type: yup
        .number()
        .transform((value, originalValue) =>
            originalValue === "" ? undefined : value
        )
        .required('Type is required')
        .oneOf([1, 2], 'Invalid staff type'),
});

type FormData = yup.InferType<typeof schema>;

interface UserFormProps {
    mode: 'create' | 'edit';
}

const convertToInputFormat = (date: string): string => {
    const [day, month, year] = date.split('/');
    return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
}

export default function UserForm({ mode }: UserFormProps) {
    const navigate = useNavigate();
    const { staffCode } = useParams<{ staffCode: string }>();
    const isEditMode = mode === 'edit';

    const { useCreateUser, useUpdateUser, useUserByStaffCode } = useUser();
    const { data: userData, isLoading: isLoadingUser } = useUserByStaffCode(isEditMode ? staffCode : undefined);
    const createUserMutation = useCreateUser();
    const updateUserMutation = useUpdateUser();

    const {
        register,
        handleSubmit,
        reset,
        control,
        formState: { errors, isSubmitting, isValid }
    } = useForm<FormData>({
        resolver: yupResolver(schema),
        mode: 'onChange',
        defaultValues: {
            firstName: '',
            lastName: '',
            dateOfBirth: undefined,
            joinedDate: undefined,
            gender: undefined,
            type: undefined,
        }
    });

    const onSubmit = (data: FormData) => {
        // console.log(data);
        if (isEditMode) {
            const userData: IUpdateUserRequest = {
                dateOfBirth: data.dateOfBirth,
                gender: data.gender,
                joinedDate: data.joinedDate,
                type: data.type,
            }
            // console.log(userData);
            if (staffCode)
                updateUserMutation.mutate({ staffCode, userData });
        }
        else {
            const userData: ICreateUserRequest = {
                firstName: data.firstName,
                lastName: data.lastName,
                dateOfBirth: data.dateOfBirth,
                gender: data.gender,
                joinedDate: data.joinedDate,
                type: data.type
            };
            // console.log(userData);
            createUserMutation.mutate(userData);
        }
    };

    const onCancel = () => {
        navigate(-1);
    };

    useEffect(() => {
        if (isEditMode && userData) {
            console.log('Setting form data:', userData);

            reset({
                firstName: userData.firstName,
                lastName: userData.lastName,
                dateOfBirth: convertToInputFormat(userData.dateOfBirth),
                joinedDate: convertToInputFormat(userData.joinedDate),
                gender: userData.gender,
                type: userData.type,
            });
        }
    }, [isEditMode, userData, reset]);

    if (isEditMode && isLoadingUser) {
        return <div className="flex justify-center p-8">Loading user data...</div>;
    }

    // Check if form has errors
    const hasErrors = Object.keys(errors).length > 0;
    const isSubmitDisabled = !isValid || hasErrors || isSubmitting || (createUserMutation.isPending || updateUserMutation.isPending);

    return (
        <div className="max-w-md mx-auto p-6 bg-white border-gray-200">
            <h2 className="text-primary text-2xl font-bold mb-6">
                {isEditMode ? 'Edit User' : 'Create New User'}
            </h2>

            <form onSubmit={handleSubmit(onSubmit)}>
                <div className="space-y-5">
                    {/* First Name */}
                    <div className="grid grid-cols-12 items-center">
                        <label className="col-span-4 font-medium text-gray-700">First Name</label>
                        <div className="col-span-8">
                            <input
                                type="text"
                                {...register('firstName')}
                                disabled={isEditMode}
                                className={`w-full p-2 border rounded-md focus:ring-2 focus:ring-primary focus:border-primary transition-colors ${isEditMode
                                    ? 'bg-gray-100 text-gray-500 border-gray-200 cursor-not-allowed'
                                    : 'bg-white text-gray-900 border-gray-300 hover:border-gray-400'
                                    }`}
                            />
                            {errors.firstName && <p className="mt-1 text-sm text-red-600">{errors.firstName.message}</p>}
                        </div>
                    </div>

                    {/* Last Name */}
                    <div className="grid grid-cols-12 items-center">
                        <label className="col-span-4 font-medium text-gray-700">Last Name</label>
                        <div className="col-span-8">
                            <input
                                type="text"
                                {...register('lastName')}
                                disabled={isEditMode}
                                className={`w-full p-2 border rounded-md focus:ring-2 focus:ring-primary focus:border-primary transition-colors ${isEditMode
                                    ? 'bg-gray-100 text-gray-500 border-gray-200 cursor-not-allowed'
                                    : 'bg-white text-gray-900 border-gray-300 hover:border-gray-400'
                                    }`}
                            />
                            {errors.lastName && <p className="mt-1 text-sm text-red-600">{errors.lastName.message}</p>}
                        </div>
                    </div>

                    {/* Date of Birth */}
                    <div className="grid grid-cols-12 items-center">
                        <label htmlFor="dateOfBirth" className="col-span-4 font-medium text-gray-700">
                            Date of Birth
                        </label>
                        <div className="col-span-8">
                            <input
                                id="dateOfBirth"
                                type="date"
                                min="1000-01-01"
                                max="9999-12-31"
                                // RHF stores this as a string; Yup will parse it
                                {...register('dateOfBirth')}
                                className="w-full p-2 border rounded-md focus:ring-2 focus:ring-primary focus:border-primary transition-colors"
                            />
                            {errors.dateOfBirth && (
                                <p className="mt-1 text-sm text-red-600">
                                    {errors.dateOfBirth.message}
                                </p>
                            )}
                        </div>
                    </div>

                    {/* Gender */}
                    <div className="grid grid-cols-12 items-center">
                        <label className="col-span-4 font-medium text-gray-700">Gender</label>
                        <Controller
                            name='gender'
                            defaultValue={1}
                            control={control}
                            render={({ field }) => (
                                <div className="col-span-8 flex space-x-6">
                                    <div className="flex items-center">
                                        <input
                                            type="radio"
                                            id="female"
                                            value={2}
                                            checked={field.value === 2}
                                            onChange={() => field.onChange(2)}
                                            className="h-4 w-4 accent-primary focus:ring-primary border-gray-300"
                                        />
                                        <label htmlFor="female" className="ml-2 text-gray-700">Female</label>
                                    </div>
                                    <div className="flex items-center">
                                        <input
                                            type="radio"
                                            id="male"
                                            value={1}
                                            checked={field.value === 1}
                                            onChange={() => field.onChange(1)}
                                            className="h-4 w-4 accent-primary focus:ring-primary border-gray-300"
                                        />
                                        <label htmlFor="male" className="ml-2 text-gray-700">Male</label>
                                    </div>
                                </div>
                            )}
                        />
                        {errors.gender && <p className="col-start-5 col-span-8 mt-1 text-sm text-red-600">{errors.gender.message}</p>}
                    </div>

                    {/* Joined Date */}
                    <div className="grid grid-cols-12 items-center">
                        <label htmlFor="joinedDate" className="col-span-4 font-medium text-gray-700">
                            Joined Date
                        </label>
                        <div className="col-span-8">
                            <input
                                id="joinedDate"
                                type="date"
                                min="1000-01-01"
                                max="9999-12-31"
                                {...register('joinedDate')}
                                className="w-full p-2 border rounded-md focus:ring-2 focus:ring-primary focus:border-primary transition-colors"
                            />
                            {errors.joinedDate && (
                                <p className="mt-1 text-sm text-red-600">
                                    {errors.joinedDate.message}
                                </p>
                            )}
                        </div>
                    </div>

                    {/* Type */}
                    <div className="grid grid-cols-12 items-center">
                        <label className="col-span-4 font-medium text-gray-700">Type</label>
                        <div className="col-span-8">
                            <select
                                {...register('type', {
                                    setValueAs: (value) => value === "" ? undefined : Number(value)
                                })}
                                className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary hover:border-gray-400 transition-colors"
                            >
                                <option value="">Select Type</option>
                                <option value="1">Admin</option>
                                <option value="2">Staff</option>
                            </select>
                            {errors.type && <p className="mt-1 text-sm text-red-600">{errors.type.message}</p>}
                        </div>
                    </div>
                </div>

                {/* Buttons */}
                <div className="mt-8 flex justify-end space-x-4">
                    <button
                        type="submit"
                        disabled={isSubmitDisabled}
                        className={`px-4 py-2 rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 transition-colors ${isSubmitDisabled
                            ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                            : 'bg-primary text-white hover:bg-primary-dark focus:ring-primary'
                            }`}
                    >
                        {isSubmitting || createUserMutation.isPending || updateUserMutation.isPending ? 'Saving...' : 'Save'}
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
}