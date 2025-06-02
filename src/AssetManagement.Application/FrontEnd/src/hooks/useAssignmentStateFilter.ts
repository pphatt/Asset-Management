import { ASSIGNMENT_STATE } from '@/constants/assignment-params';
import path from '@/constants/path';
import useQueryConfig from '@/hooks/useAssignmentQuery';
import { querySchema, QuerySchema } from '@/utils/rules';
import { yupResolver } from '@hookform/resolvers/yup';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { createSearchParams, useNavigate } from 'react-router-dom';

type FormData = Pick<QuerySchema, 'state'>;
const nameSchema = querySchema.pick(['state']);

export default function useAssignmentStateFilter() {
  const queryConfig = useQueryConfig();
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(false);

  const { register, handleSubmit } = useForm<FormData>({
    defaultValues: {
      state: queryConfig.state || '',
    },
    resolver: yupResolver(nameSchema),
  });

  const onStateChange = handleSubmit((data) => {
    const config = {
      ...queryConfig,
      state: data.state === 'All' ? '' : data.state,
      pageNumber: '1',
    };

    navigate({
      pathname: path.assignment,
      search: createSearchParams(config).toString(),
    });
  });
  const getSelectedState = () => {
    if (!queryConfig.state) return 'All';
    if (queryConfig.state === 'WaitingForAcceptance') return ASSIGNMENT_STATE.WAITING_FOR_ACCEPTANCE;
    return queryConfig.state;
  };

  const currentState = getSelectedState();
  const handleStateChange = (stateValue: string) => {
    const newState = currentState === stateValue ? 'All' : stateValue;
    const newApiState = newState === 'All' ? '' : newState === ASSIGNMENT_STATE.WAITING_FOR_ACCEPTANCE ? 'WaitingForAcceptance' : newState;

    const config = {
      ...queryConfig,
      state: newApiState,
      pageNumber: '1',
    };

    if (currentState !== stateValue) {
      setIsOpen(false);
    }

    navigate({
      pathname: path.assignment,
      search: createSearchParams(config).toString(),
    });
  };

  const toggleDropdown = () => setIsOpen((prev) => !prev);
  const stateOptions = [
    { value: 'All', label: 'All' },
    { value: ASSIGNMENT_STATE.ACCEPTED, label: 'Accepted' },
    { value: ASSIGNMENT_STATE.WAITING_FOR_ACCEPTANCE, label: 'Waiting for acceptance' },
  ];

  return {
    register,
    onStateChange,
    handleStateChange,
    toggleDropdown,
    isOpen,
    stateOptions,
    selectedState: currentState,
  };
}
