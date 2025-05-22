import path from "./path";

interface NavigationItem {
    title: string;
    path: string;
    showInNav: boolean;
}

export const navigationItems: NavigationItem[] = [
    { title: "Home", path: path.home, showInNav: true },
    { title: "Manage User", path: path.user, showInNav: true },
    { title: "Manage Asset", path: path.asset, showInNav: true },
    { title: "Manage Assignment", path: path.assignment, showInNav: true },
    { title: "Request for Returning", path: path.request, showInNav: true },
    { title: "Report", path: path.report, showInNav: true },
    { title: "Manage User > Create New User", path: path.userCreate, showInNav: false },
];