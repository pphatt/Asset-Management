import path from "@/constants/path";
import useQueryConfig from "@/hooks/useAssignmentQuery";
import { querySchema, QuerySchema } from "@/utils/rules";
import { yupResolver } from "@hookform/resolvers/yup";
import { useForm } from "react-hook-form";
import { createSearchParams, useNavigate } from "react-router-dom";

type FormData = Pick<QuerySchema, "searchName">;
const nameSchema = querySchema.pick(["searchName"]);

export default function useAssignmentSearch() {
  const queryConfig = useQueryConfig();
  const navigate = useNavigate();

  const { register, handleSubmit } = useForm<FormData>({
    defaultValues: {
      searchName: "",
    },
    resolver: yupResolver(nameSchema),
  });

  const onSubmitSearch = handleSubmit((data) => {
    const config = {
      ...queryConfig,
      searchTerm: data.searchName,
    };
    navigate({
      pathname: path.assignment,
      search: createSearchParams(config).toString(),
    });
  });

  return { register, onSubmitSearch };
}
