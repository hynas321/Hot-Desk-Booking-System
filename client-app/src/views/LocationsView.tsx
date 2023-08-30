import { Location } from '../types/Location'
import LocationList from '../components/LocationList';
import Button from '../components/Button';
import config from './../config.json';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import Popup from '../components/Popup';
import ApiRequestHandler from '../http/ApiRequestHandler';
import useLocalStorageState from 'use-local-storage-state';
import TopBar from '../components/TopBar';
import { useAppSelector } from '../components/redux/hooks';
import { useDispatch } from 'react-redux';
import { updatedIsAdmin } from '../components/redux/slices/user-slice';
import { AlertManager } from '../managers/AlertManager';

export default function LocationsView() {
  const [locations, setLocations] = useState<Location[]>([]);
  const [isPopupVisible, setIsPopupVisible] = useState<boolean>(false);
  const [isLocationListVisible, setIsLocationListVisible] = useState<boolean>(false);
  const [token, setToken] = useLocalStorageState("token", { defaultValue: ""});
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const user = useAppSelector((state) => state.user);

  const apiRequestHandler: ApiRequestHandler = new ApiRequestHandler();
  const alertManager: AlertManager = new AlertManager();

  useEffect(() => {
    //if (!token || token === "") {
      //navigate(config.mainViewClientEndpoint);
    //}

    const checkIsAdminAsync = async () => {
      const isAdmin = await apiRequestHandler.checkIsAdmin(user.username);
      dispatch(updatedIsAdmin(isAdmin));
    }

    const fetchLocationsAsync = async () => {
      const fetchedLocations = await apiRequestHandler.getAllLocationNames();
      
      setLocations(fetchedLocations);

      setTimeout(() => {
        setIsLocationListVisible(true);
      }, 500);
    }
    
    checkIsAdminAsync();
    fetchLocationsAsync();
  }, []);

  const handleAddButtonClick = () => {
    setIsPopupVisible(true);
  }

  const handleChooseLocationButtonClick = (locationName: string) => {
    navigate(`${config.desksViewClientEndpoint}`, { state: { locationName: locationName } });
  }

  const handleRemoveButtonClick = async (locationName: string) => {
    try {
      const removeLocationStatusCode = await apiRequestHandler.removeLocation(token, locationName);

      if (removeLocationStatusCode != 200) {
        alertManager.displayAlert(`Could not remove the location ${locationName}`, "danger");
        return;
      }

      const updatedLocations = locations.filter(location => location.locationName !== locationName);
      setLocations(updatedLocations);
  
    } catch (error) {
      alertManager.displayAlert("Unexpected error", "danger");
    }
  }

  const handlePopupSubmit = async (locationName: string) => {
    try {
      const addLocationStatusCode = await apiRequestHandler.addLocation(token, locationName);
      setIsPopupVisible(false);

      if (addLocationStatusCode != 201) {
        alertManager.displayAlert(`Could not add the location: ${locationName}`, "danger");
        return;
      }

      const newLocation: Location = {
        locationName: locationName,
        totalDeskCount: 0,
        availableDeskCount: 0
      };

      setLocations([...locations, newLocation]);
    } catch (error) {
      alertManager.displayAlert(`Unexpected error`, "danger");
    }
  }

  return (
    <div className="mb-5">
      <TopBar isUserInfoVisible={true} />
      <h4>Locations</h4>
      <Popup 
        title={"Name a new location"}
        inputFormPlaceholderText={"Insert the name here"}
        visible={isPopupVisible}
        onSubmit={handlePopupSubmit}
        onClose={() => setIsPopupVisible(false)}
      />
      {
        isLocationListVisible ?
        <>
        {
          user.isAdmin &&
            <Button
            text={"Add location"}
            active={true}
            spacing={0}
            type={"success"}
            onClick={handleAddButtonClick}
          />
        }
          <LocationList 
            locations={locations}
            onChooseClick={handleChooseLocationButtonClick}
            onRemoveClick={handleRemoveButtonClick}
          />
        </>
         :
        <h5 className="mt-3 text-success">Loading data...</h5>
      }
    </div>
  )
}
