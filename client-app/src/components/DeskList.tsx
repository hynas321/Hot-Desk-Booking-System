import { Desk } from '../types/Desk'
import { Location } from '../types/Location'
import Button from './Button'

interface DeskListProps {
  location: Location
  desks: Desk[]
}

export default function DeskList({location, desks}: DeskListProps) {
  return (
    <>
      <h4>Desks in location <b>{location.locationName}</b></h4>
      <ul className="list-group">
        {
          desks.map((desk: Desk, index) =>
            <li 
              key={index}
              className="list-group-item">
                <div>{`${desk.deskName}`}</div>
                <Button text="Book desk" active={desk.holderName == null} spacing={0} type="info" onClick={() => {}}></Button>
                <Button text="Remove" active={desk.holderName == null} spacing={3} type="danger" onClick={() => {}}/>
            </li>
          )
        }
      </ul>
    </>
  )
}
