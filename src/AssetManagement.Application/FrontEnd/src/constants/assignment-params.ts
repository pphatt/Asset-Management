import { AssignmentState } from '@/types/assignment.type';

export const ASSIGNMENT_STATE: Record<string, AssignmentState> = {
  ACCEPTED: 'Accepted',
  DECLINED: 'Declined',
  RETURNED: 'Returned',
  WAITING_FOR_ACCEPTANCE: 'Waiting for acceptance',
};

export const ASSIGNMENT_STATE_OPTIONS = [
  { value: 'All', label: 'All' },
  { value: ASSIGNMENT_STATE.ACCEPTED, label: 'Accepted' },
  { value: ASSIGNMENT_STATE.WAITING_FOR_ACCEPTANCE, label: 'Waiting for acceptance' },
  { value: ASSIGNMENT_STATE.DECLINED, label: 'Declined' },
  { value: ASSIGNMENT_STATE.RETURNED, label: 'Returned' },
];
