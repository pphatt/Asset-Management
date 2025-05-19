import TableProvider from "@/stores/table-context";
import config from "./../../constant/config";
import { DataTable } from "@/components/ui/table/table";
import { useAccountFilter } from "./test";
import { columns } from "./column";

export default function Login() {
  console.log(config.baseURL);

  const filter = useAccountFilter({
    initSearch: "",
    initSort: [],
  });

  return <TableProvider>
    <div className="text-center bg-secondary">{config.baseURL}</div>
    <DataTable
      columns={columns}
      title="Title"
      filter={filter}
    />
  </TableProvider>
}
