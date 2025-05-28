import MainHeader from "../../components/layout/MainHeader";
import MainNavigation from "../../components/layout/MainNavigation";
import { useContext } from "react";
import FirstChangePassword from "@/pages/UpdatePassword/FirstChangePassword";
import { useLocation } from "react-router-dom";
import ResetPassword from "@/pages/UpdatePassword/ResetPassword";
import { AppContext } from "@/contexts/app.context";

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
