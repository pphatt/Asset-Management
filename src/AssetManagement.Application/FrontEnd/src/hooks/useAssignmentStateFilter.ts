import { ASSIGNMENT_STATE } from "@/constants/assignment-params";
import useQueryConfig from "@/hooks/useAssignmentQuery";
import { querySchema } from "@/utils/rules";
import { yupResolver } from "@hookform/resolvers/yup";
import { useMemo } from "react";
import { useForm } from "react-hook-form";
import { createSearchParams, useNavigate } from "react-router-dom";

type FormData = {
  states: string[];
};

const nameSchema = querySchema.pick(["states"]);

// Format state for display
const formatStateForDisplay = (state: string) => {
  if (state === "WaitingForAcceptance")
    return ASSIGNMENT_STATE.WAITING_FOR_ACCEPTANCE;
  if (state === "WaitingForReturning")
    return ASSIGNMENT_STATE.WAITING_FOR_RETURNING;
  return state;
};

// Format state for API
const formatStateForApi = (state: string) => {
  if (state === ASSIGNMENT_STATE.WAITING_FOR_ACCEPTANCE)
    return "WaitingForAcceptance";
  if (state === ASSIGNMENT_STATE.WAITING_FOR_RETURNING)
    return "WaitingForReturning";
  return state;
};

interface Props {
  path: string;
}

export default function useAssignmentStateFilter({ path }: Props) {
  const queryConfig = useQueryConfig();
  const navigate = useNavigate();

  // Parse the current state from URL
  const initialStates = useMemo(() => {
    if (!queryConfig.states) return [];
    return Array.isArray(queryConfig.states)
      ? queryConfig.states.map(formatStateForDisplay)
      : [queryConfig.states].map(formatStateForDisplay);
  }, [queryConfig.states]);

  const { setValue } = useForm<FormData>({
    defaultValues: {
      states: initialStates,
    },
    resolver: yupResolver(nameSchema),
  });

  // Handle individual state checkbox changes
  const handleStateChange = (stateValue: string) => {
    const currentStates = [...initialStates];
    const stateIndex = currentStates.indexOf(stateValue);

    if (stateIndex === -1) {
      // Add state if not already selected
      currentStates.push(stateValue);
    } else {
      // Remove state if already selected
      currentStates.splice(stateIndex, 1);
    }

    // Update form value
    setValue("states", currentStates);

    // Convert states to API format for URL
    const apiStates = currentStates.map(formatStateForApi);

    const config = {
      ...queryConfig,
      states: apiStates,
      pageNumber: "1",
    };

    navigate({
      pathname: path,
      search: createSearchParams(config).toString(),
    });
  };

  // Check if a state is currently selected
  const isStateSelected = (stateValue: string) => {
    return initialStates.includes(stateValue);
  };

  return {
    handleStateChange,
    isStateSelected,
    selectedStates: initialStates,
  };
}
