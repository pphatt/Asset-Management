export const USER_FIELD_MAP = {
  staffCode: 'code',
  fullName: 'name',
  username: 'username',
  joinedDate: 'joined',
  type: 'type',
} as const;

export type UserField = keyof typeof USER_FIELD_MAP;

export const USER_SORT_OPTIONS = {
  STAFF_CODE: 'staffCode',
  FULL_NAME: 'fullName',
  USERNAME: 'username',
  JOINED_DATE: 'joinedDate',
  TYPE: 'type',
} as const;

export const getUserApiField = (uiField: UserField): string => {
  return USER_FIELD_MAP[uiField];
};

export const USER_TYPES = {
  ALL: '',
  ADMIN: 'Admin',
  STAFF: 'Staff',
} as const;

export type UserType = typeof USER_TYPES[keyof typeof USER_TYPES];

export const USER_TYPE_OPTIONS = [
  { value: USER_TYPES.ALL, label: 'All' },
  { value: USER_TYPES.ADMIN, label: 'Admin' },
  { value: USER_TYPES.STAFF, label: 'Staff' },
];

export const STORAGE_KEYS = {
  CURRENT_USER: 'currentUser',
  USER_FILTER_STATE: 'userFilterState',
};

export const PAGINATION = {
  DEFAULT_PAGE_SIZE: 10,
  DEFAULT_PAGE_NUMBER: 1,
}; 