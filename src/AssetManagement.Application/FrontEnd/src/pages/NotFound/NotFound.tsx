import { useNavigate } from "react-router-dom";

export default function NotFound() {
  const navigate = useNavigate();

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-tertiary">
      <div className="text-center p-8 max-w-lg">
        <div className="text-primary text-9xl font-bold mb-4">404</div>
        <h1 className="text-quaternary text-3xl font-bold mb-4">
          Page Not Found
        </h1>
        <p className="text-quaternary mb-8">
          The page you are looking for might have been removed, had its name
          changed, or is temporarily unavailable.
        </p>
        <button
          onClick={() => navigate("/")}
          className="btn-primary px-6 py-2 rounded-md shadow-md hover:opacity-90 transition-opacity"
        >
          Return to Home
        </button>
      </div>
      <div className="mt-8 border-t border-primary w-48 pt-4 text-center">
        <p className="text-quaternary text-sm">Asset Management System</p>
      </div>
    </div>
  );
}
