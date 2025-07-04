export const ASSET_FIELD_MAP = {
    code: 'code',
    name: 'name',
    category: 'category',
    state: 'state',
} as const;

export type AssetField = keyof typeof ASSET_FIELD_MAP;

export const ASSET_SORT_OPTIONS = {
    ASSET_CODE: 'assetCode',
    ASSET_NAME: 'name',
    CATEGORY: 'categoryName',
    STATE: 'state',
} as const;

export const getAssetApiField = (uiField: AssetField): string => {
    return ASSET_FIELD_MAP[uiField];
};

export const ASSET_STATE = {
    ALL: '',
    ASSIGNED: 'Assigned',
    AVAILABLE: 'Available',
    NOT_AVAILABLE: 'NotAvailable',
    WAITING_RECYCLING: 'WaitingForRecycling',
    RECYCLED: 'Recycled',
} as const;

export const ASSET_CATEGORY = {
    ALL: '',
    LAPTOP: 'Laptop',
    MONITOR: 'Monitor',
    PC: 'Personal Computer'
} as const;
export type AssetCategory = (typeof ASSET_CATEGORY)[keyof typeof ASSET_CATEGORY];

export type AssetState = (typeof ASSET_STATE)[keyof typeof ASSET_STATE];

export const ASSET_STATE_OPTIONS = [
    { value: ASSET_STATE.ALL, label: 'All' },
    { value: ASSET_STATE.ASSIGNED, label: 'Assigned' },
    { value: ASSET_STATE.AVAILABLE, label: 'Available' },
    { value: ASSET_STATE.NOT_AVAILABLE, label: 'Not available' },
    { value: ASSET_STATE.WAITING_RECYCLING, label: 'Waiting for recycling' },
    { value: ASSET_STATE.RECYCLED, label: 'Recycled' },
];

export const ASSET_CATEGORY_OPTIONS = [
    { value: ASSET_CATEGORY.ALL, label: 'All' },
    { value: ASSET_CATEGORY.LAPTOP, label: 'Laptop' },
    { value: ASSET_CATEGORY.MONITOR, label: 'Monitor' },
    { value: ASSET_CATEGORY.PC, label: 'Personal Computer' },
];

export const PAGINATION = {
    DEFAULT_PAGE_SIZE: 20,
    DEFAULT_PAGE_NUMBER: 1,
};
