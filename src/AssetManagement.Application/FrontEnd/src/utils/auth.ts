import { User } from "src/types/user.type";

export const cookieEventTarget = new EventTarget();

const getCookie = (name: string) => {
  const nameEQ = `${name}=`;
  const cookies = document.cookie.split(";");
  for (let i = 0; i < cookies.length; i++) {
    let cookie = cookies[i].trim();
    if (cookie.indexOf(nameEQ) === 0) {
      return cookie.substring(nameEQ.length);
    }
  }
  return "";
};

export const clearCookieSession = () => {
  const clearSessionEvent = new Event("clearSession");
  cookieEventTarget.dispatchEvent(clearSessionEvent);
};

export const getAccessTokenFromCookie = () => getCookie("access_token") || "";

export const getProfileFromCookie = (): User | null => {
  const result = getCookie("profile");
  return result ? JSON.parse(decodeURIComponent(result)) : null;
};

export const isAuthenticated = () => {
  return !!getCookie("access_token");
};
