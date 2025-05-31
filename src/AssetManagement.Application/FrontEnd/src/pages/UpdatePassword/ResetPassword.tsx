import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { Label } from "@/components/ui/Label";
import { Controller, useForm } from "react-hook-form";
import { PasswordInput } from "@/components/layout/PasswordInput";
import { Schema, schema } from "@/utils/rules";
import { yupResolver } from "@hookform/resolvers/yup";
import { useMutation } from "@tanstack/react-query";
import { PasswordUpdateRequest } from "@/types/auth.type.ts";
import authApi from "@/apis/auth.api.ts";
import { jwtDecode, JwtPayload } from "jwt-decode";
import { setCookie } from "@/utils/auth.ts";
import { useAppContext } from "@/hooks/useAppContext";
import { Button } from "@/components/ui/Button";
import styles from "@/pages/UpdatePassword/styles.module.css";
import { useNavigate } from "react-router-dom";
import { useState } from "react";

type FormData = Pick<Schema, "newPassword" | "password" | "confirmPassword">;
const passwordSchema = schema.pick([
  "newPassword",
  "password",
  "confirmPassword",
]);

type JWTPayload = {
  "http://schemas.microsoft.com/ws/2008/06/jexp": string;
} & JwtPayload;

export default function ResetPassword() {
  const navigate = useNavigate();
  const [result, setResult] = useState(false);
  const [messageError, setMessageError] = useState("");

  const {
    handleSubmit,
    control,
    formState: { errors },
    watch,
  } = useForm({
    mode: "all",
    resolver: yupResolver(passwordSchema),
    defaultValues: { newPassword: "", password: "", confirmPassword: "" },
  });

  const { setIsAuthenticated, setProfile } = useAppContext();

  const { isPending: isLoading, mutate } = useMutation({
    mutationFn: (body: PasswordUpdateRequest) => authApi.changePassword(body),
    onError: () => {
      setMessageError("Password is incorrect");
    },
  });

  // Watch the values of the password fields
  const password = watch("password");
  const newPassword = watch("newPassword");
  const confirmPassword = watch("confirmPassword");

  const onSubmit = (data: FormData) => {
    const { newPassword, password } = data;
    mutate(
      { newPassword, password },
      {
        onSuccess(info) {
          const user = data && info?.data?.userInfo;
          const accessToken = data && info?.data.accessToken;

          const decode = jwtDecode<JWTPayload>(accessToken);

          const {
            id,
            staffCode,
            firstName,
            lastName,
            username,
            type,
            joinedDate,
            isPasswordUpdated,
          } = user;

          setProfile({
            id,
            staffCode,
            firstName,
            lastName,
            username,
            type,
            joinedDate,
            isPasswordUpdated,
          });

          const expiredTime = Number.parseInt(
            decode["http://schemas.microsoft.com/ws/2008/06/jexp"]
          );

          setCookie("access_token", accessToken, expiredTime);

          setCookie("profile", JSON.stringify(user), expiredTime);

          setIsAuthenticated(true);

          setResult(true);
        },
      }
    );
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center z-[100] backdrop-blur-[3px]">
      <Card className="gap-0 p-0 border-0 rounded-[8px] shadow-none w-[600px]">
        <CardHeader className={styles["card-header"]}>
          <CardTitle
            className={`flex justify-between items-center text-lg tracking-tight w-full ${styles["card-title"]}`}
          >
            <span>Change password</span>
          </CardTitle>
        </CardHeader>

        <CardContent className={styles["card-content"]}>
          {!result ? (
            <form className="relative" onSubmit={handleSubmit(onSubmit)}>
              <div className={styles["form-row"]}>
                <Label htmlFor={"password"} className={styles["form-label"]}>
                  Old password
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
                    name="password"
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
                    {errors?.password?.message || messageError}
                  </div>
                </div>
              </div>

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
                      width: "222px",
                      height: "20px",
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
                    !password ||
                    !newPassword ||
                    !confirmPassword ||
                    isLoading
                  }
                >
                  Save
                </Button>
                <Button
                  type="button"
                  className="ml-2 bg-white text-black border"
                  variant={"ghost"}
                  onClick={() =>
                    navigate(`${location.pathname}`, { replace: true })
                  }
                >
                  Cancel
                </Button>
              </div>
            </form>
          ) : (
            <div>
              <div className="text-center mt-2.5">
                Your password has been changed successfully!
              </div>
              <Button
                type="button"
                className="ml-2 bg-white text-black border float-end mt-10"
                variant={"ghost"}
                onClick={() =>
                  navigate(`${location.pathname}`, { replace: true })
                }
              >
                Close
              </Button>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
