import * as yup from "yup";
import LoginHeader from "@/components/layout/login-header.tsx";
import { PASSWORD_REG } from "@/constants/validate-rules";
import { Controller, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import { useAppContext } from "@/hooks/use-app-context.tsx";
import { useMutation } from "@tanstack/react-query";
import { LoginRequest } from "@/types/auth.type.ts";
import { toast } from "react-toastify";
import authApi from "@/apis/auth.api.ts";
import { Button } from "@/components/ui/button.tsx";
import { Input } from "@/components/ui/forms/input.tsx";
import { Label } from "@/components/ui/label.tsx";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import styles from "./styles.module.css";
import { PasswordInput } from "@/components/layout/password-input.tsx";
import { useNavigate } from "react-router-dom";

const schema = yup.object().shape({
  username: yup.string().required("Username is required"),
  password: yup
    .string()
    .required("Password is required")
    .matches(PASSWORD_REG, "Password Format Error"),
});

export default function Login() {
  const navigate = useNavigate();

  const {
    handleSubmit,
    control
  } = useForm({
    mode: "onBlur",
    resolver: yupResolver(schema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const { setIsAuthenticated, setProfile } = useAppContext();

  const { isPending: isLoading, mutate } = useMutation({
    mutationFn: (body: LoginRequest) => authApi.loginAccount(body),
    onError: (err: any) => {
      const errMsg = err.response.data.error.message;
      toast.error(errMsg || "error when login");
    },
  });

  const onSubmit = (data: LoginRequest) => {
    mutate(data, {
      onSuccess(info) {
        // Core Template
        const user = data && (info?.data?.userInfo)

        const { staffCode, firstName, lastName, username, type, joinedDate } = user
        setProfile({ staffCode, firstName, lastName, username, type, joinedDate })
        setIsAuthenticated(true);
        toast.success("Login successfully!");
        navigate("/");
      },
    });
  };

  return (
    <div className="h-full">
      <LoginHeader />

      <div
        className="bg-primary-foreground container grid h-[100%] max-w-none"
        style={{ marginTop: 100 }}
      >
        <div
          className="mx-auto flex w-full flex-col space-y-2 py-8 sm:w-[480px] sm:p-8"
          style={{ width: 580 }}
        >
          <Card className="gap-0 p-0 border-0 shadow-none">
            <CardHeader className={styles["card-header"]}>
              <CardTitle
                className={`text-lg tracking-tight ${styles["card-title"]}`}
              >
                Welcome to Online Asset Management
              </CardTitle>
            </CardHeader>

            <CardContent className={styles["card-content"]}>
              <form onSubmit={handleSubmit(onSubmit)} className="relative">
                <div className={styles["form-row"]}>
                  <Label htmlFor={"username"} className={styles["form-label"]}>
                    Username
                    <span style={{ color: "red" }}>*</span>
                  </Label>

                  <Controller
                    control={control}
                    name="username"
                    render={({ field }) => (
                      <Input
                        id={"username"}
                        className={`${styles["form-input"]}`}
                        {...field}
                      />
                    )}
                  />

                  {/*<div className='h-3 mt-2 text-sm font-medium text-red-500'>*/}
                  {/*  {errors?.username && errors?.username?.message}*/}
                  {/*</div>*/}
                </div>

                <div className={styles["form-row"]}>
                  <Label htmlFor={"password"} className={styles["form-label"]}>
                    Password
                    <span style={{ color: "red" }}>*</span>
                  </Label>

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

                  {/*<div className='h-3 mt-2 text-sm font-medium text-red-500'>*/}
                  {/*  {errors?.password && errors?.password?.message}*/}
                  {/*</div>*/}
                </div>

                <div className={styles["login-btn-wrapper"]}>
                  <Button type={"submit"} className={styles["login-btn"]} disabled={isLoading}>
                    Login
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
