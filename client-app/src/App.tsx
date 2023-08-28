import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import MainView from './views/MainView';
import config from './config.json';
import LocationView from './views/LocationView';
import NotFoundView from './views/NotFoundView';

function App() {
  const router = createBrowserRouter([
    {
      path: config.mainViewClientEndpoint,
      element: <MainView />
    },
    {
      path: `${config.locationViewClientEndpoint}`,
      element: <LocationView />,
    },
    {
      path: "*",
      element: <NotFoundView />
    }
]);

return (
    <>
      <h3 className="text-center mt-3">Hot Desk Booking System</h3>
      <RouterProvider router={router}/>
    </>
  )
}

export default App;
