export const Input = ({
  className,
  type,
  ...props
}: React.ComponentProps<"input">) => {
  return (
    <input
      type={type}
      data-slot="input"
      className={`border-2 border-gray-400 rounded-md p-2 focus:outline-none focus:ring-0 ${className}`}
      {...props}
    />
  );
};
