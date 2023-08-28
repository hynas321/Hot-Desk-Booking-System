import { Location } from '../types/Location'
import LocationList from '../components/LocationList';
import Button from '../components/Button';
import config from './../config.json';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import Popup from '../components/Popup';

export default function LocationsView() {
  const [locations, setLocations] = useState<Location[]>([]);
  const [isPopupVisible, setIsPopupVisible] = useState<boolean>(false);
  const navigate = useNavigate();

  useEffect(() => {
    //TODO: Fetch desks from the server based on location name

    const location1: Location = {
      locationName: 'Office A',
      deskCount: 2
    }
  
    const location2: Location = {
      locationName: 'Office B',
      deskCount: 2
    }
  
    setLocations([...locations, location1, location2]);
  }, [])

  const handleAddButtonClick = () => {
    setIsPopupVisible(true);
  }

  const handleChooseLocationButtonClick = (locationName: string) => {
    navigate(`${config.desksViewClientEndpoint}`, { state: { locationName: locationName } });
  }

  const handleRemoveButtonClick = (index: number) => {
    //TODO: API call

    const newLocations = locations.filter((_, i) => i !== index);
    setLocations(newLocations);
  }

  const handlePopupSubmit = (locationName: string) => {
    //TODO: API CALL
    setIsPopupVisible(false);

    const newLocation: Location = {
      locationName: locationName,
      deskCount: 0
    };

    setLocations([...locations, newLocation]);
  }

  return (
    <>
      <h4>Locations</h4>
      <Popup 
        title={"Name a new location"}
        inputFormPlaceholderText={"Insert the name here"}
        visible={isPopupVisible}
        onSubmit={handlePopupSubmit}
        onClose={() => setIsPopupVisible(false)}
      />
      <Button
        text={"Add location"}
        active={true}
        spacing={0}
        type={"success"}
        onClick={handleAddButtonClick}
      />
      <LocationList 
        locations={locations}
        onChooseClick={handleChooseLocationButtonClick}
        onRemoveClick={handleRemoveButtonClick}
      />
    </>
  )
}
