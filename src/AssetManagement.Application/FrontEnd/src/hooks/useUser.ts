import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';
import userApi from '../apis/user.api';
import { getUserApiField, STORAGE_KEYS, UserField } from '../constants/user-params';
import { ICreateUserRequest, IUpdateUserRequest, IUserParams } from '../types/user.type';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import useUserFilterState from './useUserFilterState';

export function useUser() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [_, setQueryParams] = useUserFilterState();

  /**
   * Get the current admin's location
   * @returns {string} The current admin's location
   * @description Get the current admin's location from localStorage
   * @technique {useCallback} -> Memoize the function to avoid re-rendering
   */
  const getCurrentAdminLocation = useCallback(() => {
    const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.CURRENT_USER) || '{}');
    return user?.location || '';
  }, []);

  /**
   * Get the users list
   * @param {IUserParams} params - The parameters for the users list
   * @returns {UseQueryResult<PaginatedResult<IUser>>} The users list
   * @description Get the users list from the API
   * @technique {useQuery} -> Query the users list from the API
   */
  function useUsersList(params: IUserParams) {
    const adminLocation = getCurrentAdminLocation();

    return useQuery({
      queryKey: ['users', params],
      queryFn: async () => {
        const apiParams = { ...params };
        if (!apiParams.location) {
          apiParams.location = adminLocation;
        }
        if (apiParams.sortBy) {
          const [field, direction] = apiParams.sortBy.split(':');
          const apiField = getUserApiField(field as UserField) || field;
          apiParams.sortBy = `${apiField}:${direction}`;
        }
        if (apiParams._apiSortBy) {
          apiParams.sortBy = apiParams._apiSortBy;
          delete apiParams._apiSortBy;
        }
        console.log('apiParams', apiParams);
        const response = await userApi.getUsers(apiParams);
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch users');
      },
    });
  }

  /**
   * Get the user types
   * @returns {UseQueryResult<IUserType[]>} The user types
   * @description Get the user types from the API
   * @technique {useQuery} -> Query the user types from the API
   */
  function useUserTypes() {
    return useQuery({
      queryKey: ['userTypes'],
      queryFn: async () => {
        const response = await userApi.getUserTypes();
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch user types');
      },
      staleTime: 24 * 60 * 60 * 1000,
    });
  }

  /**
   * Get the user by staff code
   * @param {string} staffCode - The staff code of the user
   * @returns {UseQueryResult<IUser>} The user
   * @description Get the user by staff code from the API
   */
  function useUserByStaffCode(staffCode?: string) {
    return useQuery({
      queryKey: ['user', staffCode],
      queryFn: async () => {
        if (!staffCode) throw new Error('Staff Code is required');
        const response = await userApi.getUserByStaffCode(staffCode);
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch user');
      },
      enabled: !!staffCode,
    });
  }

  /**
   * Create a new user
   * @returns {UseMutationResult<IUser>} The new user
   * @description Create a new user from the API
   * @technique {useMutation} -> Mutate the new user from the API
   */
  function useCreateUser() {
    return useMutation({
      mutationFn: async (userData: ICreateUserRequest) => userApi.createUser(userData),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['users'] });
        setQueryParams((prev) => ({
          ...prev,
          // currently keep this as a fixed string (TODO: refactor this)
          sortBy: 'created:desc',
          pageNumber: 1,
        }))
        toast.success("User created successfully");
        navigate('/user');
      },
      onError: (err: any) => {
        const errMsg = err.response?.data?.errors;
        toast.error(errMsg?.[0] || "Error creating user");
      },
    });
  }

  /**
   * Update a user
   * @returns {UseMutationResult<IUser>} The updated user
   * @description Update a user from the API
   * @technique {useMutation} -> Mutate the updated user from the API
   */
  function useUpdateUser() {
    return useMutation({
      mutationFn: async ({ staffCode, data }: { staffCode: string; data: IUpdateUserRequest }) => {
        const response = await userApi.updateUser(staffCode, data);
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to update user');
      },
    });
  }

  /**
   * Delete a user
   * @returns {UseMutationResult<void>} The deleted user
   * @description Delete a user from the API
   * @technique {useMutation} -> Mutate the deleted user from the API
   */
  function useDeleteUser() {
    return useMutation({
      mutationFn: async (staffCode: string) => {
        const response = await userApi.deleteUser(staffCode);
        if (!response.success) {
          throw new Error(response.message || 'Failed to delete user');
        }
      },
    });
  }

  return {
    useUsersList,
    useUserTypes,
    useUserByStaffCode,
    useCreateUser,
    useUpdateUser,
    useDeleteUser,
    getCurrentAdminLocation,
  };
}

export default useUser;
