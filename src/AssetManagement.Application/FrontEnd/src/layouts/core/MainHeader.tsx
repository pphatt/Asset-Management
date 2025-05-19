import { navigationItems } from "@/constants/menu";
import { useLocation } from "react-router-dom";

export default function MainHeader() {
    const location = useLocation();
    const currentPath = location.pathname;

    const getHeaderTitle = () => {
        return navigationItems.find((item) => item.path === currentPath)?.title
    };

    return (
        <header className="bg-red-600 text-white py-4 px-6">
            <h1 className="text-xl font-medium">{getHeaderTitle()}</h1>
        </header>
    );
}