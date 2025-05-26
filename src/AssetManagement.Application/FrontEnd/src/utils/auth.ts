import { IUser } from "src/types/user.type";

export const cookieEventTarget = new EventTarget();

export const setCookie = (name: string, value: string, durations: number) => {
  // Validate inputs
  if (!name || !value) {
    console.warn('setCookie: Name and value must not be empty');
    return;
  }
  if (durations < 0) {
    console.warn('setCookie: Duration must not be negative');
    return;
  }

  // Calculate expiration in milliseconds
  const expires = durations === 0 ? '' : `expires=${new Date(Date.now() + durations).toUTCString()}`;

  // Build cookie string
  const cookieString = [
    `${name}=${encodeURIComponent(value)}`,
    expires,
    'path=/',
    'SameSite=None; Secure',
  ]
    .filter(Boolean)
    .join('; ');

  document.cookie = cookieString;
};

const getCookie = (name: string) => {
  const nameEQ = `${name}=`;
  const cookies = document.cookie.split(";");
  for (let i = 0; i < cookies.length; i++) {
    const cookie = cookies[i].trim();
    if (cookie.indexOf(nameEQ) === 0) {
      return cookie.substring(nameEQ.length);
    }
  }
  return "";
};

export const clearCookieSession = () => {
  // Get all cookies as a string
  const cookies = document.cookie.split(';');

  // Iterate through each cookie
  cookies.forEach((cookie) => {
    // Extract the cookie name
    const cookieName = cookie.split('=')[0].trim();
    // Set the cookie's expiration to a past date to delete it
    document.cookie = `${cookieName}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/`;
  });

  // Optionally dispatch an event to notify listeners
  const event = new Event('cookiesCleared');
  cookieEventTarget.dispatchEvent(event);
};

export const getAccessTokenFromCookie = () => getCookie("access_token") || "";

export const getProfileFromCookie = (): IUser | null => {
  const result = getCookie("profile");
  return result ? JSON.parse(decodeURIComponent(result)) : null;
};

export const isAuthenticated = () => {
  return !!getCookie("access_token");
};
