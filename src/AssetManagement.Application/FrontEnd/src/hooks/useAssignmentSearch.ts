import useQueryConfig from "@/hooks/useAssignmentQuery";
import { querySchema } from "@/utils/rules";
import { yupResolver } from "@hookform/resolvers/yup";
import { useForm } from "react-hook-form";
import { createSearchParams, useNavigate } from "react-router-dom";

// type FormData = { searchName: string | undefined };
const nameSchema = querySchema.pick(["searchName"]);

interface Props {
  path: string;
}

export default function useAssignmentSearch({ path }: Props) {
  const queryConfig = useQueryConfig();
  const navigate = useNavigate();

  const { register, handleSubmit } = useForm({
    defaultValues: {
      searchName: "",
    },
    resolver: yupResolver(nameSchema),
  });

  const onSubmitSearch = handleSubmit((data) => {
    const config = {
      ...queryConfig,
      searchTerm: data.searchName ?? "",
      pageNumber: "1",
    };
    navigate({
      pathname: path,
      search: createSearchParams(config).toString(),
    });
  });

  return { register, onSubmitSearch };
}
