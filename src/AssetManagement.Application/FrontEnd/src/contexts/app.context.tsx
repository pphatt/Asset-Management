import { createContext, useState } from "react";
import { IUser } from "./../types/user.type";
import { getProfileFromCookie, isAuthenticated } from "./../utils/auth";

interface AppContextInterface {
  isAuthenticated: boolean;
  setIsAuthenticated: React.Dispatch<React.SetStateAction<boolean>>;
  profile: IUser | null;
  setProfile: React.Dispatch<React.SetStateAction<IUser | null>>;
  reset: () => void;
}

const intialAppContext: AppContextInterface = {
  isAuthenticated: isAuthenticated(),
  setIsAuthenticated: () => null,
  profile: getProfileFromCookie(),
  setProfile: () => null,
  reset: () => null,
};

export const AppContext = createContext<AppContextInterface>(intialAppContext);

export const AppProvider = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(
    intialAppContext.isAuthenticated
  );
  const [profile, setProfile] = useState<IUser | null>(intialAppContext.profile);

  const reset = () => {
    setIsAuthenticated(false);
    setProfile(null);
  };

  return (
    <AppContext.Provider
      value={{
        isAuthenticated,
        setIsAuthenticated,
        profile,
        setProfile,
        reset,
      }}
    >
      {children}
    </AppContext.Provider>
  );
};
