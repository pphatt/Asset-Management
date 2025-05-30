import useQueryParams from "@/hooks/useQueryParams";
import { QueryConfig } from "@/pages/Assignment/Assignment";
import { isUndefined, omitBy } from "lodash";

export default function useAssignmentQuery() {
  const queryParams = useQueryParams();
  const queryConfig: QueryConfig = omitBy(
    {
      searchTerm: queryParams.searchTerm || "",
      pageNumber: queryParams.pageNumber || "1",
      pageSize: queryParams.pageSize || "5",
      sortBy: queryParams.sortBy,
      state: queryParams.state,
      date: queryParams.date,
    },
    isUndefined
  );
  return queryConfig;
}
