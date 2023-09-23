import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import LocationsView from './views/LocationsView';
import config from './config.json';
import DesksView from './views/DesksView';
import NotFoundView from './views/NotFoundView';
import MainView from './views/MainView';
import { Provider } from 'react-redux';
import { store } from './components/redux/store';

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
      path: config.desksViewClientEndpoint,
      element: <DesksView />,
    },
    {
      path: "*",
      element: <NotFoundView />
    }
]);

return (
    <Provider store={store}>
      <RouterProvider router={router}/>
    </Provider>
  )
}

export default App;
