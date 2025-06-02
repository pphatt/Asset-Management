import { ASSIGNMENT_STATE } from '@/constants/assignment-params';
import { AssignmentState } from '@/types/assignment.type';
import { GenderEnum, LocationEnum, UserTypeEnum } from '@/types/user.type';

// Map enum to string
export const GenderDisplay: Record<GenderEnum, string> = {
  [GenderEnum.Male]: 'Male',
  [GenderEnum.Female]: 'Female',
};

export const UserTypeDisplay: Record<UserTypeEnum, string> = {
  [UserTypeEnum.Admin]: 'Admin',
  [UserTypeEnum.Staff]: 'Staff',
};
export const LocationDisplay: Record<LocationEnum, string> = {
  [LocationEnum.HCM]: 'HCM',
  [LocationEnum.DN]: 'DN',
  [LocationEnum.HN]: 'HN',
};
/**
 * Map enum to string
 */
export function getGenderDisplay(gender: GenderEnum): string {
  return GenderDisplay[gender] || 'Unknown';
}

export function getUserTypeDisplay(type: UserTypeEnum): string {
  return UserTypeDisplay[type] || 'Unknown';
}

export function getLocationDisplay(location: LocationEnum): string {
  return LocationDisplay[location] || 'Unknown';
}

/**
 * Check if an assignment can be edited or deleted
 * @param state The assignment state to check
 * @returns true if the assignment can be edited or deleted, false otherwise
 */
export function isAssignmentModifiable(state: AssignmentState): boolean {
  return state === ASSIGNMENT_STATE.WAITING_FOR_ACCEPTANCE;
}

/**
 * Get a human-readable message explaining why an assignment can't be edited
 * @param state The assignment state
 * @returns A message explaining edit permissions, or empty string if editable
 */
export function getAssignmentEditMessage(state: AssignmentState): string {
  if (isAssignmentModifiable(state)) {
    return '';
  }
  return "Only assignments with 'Waiting for acceptance' state can be edited";
}

/**
 * Get the human-friendly label for an assignment state
 * @param state The assignment state
 * @returns A formatted string label for display
 */
export function getAssignmentStateLabel(state: AssignmentState): string {
  // The values are already human-readable, but we could add custom formatting here
  return state;
}

/**
 * Check if an assignment state is the provided state
 * @param currentState The assignment's current state
 * @param targetState The state to check against
 * @returns true if the states match
 */
export function isAssignmentState(currentState: AssignmentState, targetState: AssignmentState): boolean {
  return currentState === targetState;
}

/**
 * Map string to enum
 */
export function parseGenderEnum(gender: string): GenderEnum {
  return gender === 'Male' ? GenderEnum.Male : GenderEnum.Female;
}

export function parseUserTypeEnum(type: string): UserTypeEnum {
  return type === 'Admin' ? UserTypeEnum.Admin : UserTypeEnum.Staff;
}

/**
 * Check if the enum value is valid
 */
export function isValidGender(value: number): value is GenderEnum {
  return value === GenderEnum.Male || value === GenderEnum.Female;
}

export function isValidUserType(value: number): value is UserTypeEnum {
  return value === UserTypeEnum.Admin || value === UserTypeEnum.Staff;
}
