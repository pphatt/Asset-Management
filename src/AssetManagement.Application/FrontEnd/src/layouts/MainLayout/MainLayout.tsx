import MainHeader from '../core/MainHeader';
import MainNavigation from '../core/MainNavigation';

interface Props {
  children?: React.ReactNode;
}

export default function MainLayout({ children }: Props) {
  return (
    <>
      <MainHeader />
      <div className="flex mt-10 p-4 mx-auto">
        <MainNavigation />
        <main className="p-6 w-full">{children}</main>
      </div>
    </>
  );
}
