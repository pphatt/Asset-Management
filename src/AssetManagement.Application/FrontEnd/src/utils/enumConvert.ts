import { GenderEnum, LocationEnum, UserTypeEnum } from "@/types/user.type";

// Map enum to string
export const GenderDisplay: Record<GenderEnum, string> = {
    [GenderEnum.Male]: "Male",
    [GenderEnum.Female]: "Female"
};

export const UserTypeDisplay: Record<UserTypeEnum, string> = {
    [UserTypeEnum.Admin]: "Admin",
    [UserTypeEnum.Staff]: "Staff"
};
export const LocationDisplay: Record<LocationEnum, string> = {
    [LocationEnum.HCM]: "HCM",
    [LocationEnum.DN]: "DN",
    [LocationEnum.HN]: "HN"
}
/**
* Map enum to string
*/
export function getGenderDisplay(gender: GenderEnum): string {
    return GenderDisplay[gender] || "Unknown";
}

export function getUserTypeDisplay(type: UserTypeEnum): string {
    return UserTypeDisplay[type] || "Unknown";
}

export function getLocationDisplay(location: LocationEnum): string {
    return LocationDisplay[location] || "Unknown";
}

/**
* Map string to enum
*/
export function parseGenderEnum(gender: string): GenderEnum {
    return gender === "Male" ? GenderEnum.Male : GenderEnum.Female;
}

export function parseUserTypeEnum(type: string): UserTypeEnum {
    return type === "Admin" ? UserTypeEnum.Admin : UserTypeEnum.Staff;
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