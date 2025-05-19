import UserList from '../../components/user/UserList';

export default function User() {
  return (
    <div className="w-full max-w-5xl mx-auto px-4 py-6">
      <h2 className="text-primary text-xl font-normal mb-5">User List</h2>
      <UserList />
    </div>
  );
}
