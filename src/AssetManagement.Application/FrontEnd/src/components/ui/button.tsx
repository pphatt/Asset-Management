import React from "react";

export const Button = (props: React.InputHTMLAttributes<HTMLInputElement>) => {
    return (
        <button type="button" className={`focus:outline-none btn-primary focus:ring-4 focus:ring-red-300 font-medium rounded-md text-sm px-5 py-2.5 me-2 dark:hover:bg-red-700 ${props.className}`}>
            {props.children}
        </button>
    )
}
