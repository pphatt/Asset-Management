import type React from "react";
import { X } from "lucide-react";
import { IAssetDetails } from "@/types/asset.type.ts";
import AssetDetailsHistoryTable from "@/components/asset/AssetDetailsHistoryTable.tsx";

interface AssetDetailsPopupProps {
  asset: IAssetDetails | null;
  isOpen: boolean;
  onClose: () => void;
}

const AssetDetailsPopup: React.FC<AssetDetailsPopupProps> = ({
  asset,
  isOpen,
  onClose,
}) => {
  if (!isOpen || !asset) return null;

  return (
    <div className="fixed inset-0 bg-opacity-5 border-black-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-md shadow-lg w-full max-w-[40rem] relative">
        <div className="flex justify-between items-center p-4 border bg-tertiary rounded-t-md border-black text-white">
          <h2 className="text-xl font-semibold text-primary">
            Detailed Asset Information
          </h2>
          <button
            onClick={onClose}
            className="text-primary hover:text-dark-200 focus:outline-none border-red rounded-md border-3"
            aria-label="Close"
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-6 border-1 border-black rounded-b-md">
          <div
            style={{ gridTemplateColumns: "1fr 4fr" }}
            className="grid gap-4"
          >
            <div className="text-gray-600">Asset Code</div>
            <div>{asset.code}</div>

            <div className="text-gray-600">Asset Name</div>
            <div>{asset.name}</div>

            <div className="text-gray-600">Category</div>
            <div>{asset.categoryName}</div>

            <div className="text-gray-600">Installed Date</div>
            <div>{asset.installedDate}</div>

            <div className="text-gray-600">State</div>
            <div>{asset.state}</div>

            <div className="text-gray-600">Location</div>
            <div>{asset.location}</div>

            <div className="text-gray-600">Specification</div>
            <div>{asset.specification}</div>

            <div className="text-gray-600">History</div>
            <AssetDetailsHistoryTable assetDetailsHistory={asset.assignments} />
          </div>
        </div>
      </div>
    </div>
  );
};

export default AssetDetailsPopup;
