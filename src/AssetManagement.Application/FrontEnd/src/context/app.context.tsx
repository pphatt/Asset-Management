import { createContext, useState } from "react";
import { User } from "./../types/user.type";
import { getProfileFromCookie, isAuthenticated } from "./../utils/auth";

interface AppContextInterface {
  isAuthenticated: boolean;
  setIsAuthenticated: React.Dispatch<React.SetStateAction<boolean>>;
  profile: User | null;
  setProfile: React.Dispatch<React.SetStateAction<User | null>>;
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
  const [profile, setProfile] = useState<User | null>(intialAppContext.profile);

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
