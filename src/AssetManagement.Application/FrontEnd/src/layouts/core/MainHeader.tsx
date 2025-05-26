import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { IUser } from "@/types/user.type";
import { clearCookieSession, getProfileFromCookie } from "@/utils/auth";
import { DropdownMenu } from "../../components/ui/dropdown";
import { adminNavigationItems, staffNavigationItems } from "@/constants/menu";

export default function MainHeader() {
  const location = useLocation();
  const navigate = useNavigate();
  const currentPath = location.pathname;
  const profileUser: IUser | null = getProfileFromCookie();
  const [, setSelectedValue] = useState<string>("");

  const dropdownItems = [
    {
      label: "Change Password",
      value: "changePassword",
      icon: (
        <svg
          className="w-4 h-4"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="2"
            d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
          />
        </svg>
      ),
    },
    {
      label: "Logout",
      value: "logout",
      icon: (
        <svg
          className="w-4 h-4"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="2"
            d="M17 16l4-4m0 0l-4-4m4 4H7m5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h3a3 3 0 013 3v1"
          />
        </svg>
      ),
    },
  ];

  const handleDropdownChange = (value: string) => {
    setSelectedValue(value);
    if (value === "logout") {
      try {
        clearCookieSession();
        window.location.replace("/login");
      } catch {
        navigate("/login", { replace: true }); // Navigate even if cookie clearing fails
      }
    } else if (value === "changePassword") {
      navigate(`${location.pathname}?changePassword=true`, { replace: true });
    }
  };

  const navigationItems = profileUser?.type === "Admin" ? adminNavigationItems : staffNavigationItems;

  const getHeaderTitle = () => {
    return navigationItems.find((item) => item.path === currentPath)?.title || "Dashboard";
  };

  return (
    <header className="bg-primary text-white py-4 px-6 flex flex-row items-center">
      <h1 className="text-xl font-medium basis-1/2">{getHeaderTitle()}</h1>
      <div className="basis-1/2 text-right flex items-center justify-end">
        <DropdownMenu
          items={dropdownItems}
          onChange={handleDropdownChange}
          title={profileUser ? `${profileUser.firstName} ${profileUser.lastName}` : "Guest"}
        />
      </div>
    </header>
  );
}