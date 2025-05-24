import { adminNavigationItems, staffNavigationItems } from "@/constants/menu";
import { useAppContext } from "@/hooks/use-app-context";
import { matchPath, useLocation } from "react-router-dom";

export default function MainHeader() {
    const location = useLocation();
    const currentPath = location.pathname;
    const { profile } = useAppContext();
    const navigationItems = profile?.type === "Admin" ? adminNavigationItems : staffNavigationItems;

    const getHeaderTitle = () => {
        return navigationItems.find((item) => matchPath({ path: item.path, end: true }, currentPath))?.title;
    };

    return (
        <header className="bg-red-600 text-white py-4 px-6">
            <h1 className="text-xl font-medium">{getHeaderTitle()}</h1>
        </header>
    );
}