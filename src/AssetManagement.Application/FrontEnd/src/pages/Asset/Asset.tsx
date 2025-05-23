// import UserList from "@/components/user/UserList";

import AssetList from "@/components/asset/AssetList";


export default function Asset() {
  return (
      <div className="w-full max-w-5xl mx-auto px-4 py-6">
          <h2 className="text-primary text-xl font-normal mb-5">Asset List</h2>
          <AssetList />
      </div>
  );
}
