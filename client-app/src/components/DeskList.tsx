import { Desk } from '../types/Desk'
import Button from './Button'

interface DeskListProps {
  desks: Desk[],
  onBookClick: (index: number) => void,
  onRemoveClick: (index: number) => void
}

export default function DeskList({desks, onBookClick, onRemoveClick}: DeskListProps) {
  return (
    <>
      <ul className="list-group mt-3">
        {
          desks.map((desk: Desk, index) =>
            <li 
              key={index}
              className={`list-group-item ${desk.holderName === null ? "bg-white" : "bg-light"}`}>
                <div>{`${desk.deskName}`}</div>
                <div className={`d-flex ${desk.holderName === null ? "text-success" : "text-danger"}`}>
                  {`${desk.holderName === null ? "AVAILABLE" : "BOOKED"}`}
                </div>
                <Button
                  text="Book desk"
                  active={desk.holderName === null}
                  spacing={0}
                  type="primary"
                  onClick={() => onBookClick(index)}
                />
                <Button
                  text="Remove"
                  active={desk.holderName === null}
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
