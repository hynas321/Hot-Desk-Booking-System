import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import LocationsView from './views/LocationsView';
import config from './config.json';
import DesksView from './views/DesksView';
import NotFoundView from './views/NotFoundView';
import MainView from './views/MainView';

function App() {
  const router = createBrowserRouter([
    {
      path: config.mainViewClientEndpoint,
      element: <MainView />
    },
    {
      path: config.locationsViewClientEndpoint,
      element: <LocationsView />
    },
    {
      path: `${config.desksViewClientEndpoint}`,
      element: <DesksView />,
    },
    {
      path: "*",
      element: <NotFoundView />
    }
]);

return (
  <>
    <div className="container">
      <div className="row">
        <h3 className="text-center mt-3">Hot Desk Booking System</h3>
      </div>
      <div className="container col-10">
        <RouterProvider router={router}/>
      </div>
    </div>
  </>
  )
}

export default App;
