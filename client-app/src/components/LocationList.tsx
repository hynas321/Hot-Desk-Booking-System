import { Location } from '../types/Location'
import Button from './Button'
import config from './../config.json';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';

export default function LocationList() {
  const [locations, setLocations] = useState<Location[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    //TODO: Fetch desks from the server based on location name

    const location1: Location = {
      locationName: 'Office A',
    }
  
    const location2: Location = {
      locationName: 'Office B',
    }
  
    setLocations([...locations, location1, location2]);
  }, [])

  const handleAddButtonClick = () => {
    //TODO: Open a pop-up window and ask for a name

    const location3: Location = {
      locationName: "Office C"
    }

    setLocations([...locations, location3])
  }

  const handleChooseLocationButtonClick = (locationName: string) => {
    navigate(`${config.desksViewClientEndpoint}`, { state: { locationName: locationName } });
  }

  const handleRemoveButtonClick = (index: number) => {
    //TODO: API call

    const newLocations = locations.filter((_, i) => i !== index);
    setLocations(newLocations);
  }

  return (
    <>
      <Button
        text={"Add location"}
        active={true}
        spacing={0}
        type={"success"}
        onClick={handleAddButtonClick}
      />
      <ul className="list-group mt-3">
      {
        locations.map((location: Location, index) =>
          <li
            key={index}
            className="list-group-item">
              <div>{`${location.locationName}`}</div>
              <Button
                text="Choose location"
                active={true}
                spacing={0}
                type="primary"
                onClick={() => handleChooseLocationButtonClick(location.locationName)}
              />
              <Button
                text="Remove"
                active={true}
                spacing={3}
                type="danger"
                onClick={() => handleRemoveButtonClick(index)}
              />
          </li>
        )
      }
    </ul>
    </>
  )
}
