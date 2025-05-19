export const Input = (props: React.InputHTMLAttributes<HTMLInputElement>) => {
    return (
        <input
            {...props}
            className={`border-2 border-gray-400 rounded-md p-2 focus:outline-none focus:ring-0 ${props.className}`}
        />
    )
}