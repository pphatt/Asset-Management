import { AppContext } from "@/context/app.context";
import MainHeader from "../core/MainHeader";
import MainNavigation from "../core/MainNavigation";
import { useContext } from "react";
import FirstChangePassword from "@/pages/UpdatePassword/FirstChangePassword";
import { useLocation } from "react-router-dom";
import ResetPassword from "@/pages/UpdatePassword/ResetPassword";
interface Props {
  children?: React.ReactNode;
}
export default function MainLayout({ children }: Props) {
  const { profile } = useContext(AppContext);
  const location = useLocation();
  const queryParams = new URLSearchParams(location.search);
  const changePassword = queryParams.get('changePassword');

  return (
    <>
      <MainHeader />
      <div className="flex mt-10 p-4 mx-auto">
        {(profile?.isPasswordUpdated == null ||
          !profile?.isPasswordUpdated) && <FirstChangePassword />}
        {changePassword == "true" && <ResetPassword />}
        <MainNavigation /> <main className="p-6 w-full">{children}</main>
      </div>
    </>
  );
}
