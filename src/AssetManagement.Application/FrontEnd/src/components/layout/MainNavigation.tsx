import { NashtechLogo } from '@/assets/NashtechLogo';
import { NavLink } from 'react-router-dom';
import { adminNavigationItems, staffNavigationItems } from '@/constants/menu';
import { useAppContext } from '@/hooks/useAppContext';

export default function MainNavigation() {
  const { profile } = useAppContext();
  const navigationItems = profile?.type === "Admin" ? adminNavigationItems : staffNavigationItems;

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
          <NavLink
            key={index}
            to={item.path}
            end={false}
            className={({ isActive }) =>
              `font-bold block px-4 py-4 transition-colors
              ${isActive ? "bg-primary text-white" : "text-quaternary bg-gray-200"}
              `}
          >
            {item.title}
          </NavLink>
        ))}
      </nav>
    </div>
  );
}
