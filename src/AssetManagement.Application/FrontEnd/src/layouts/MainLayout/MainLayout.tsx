interface Props {
  children?: React.ReactNode;
}

export default function MainLayout({ children }: Props) {
  return <div className="container">MainLayout{children}</div>;
}
