import LoginHeader from "@/components/layout/login-header.tsx";

interface Props {
  children?: React.ReactNode;
}

export default function LoginLayout({children}: Props) {
  return (
    <div className="h-full">
      <LoginHeader/>

      <div
        className="bg-primary-foreground container grid h-[100%] max-w-none"
        style={{ marginTop: 100 }}
      >
        <div
          className="mx-auto flex w-full flex-col space-y-2 py-8 sm:w-[480px] sm:p-8"
          style={{ width: 580 }}
        >
          {children}
        </div>
      </div>
    </div>
  )
}
