import { Location } from '../types/Location'
import Button from './Button'

interface LocationListProps {
  locations: Location[],
  onChooseClick: (locationName: string) => void,
  onRemoveClick: (index: number) => void
}

export default function LocationList({locations, onChooseClick, onRemoveClick}: LocationListProps) {
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
                text="Choose location"
                active={true}
                spacing={0}
                type="primary"
                onClick={() => onChooseClick(location.locationName)}
              />
              <Button
                text="Remove"
                active={true}
                spacing={3}
                type="danger"
                onClick={() => onRemoveClick(index)}
              />
          </li>
        )
      }
      </ul>
    </>
  )
}
