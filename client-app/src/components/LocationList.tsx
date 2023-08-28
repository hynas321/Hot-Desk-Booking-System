import { Location } from '../types/Location'
import Button from './Button'

interface LocationListProps {
  locations: Location[]
}

export default function LocationList({locations}: LocationListProps) {
  return (
    <>
      <h4>Locations</h4>
      <ul className="list-group">
      {
        locations.map((location: Location, index) =>
          <li
            key={index}
            className="list-group-item">
              <div>{`${location.locationName}`}</div>
              <Button text="Choose location" active={true} spacing={0} type="success" onClick={() => {}}/>
              <Button text="Remove" active={true} spacing={3} type="danger" onClick={() => {}}/>
          </li>
        )
      }
    </ul>
    </>
  )
}
