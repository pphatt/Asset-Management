import * as yup from "yup";

function handleConfirmPassword(typeConfirm: string) {
  return yup
    .string()
    .required("Confirm Password is required")
    .min(12, "New Password Format Error")
    .oneOf([yup.ref(`${typeConfirm}`)], "Password not match");
}

export const schema = yup.object({
  email: yup
    .string()
    .required("Email is Required !")
    .email()
    .min(5, "Can not under 5 characters")
    .max(160, "Can not exceed 160 characters"),
  confirmPassword: handleConfirmPassword("newPassword"),
  password: yup.string().required("Old Password is required"),
  newPassword: yup
    .string()
    .required("New Password is required")
    .min(12, "New Password Format Error"),
});

export const querySchema = yup.object({
  searchName: yup.string().optional(),
  states: yup
    .array()
    .of(
      yup
        .string()
        .required()
        .oneOf(["Accepted", "Declined", "Returned", "Waiting for acceptance"])
    )
    .default([]),
});

export type Schema = yup.InferType<typeof schema>;
export type QuerySchema = yup.InferType<typeof querySchema>;
