import useUser from '@/hooks/useUser';
import { ICreateUserRequest } from '@/types/user.type';
import { yupResolver } from '@hookform/resolvers/yup';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import * as yup from 'yup';

const schema = yup.object().shape({
    firstName: yup.string().required('First name is required'),
    lastName: yup.string().required('Last name is required'),
    dateOfBirth: yup
        .date()
        .required('Please select date of birth')
        .typeError('Please select date of birth')
        .max(new Date(new Date().setFullYear(new Date().getFullYear() - 18)), 'User is under 18. Please select a different date'),
    joinedDate: yup
        .date()
        .required('Please select joined date')
        .typeError('Please select joined date')
        // Check DOB not null
        .test('dob-present', 'Please Select Date of Birth', function () {
            const { dateOfBirth } = this.parent;
            if (!dateOfBirth) return false;
            return true;
        })
        // Check age >= 18 at join
        .test(
            'age-validation',
            'User under the age of 18 may not join company. Please select a different date',
            function (value) {
                const { dateOfBirth } = this.parent;
                if (!dateOfBirth || !value) return true;
                const dob = new Date(dateOfBirth);
                const join = new Date(value);
                const adultDate = new Date(dob.setFullYear(dob.getFullYear() + 18));
                return join >= adultDate;
            }
        )
        // Check weekend
        .test('weekday', 'Joined date is Saturday or Sunday. Please select a different date', function (value) {
            if (!value) return true;
            const day = new Date(value).getDay();
            return day !== 0 && day !== 6;
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

const formatDateToString = (date: Date): string => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
};

export default function UserForm() {
    const navigate = useNavigate();
    const { useCreateUser } = useUser();
    const createUserMutation = useCreateUser();
    const {
        register,
        handleSubmit,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
    });

    const onSubmit = (data: FormData) => {
        console.log(data);
        const userData: ICreateUserRequest = {
            firstName: data.firstName,
            lastName: data.lastName,
            dateOfBirth: formatDateToString(data.dateOfBirth),
            gender: data.gender,
            joinedDate: formatDateToString(data.joinedDate),
            type: data.type,
        };
        createUserMutation.mutate(userData);
    };

    const onCancel = () => {
        navigate(-1);
    };

    return (
        <div className="max-w-md mx-auto p-6 bg-white border-gray-200">
            <h2 className="text-primary text-2xl font-bold mb-6">Create New User</h2>

            <form onSubmit={handleSubmit(onSubmit)}>
                <div className="space-y-5">
                    {/* First Name */}
                    <div className="grid grid-cols-12 items-center">
                        <label className="col-span-4 font-medium text-gray-700">First Name</label>
                        <div className="col-span-8">
                            <input
                                type="text"
                                {...register('firstName')}
                                className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary"
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
                                className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary"
                            />
                            {errors.lastName && <p className="mt-1 text-sm text-red-600">{errors.lastName.message}</p>}
                        </div>
                    </div>

                    {/* Date of Birth */}
                    <div className="grid grid-cols-12 items-center">
                        <label htmlFor="dateOfBirth" className="col-span-4 font-medium text-gray-700">Date of Birth</label>
                        <div className="col-span-8">
                            <input
                                id="dateOfBirth"
                                type="date"
                                {...register('dateOfBirth')}
                                className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary"
                            />
                            {errors.dateOfBirth && <p className="mt-1 text-sm text-red-600">{errors.dateOfBirth.message}</p>}
                        </div>
                    </div>

                    {/* Gender */}
                    <div className="grid grid-cols-12 items-center">
                        <label className="col-span-4 font-medium text-gray-700">Gender</label>
                        <div className="col-span-8 flex space-x-6">
                            <div className="flex items-center">
                                <input
                                    type="radio"
                                    id="female"
                                    value={2}
                                    {...register('gender')}
                                    className="h-4 w-4 accent-primary focus:ring-primary border-gray-300"
                                />
                                <label htmlFor="female" className="ml-2 text-gray-700">Female</label>
                            </div>
                            <div className="flex items-center">
                                <input
                                    type="radio"
                                    id="male"
                                    value={1}
                                    {...register('gender')}
                                    className="h-4 w-4 accent-primary focus:ring-primary border-gray-300"
                                />
                                <label htmlFor="male" className="ml-2 text-gray-700">Male</label>
                            </div>
                        </div>
                        {errors.gender && <p className="col-start-5 col-span-8 mt-1 text-sm text-red-600">{errors.gender.message}</p>}
                    </div>

                    {/* Joined Date */}
                    <div className="grid grid-cols-12 items-center">
                        <label htmlFor="joinedDate" className="col-span-4 font-medium text-gray-700">Joined Date</label>
                        <div className="col-span-8">
                            <input
                                id="joinedDate"
                                type="date"
                                {...register('joinedDate')}
                                className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary"
                            />
                            {errors.joinedDate && <p className="mt-1 text-sm text-red-600">{errors.joinedDate.message}</p>}
                        </div>
                    </div>

                    {/* Type */}
                    <div className="grid grid-cols-12 items-center">
                        <label className="col-span-4 font-medium text-gray-700">Type</label>
                        <div className="col-span-8">
                            <select
                                {...register('type')}
                                className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary focus:border-primary"
                            >
                                <option value="">Select Type</option>
                                <option value={1}>Admin</option>
                                <option value={2}>Staff</option>
                            </select>
                        </div>
                        {errors.type && <p className="col-start-5 col-span-8 mt-1 text-sm text-red-600">{errors.type.message}</p>}
                    </div>
                </div>

                {/* Buttons */}
                <div className="mt-8 flex justify-end space-x-4">
                    <button
                        type="submit"
                        className="px-4 py-2 bg-primary text-white rounded-md hover:bg-primary-dark focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 transition-colors"
                    >
                        Save
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