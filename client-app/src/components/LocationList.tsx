import { Location } from '../types/Location'
import Button from './Button'
import { useAppSelector } from './redux/hooks';

interface LocationListProps {
  locations: Location[],
  onChooseClick: (locationName: string) => void,
  onRemoveClick: (locationName: string) => void
}

export default function LocationList({locations, onChooseClick, onRemoveClick}: LocationListProps) {
  const isUserAdmin = useAppSelector((state) => state.user.isAdmin);
  
  return (
    <ul className="list-group mt-3">
      {
        locations.length !== 0 ? (
          locations.map((location: Location, index) =>
          <li
            key={index}
            className="list-group-item">
              <div><b>{`${location.locationName}`}</b></div>
              <div className="mb-2">{`Total desks: ${location.totalDeskCount}, available: ${location.availableDeskCount}`}</div>
              <Button
                text="Enter location"
                active={true}
                spacing={0}
                type="primary"
                onClick={() => onChooseClick(location.locationName)}
              />
              {
                isUserAdmin &&
                  <Button
                  text="Remove"
                  active={location.totalDeskCount === 0}
                  spacing={3}
                  type="danger"
                  onClick={() => onRemoveClick(location.locationName)}
                />
              }
          </li>
        ))
        : <h5 className="text-danger">{"Nothing to display :("}</h5>
      }
    </ul>
  )
}
