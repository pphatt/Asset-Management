import styles from "@/pages/UpdatePassword/styles.module.css";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card.tsx";
import { Label } from "@/components/ui/label.tsx";
import { Controller, useForm } from "react-hook-form";
import { PasswordInput } from "@/components/layout/password-input.tsx";

import { Schema, schema } from "@/utils/rules";
import { yupResolver } from "@hookform/resolvers/yup";
import { useMutation } from "@tanstack/react-query";
import { PasswordUpdateRequest } from "@/types/auth.type.ts";
import authApi from "@/apis/auth.api.ts";
import { toast } from "react-toastify";
import { jwtDecode, JwtPayload } from "jwt-decode";
import { setCookie } from "@/utils/auth.ts";
import { useAppContext } from "@/hooks/use-app-context.tsx";
import { Button } from "@/components/ui/button.tsx";

type FormData = Pick<Schema, "newPassword">;
const passwordSchema = schema.pick([
  "newPassword",
  "confirmPassword",
]);

type JWTPayload = {
  "http://schemas.microsoft.com/ws/2008/06/jexp": string;
} & JwtPayload;

export default function FirstChangePassword() {

  const {
    handleSubmit,
    control,
    getValues,
    formState: { errors },
  } = useForm({
    mode: "all",
    resolver: yupResolver(passwordSchema),
    defaultValues: { newPassword: "", confirmPassword: "" },
  });

  const { setIsAuthenticated, setProfile } = useAppContext();

  const { isPending: isLoading, mutate } = useMutation({
    mutationFn: (body: PasswordUpdateRequest) => authApi.changePassword(body),
    onError: (err: any) => {
      const errMsg = err.response.data.errors;
      toast.error(errMsg[0] || "error when change password");
    },
  });

  const onSubmit = (data: FormData) => {
    const { newPassword } = data;
    mutate(
      { newPassword },
      {
        onSuccess(info) {
          const user = data && info?.data?.userInfo;
          const accessToken = data && info?.data.accessToken;

          const decode = jwtDecode<JWTPayload>(accessToken);

          const {
            staffCode,
            firstName,
            lastName,
            username,
            type,
            joinedDate,
            isPasswordUpdated,
          } = user;

          setProfile({
            staffCode,
            firstName,
            lastName,
            username,
            type,
            joinedDate,
            isPasswordUpdated,
          });

          const expiredTime = Number.parseInt(
            decode["http://schemas.microsoft.com/ws/2008/06/jexp"],
          );

          setCookie("access_token", accessToken, expiredTime);

          setCookie("profile", JSON.stringify(user), expiredTime);

          setIsAuthenticated(true);
          toast.success("Change password successfully!");
        },
      },
    );
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center z-[100] backdrop-blur-[3px]">
      <Card className="gap-0 p-0 border-0 rounded-[8px] shadow-none">
        <CardHeader className={styles["card-header"]}>
          <CardTitle
            className={`text-lg tracking-tight ${styles["card-title"]}`}
          >
            Change password
          </CardTitle>
        </CardHeader>

        <CardContent className={styles["card-content"]}>
          <p className="text-[1rem] text-gray-600 mb-4">
            This is the first time you logged in. <br /> You have to change your
            password to continue.
          </p>

          <form className="relative" onSubmit={handleSubmit(onSubmit)}>
            <div className={styles["form-row"]}>
              <Label htmlFor={"password"} className={styles["form-label"]}>
                New password
              </Label>

              <div
                style={{
                  display: "flex",
                  flexDirection: "column",
                  width: "100%",
                }}
              >
                <Controller
                  control={control}
                  name="newPassword"
                  render={({ field }) => (
                    <PasswordInput
                      id={"password"}
                      className={`${styles["form-input"]}`}
                      {...field}
                    />
                  )}
                />

                <div
                  style={{
                    marginTop: "calc(var(--spacing) * 2)",
                  }}
                  className="mt-2 text-sm font-medium text-red-500"
                >
                  {errors?.newPassword?.message}
                </div>
              </div>
            </div>

            <div className={styles["form-row"]}>
              <Label htmlFor={"password"} className={styles["form-label"]}>
                Confirm password
              </Label>

              <div
                style={{
                  display: "flex",
                  flexDirection: "column",
                  width: "100%",
                }}
              >
                <Controller
                  control={control}
                  name="confirmPassword"
                  render={({ field }) => (
                    <PasswordInput
                      id={"password"}
                      className={`${styles["form-input"]}`}
                      {...field}
                    />
                  )}
                />

                <div
                  style={{
                    width: "222px",
                    height: "20px",
                    marginTop: "calc(var(--spacing) * 2)",
                  }}
                  className="mt-2 text-sm font-medium text-red-500"
                >
                  {errors?.confirmPassword?.message}
                </div>
              </div>
            </div>

            <div className={styles["login-btn-wrapper"]}>
              <Button
                type="submit"
                className={styles["login-btn"]}
                disabled={
                  Object.keys(errors).length > 0 ||
                  !getValues("newPassword") ||
                  isLoading
                }
              >
                Save
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
