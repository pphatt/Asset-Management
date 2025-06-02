import { AssignmentTable } from "./AssignmentTable";

const MyAssignmentList: React.FC = () => {
    return (
        <div>
            <h1 className="text-primary font-bold mb-5">My Assignment</h1>
            <AssignmentTable />
        </div>
    )
}

export default MyAssignmentList;