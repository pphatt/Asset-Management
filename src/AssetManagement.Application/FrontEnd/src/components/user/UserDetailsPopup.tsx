import type React from "react"
import { IUserDetails } from "@/types/user.type";
import { X } from "lucide-react"
import { getGenderDisplay, getLocationDisplay, getUserTypeDisplay } from "@/utils/enumConvert";

interface UserDetailsPopupProps {
  user: IUserDetails | null
  isOpen: boolean
  onClose: () => void
}

const UserDetailsPopup: React.FC<UserDetailsPopupProps> = ({ user, isOpen, onClose }) => {
  if (!isOpen || !user) return null

  const formatDate = (dateString: string): string => {
    if (!dateString) return '';
    const [day, month, year] = dateString.split('/').map(Number);
    const date = new Date(year, month - 1, day);
    if (
      isNaN(date.getTime()) ||
      date.getDate() !== day ||
      date.getMonth() !== month - 1 ||
      date.getFullYear() !== year
    ) {
      return 'Invalid date';
    }
    return date.toLocaleDateString('en-GB');
  };
  return (

    <div className="fixed inset-0 bg-opacity-5 border-black-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-md shadow-lg w-full max-w-md relative">
        <div className="flex justify-between items-center p-4 border bg-tertiary rounded-t-md border-black text-white">
          <h2 className="text-xl font-semibold text-primary">Detailed User Information</h2>
          <button onClick={onClose} className="text-primary hover:text-dark-200 focus:outline-none border-red rounded-md border-3" aria-label="Close">
            <X size={24} />
          </button>
        </div>
        <div className="p-6 border-1 border-black rounded-b-md">
          <div className="grid grid-cols-2 gap-4">
            <div className="text-gray-600">Staff Code</div>
            <div>{user.staffCode}</div>

            <div className="text-gray-600">Full Name</div>
            <div>{`${user.firstName} ${user.lastName}`}</div>

            <div className="text-gray-600">Username</div>
            <div>{user.username}</div>

            {user.dateOfBirth && (
              <>
                <div className="text-gray-600">Date of Birth</div>
                <div>{user.dateOfBirth}</div>
              </>
            )}

            {user.gender && (
              <>
                <div className="text-gray-600">Gender</div>
                <div>{getGenderDisplay(user.gender)}</div>
              </>
            )}

            <div className="text-gray-600">Joined Date</div>
            <div>{formatDate(user.joinedDate)}</div>

            <div className="text-gray-600">Type</div>
            <div>{getUserTypeDisplay(user.type)}</div>

            {user.location && (
              <>
                <div className="text-gray-600">Location</div>
                <div>{getLocationDisplay(user.location)}</div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

export default UserDetailsPopup