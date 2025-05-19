import path from "./path";

interface NavigationItem {
    title: string;
    path: string;
}

export const navigationItems: NavigationItem[] = [
    { title: "Home", path: path.home },
    { title: "Manage User", path: path.user },
    { title: "Manage Asset", path: path.asset },
    { title: "Manage Assignment", path: path.assignment },
    { title: "Request for Returning", path: path.request },
    { title: "Report", path: path.report },
];