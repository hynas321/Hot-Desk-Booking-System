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
    <>
      <ul className="list-group mt-3">
      {
        locations.map((location: Location, index) =>
          <li
            key={index}
            className="list-group-item">
              <div><b>{`${location.locationName}`}</b></div>
              <div className="mb-2">{`Number of desks: ${location.deskCount}`}</div>
              <Button
                text="Enter this location"
                active={true}
                spacing={0}
                type="primary"
                onClick={() => onChooseClick(location.locationName)}
              />
              {
                isUserAdmin &&
                  <Button
                  text="Remove"
                  active={location.deskCount === 0}
                  spacing={3}
                  type="danger"
                  onClick={() => onRemoveClick(location.locationName)}
                />
              }
          </li>
        )
      }
      </ul>
    </>
  )
}
