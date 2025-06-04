import { useSearchParams } from "react-router-dom";

export default function useQueryParams() {
  const [searchParams] = useSearchParams();
  const params: Record<string, string | string[]> = {};

  // Process all search parameters
  searchParams.forEach((value, key) => {
    // Check if this parameter already exists (for handling multiple values)
    if (params[key]) {
      // If it's already an array, add the new value
      if (Array.isArray(params[key])) {
        (params[key] as string[]).push(value);
      } else {
        // If it's a string, convert to array with both values
        params[key] = [params[key] as string, value];
      }
    } else {
      // First occurrence of this parameter
      params[key] = value;
    }
  });

  return params;
}
