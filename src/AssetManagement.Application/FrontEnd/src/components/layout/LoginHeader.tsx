import { NashtechLogo } from "@/assets/NashtechLogo";
import { HTMLAttributes } from "react";

interface LoginHeaderProps extends HTMLAttributes<HTMLDivElement> { }

export default function LoginHeader({ ...props }: LoginHeaderProps) {
  return (
    <div className="flex h-[80px] bg-[#D90D1E]" {...props}>
      <div className="flex items-center w-full p-[8px] mx-[135px]">
        <NashtechLogo className="bg-white w-fit h-full p-[2px]" />
        <span className="text-white flex items-center pl-[20px] ml-[15px] text-[25px] font-bold">
          Online Asset Management
        </span>
      </div>
    </div>
  );
}
