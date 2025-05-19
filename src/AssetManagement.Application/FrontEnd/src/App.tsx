import { ToastContainer } from "react-toastify";
import useRouteElements from "./useRouteElement";

function App() {
  const routeElements = useRouteElements();
  return (
    <>
      <div>{routeElements}</div>
      <ToastContainer />
    </>
  );
}

export default App;
