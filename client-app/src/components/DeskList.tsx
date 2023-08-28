import { useState } from 'react';
import { Desk } from '../types/Desk'
import Button from './Button'
import Range from './Range'

interface DeskListProps {
  desks: Desk[],
  onBookClick: (index: number) => void,
  onRemoveClick: (index: number) => void
}

export default function DeskList({desks, onBookClick, onRemoveClick}: DeskListProps) {
  const [hoveredIndex, setHoveredIndex] = useState<number | null>(null);
  
  return (
    <>
      <ul className="list-group mt-3">
        {
          desks.map((desk: Desk, index) =>
            <li 
              key={index}
              className={`list-group-item ${desk.bookerName === null ? "bg-white" : "bg-light"}`}
              onMouseEnter={() => setHoveredIndex(index)}
              onMouseLeave={() => setHoveredIndex(null)}
            >
              <div><b>{`${desk.deskName}`}</b></div>
              <div className={`d-flex mb-2 ${desk.bookerName === null ? "text-success" : "text-danger"}`}>
                {`${desk.bookerName === null ? "AVAILABLE" : "BOOKED"}`}
              </div>
              <div className="d-flex">
                <Button
                  text="Book desk"
                  active={desk.bookerName === null}
                  spacing={0}
                  type="primary"
                  onClick={() => onBookClick(index)}
                />
                <Button
                  text="Remove"
                  active={desk.bookerName === null}
                  spacing={3}
                  type="danger"
                  onClick={() => onRemoveClick(index)}
                />
                { 
                  desk.bookerName === null && hoveredIndex === index && (
                    <Range
                      title={"Booking timespan"}
                      suffix={"days"}
                      minValue={1}
                      maxValue={7}
                      step={1}
                      defaultValue={1}
                      onChange={() => {}}
                    />
                  )
                }
              </div>  
            </li>
          )
        }
      </ul>
    </>
  )
}
