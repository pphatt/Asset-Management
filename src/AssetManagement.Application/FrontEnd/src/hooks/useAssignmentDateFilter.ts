import { useState, useCallback } from "react";
import { createSearchParams, useNavigate } from "react-router-dom";
import path from "@/constants/path";
import useQueryConfig from "@/hooks/useAssignmentQuery";
import { format, isSameDay } from "date-fns";

export default function useAssignmentDateFilter() {
  const queryConfig = useQueryConfig();
  const navigate = useNavigate();
  const [isDatePickerOpen, setIsDatePickerOpen] = useState(false);

  // Parse date from URL query if it exists
  const [selectedDate, setSelectedDate] = useState<Date | null>(() => {
    if (queryConfig.date) {
      const parsedDate = new Date(queryConfig.date);
      return isNaN(parsedDate.getTime()) ? null : parsedDate;
    }
    return null;
  });

  // Handle date selection with toggle behavior
  const handleDateChange = useCallback(
    (date: Date | null) => {
      // Toggle behavior: If same date selected twice, clear it
      if (date && selectedDate && isSameDay(date, selectedDate)) {
        setSelectedDate(null);

        const config = {
          ...queryConfig,
          date: "",
          pageNumber: "1",
        };

        navigate({
          pathname: path.assignment,
          search: createSearchParams(config).toString(),
        });
      } else {
        setSelectedDate(date);

        // Format date for API or remove if null
        const formattedDate = date ? format(date, "yyyy/MM/dd") : "";

        const config = {
          ...queryConfig,
          date: formattedDate,
          pageNumber: "1",
        };

        navigate({
          pathname: path.assignment,
          search: createSearchParams(config).toString(),
        });
      }

      // Keep the date picker open for better UX
      // This allows users to easily see the toggle action
      // Only close if explicitly clearing the date
      if (!date) {
        setIsDatePickerOpen(false);
      }
    },
    [selectedDate, queryConfig, navigate]
  );

  const toggleDatePicker = useCallback(() => {
    setIsDatePickerOpen((prev) => !prev);
  }, []);

  // Format date for display
  const displayDate = selectedDate ? format(selectedDate, "dd/MM/yyyy") : "";

  return {
    selectedDate,
    displayDate,
    handleDateChange,
    isDatePickerOpen,
    setIsDatePickerOpen,
    toggleDatePicker,
  };
}
