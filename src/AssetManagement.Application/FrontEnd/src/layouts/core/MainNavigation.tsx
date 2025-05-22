import NashtechLogo from '@/assets/nashtech-logo';
import { navigationItems } from '@/constants/menu';
import { Link, useLocation } from 'react-router-dom';

export default function MainNavigation() {
  const location = useLocation();
  const currentPath = location.pathname;

  return (
    <div className="h-screen w-[25%]">
      <div className="p-4 border-b border-gray-200">
        <div className="flex items-start flex-col gap-4">
          <div className="h-[80px]">
            <NashtechLogo className="w-full h-full" />
          </div>
          <div className="text-primary font-bold text-center">Online Asset Management</div>
        </div>
      </div>

      <nav className="mt-4 flex flex-col gap-[0.1rem]">
        {navigationItems.filter(item => item.showInNav).map((item, index) => (
          <Link
            key={index}
            to={item.path}
            className={`font-bold block px-4 py-4 transition-colors ${currentPath === item.path ? 'bg-primary text-white' : 'text-quaternary bg-gray-200'
              }`}
          >
            {item.title}
          </Link>
        ))}
      </nav>
    </div>
  );
}
