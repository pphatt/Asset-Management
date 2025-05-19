import * as React from "react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Eye, EyeOff } from "lucide-react";
import { Input } from "@/components/ui/forms/input";

type PasswordInputProps = Omit<
  React.InputHTMLAttributes<HTMLInputElement>,
  "type"
>;

const PasswordInput = React.forwardRef<HTMLInputElement, PasswordInputProps>(
  ({ className, disabled, ...props }, ref) => {
    const [showPassword, setShowPassword] = React.useState(false);
    return (
      <div className={cn("relative rounded-md", className)}>
        <Input
          type={showPassword ? "text" : "password"}
          className="border-input placeholder:text-muted-foreground flex h-9 w-full rounded-md border bg-transparent px-3 py-1 text-sm shadow-xs transition-colors disabled:cursor-not-allowed disabled:opacity-50"
          style={{ paddingRight: 36, height: 40 }}
          ref={ref}
          disabled={disabled}
          {...props}
        />

        <Button
          type="button"
          disabled={disabled}
          variant="ghost"
          className="text-muted-foreground absolute top-1/2 right-1 h-9 w-9 -translate-y-1/2 rounded-md"
          onClick={() => setShowPassword((prev) => !prev)}
        >
          {showPassword ? <Eye size={18} /> : <EyeOff size={18} />}
        </Button>
      </div>
    );
  },
);
PasswordInput.displayName = "PasswordInput";

export { PasswordInput };
