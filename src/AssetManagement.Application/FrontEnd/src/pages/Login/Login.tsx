import authApi from '@/apis/auth.api.ts';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/forms/Input';
import { Label } from '@/components/ui/Label';
import { useAppContext } from '@/hooks/useAppContext';
import { LoginRequest } from '@/types/auth.type.ts';
import { yupResolver } from '@hookform/resolvers/yup';
import { useMutation } from '@tanstack/react-query';
import { Controller, useForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import * as yup from 'yup';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

import { PasswordInput } from '@/components/layout/PasswordInput';
import { setCookie } from '@/utils/auth.ts';
import { jwtDecode, JwtPayload } from 'jwt-decode';
import styles from './styles.module.css';

const schema = yup.object().shape({
  username: yup.string().required('Username is required'),
  password: yup.string().required('Password is required'),
});

type JWTPayload = {
  exp: string;
} & JwtPayload;

export default function Login() {
  const {
    handleSubmit,
    control,
    watch,
    formState: { errors },
  } = useForm({
    mode: 'all',
    resolver: yupResolver(schema),
    defaultValues: { username: '', password: '' },
  });

  const username = watch('username');
  const password = watch('password');

  const { setIsAuthenticated, setProfile } = useAppContext();

  const { isPending: isLoading, mutate } = useMutation({
    mutationFn: (body: LoginRequest) => authApi.loginAccount(body),
    onError: (err: any) => {
      const errMsg = err.response.data.errors;
      toast.error(errMsg[0] || 'error when login');
    },
  });

  const onSubmit = (data: LoginRequest) => {
    mutate(data, {
      onSuccess(info) {
        const user = data && info?.data?.userInfo;
        const accessToken = data && info?.data.accessToken;

        const decode = jwtDecode<JWTPayload>(accessToken);

        const { id, staffCode, firstName, lastName, username, type, joinedDate, isPasswordUpdated } = user;

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

        const expInMs = Number.parseInt(decode["exp"]) * 1000; // JWT exp to ms
        const currentTime = Date.now(); // current time in ms


        // Cookie lifetime in seconds
        const maxAge = Math.floor((expInMs - currentTime) / 1000);

        setCookie("access_token", accessToken, maxAge);

        setCookie("profile", JSON.stringify(user), maxAge);

        setIsAuthenticated(true);
        toast.success('Login successfully!');
      },
    });
  };

  return (
    <Card className="gap-0 p-0 border-0 shadow-none">
      <CardHeader className={styles['card-header']}>
        <CardTitle className={`text-lg tracking-tight ${styles['card-title']}`}>Welcome to Online Asset Management</CardTitle>
      </CardHeader>

      <CardContent className={styles['card-content']}>
        <form onSubmit={handleSubmit(onSubmit)} className="relative">
          <div className={styles['form-row']}>
            <Label htmlFor={'username'} className={styles['form-label']}>
              Username <span style={{ color: 'red' }}>*</span>
            </Label>

            <div style={{ display: 'flex', flexDirection: 'column' }}>
              <Controller
                control={control}
                name="username"
                render={({ field }) => <Input id={'username'} className={`${styles['form-input']}`} {...field} />}
              />

              <div
                style={{
                  width: '222px',
                  height: '20px',
                  margin: '0 25px 0 0',
                  marginTop: 'calc(var(--spacing) * 2)',
                }}
                className="mt-2 text-sm font-medium text-red-500"
              >
                {errors?.username?.message}
              </div>
            </div>
          </div>

          <div className={styles['form-row']}>
            <Label htmlFor={'password'} className={styles['form-label']}>
              Password <span style={{ color: 'red' }}>*</span>
            </Label>

            <div style={{ display: 'flex', flexDirection: 'column' }}>
              <Controller
                control={control}
                name="password"
                render={({ field }) => <PasswordInput id={'password'} className={`${styles['form-input']}`} {...field} />}
              />

              <div
                style={{
                  width: '222px',
                  height: '20px',
                  margin: '0 25px 0 0',
                  marginTop: 'calc(var(--spacing) * 2)',
                }}
                className="mt-2 text-sm font-medium text-red-500"
              >
                {errors?.password?.message}
              </div>
            </div>
          </div>

          <div className={styles['login-btn-wrapper']}>
            <Button type={'submit'} className={styles['login-btn']} disabled={Object.keys(errors).length > 0 || !username || !password || isLoading}>
              Login
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
