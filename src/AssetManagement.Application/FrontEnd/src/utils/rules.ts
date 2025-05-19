import * as yup from "yup";

// Sample Setup about Yup
export const schema = yup.object({
  email: yup
    .string()
    .required("Email is Required !")
    .email()
    .min(5, "Can not under 5 characters")
    .max(160, "Can not exceed 160 characters"),
  password: yup
    .string()
    .required("Password Is Required !")
    .min(6, "Can not under 5 characters")
    .max(160, "Can not exceed 160 characters"),
});

export type Schema = yup.InferType<typeof schema>;
