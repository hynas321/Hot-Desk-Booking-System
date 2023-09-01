import { useState } from 'react';
import { Desk } from '../types/Desk'
import Button from './Button'
import Range from './Range'
import { useAppSelector } from './redux/hooks';

interface DeskListProps {
  desks: Desk[],
  onBookClick: (deskName: string) => void,
  onUnbookClick: (deskName: string) => void,
  onRemoveClick: (deskName: string) => void,
  onEnableClick: (deskName: string) => void,
  onDisableClick: (deskName: string) => void,
  onRangeChange: (value: number) => void
}

export default function DeskList({desks, onBookClick, onUnbookClick, onRemoveClick, onEnableClick, onDisableClick, onRangeChange}: DeskListProps) {
  const [hoveredIndex, setHoveredIndex] = useState<number | null>(null);
  const user = useAppSelector((state) => state.user);

  return (
    <>
      <ul className="list-group mt-3">
        {
          desks.length !== 0 ? (
            desks.map((desk: Desk, index) =>
            <li 
              key={index}
              className={
                `list-group-item 
                ${desk.username === null && desk.isEnabled ? "bg-white" : "bg-light"}`
              }
              onMouseEnter={() => setHoveredIndex(index)}
              onMouseLeave={() => setHoveredIndex(null)}
            >
              <div>
                <b>{`${desk.deskName}`}</b>
              </div>
              <div className={`d-flex ${desk.username === null && desk.isEnabled ? "text-success" : "text-danger"}`}>
                {
                  (desk.username === null && desk.isEnabled) && <>{"Available"}</>
                }
                {
                  (desk.username === null && !desk.isEnabled) && <>{"Disabled"}</>
                }
                {
                  (desk.username !== null) && <>{`Booked from ${desk.startTime}`}<br/>{`Booked until the end of ${desk.endTime}`}</>
                }
              </div>
                <div className="text-primary mb-2">
                  {
                    (desk.username === user.username) && "Your booking"
                  }
                  {
                    (user.isAdmin && desk.username !== null && desk.username != user.username) && `Booked by ${desk.username}`
                  }
                </div>
                <div className="d-flex">
                  {
                    (desk.username === user.username) ?
                    <Button
                      text="Unbook desk"
                      active={desk.username === user.username}
                      spacing={0}
                      type="primary"
                      onClick={() => onUnbookClick(desk.deskName)}
                    />
                    :
                    <Button
                      text="Book desk"
                      active={desk.username === null && desk.isEnabled && !user.bookedDesk}
                      spacing={0}
                      type="primary"
                      onClick={() => onBookClick(desk.deskName)}
                    />
                  }
                  {
                    (user.isAdmin) &&
                    <Button
                      text="Remove"
                      active={desk.username === null}
                      spacing={2}
                      type="danger"
                      onClick={() => onRemoveClick(desk.deskName)}
                    />
                  }
                  {
                    (user.isAdmin && desk.isEnabled) &&
                    <Button
                      text="Disable"
                      active={desk.username === null}
                      spacing={0}
                      type="secondary"
                      onClick={() => onDisableClick(desk.deskName)}
                    />
                  }
                  {
                    (user.isAdmin && !desk.isEnabled) &&
                    <Button
                      text="Enable"
                      active={desk.username === null}
                      spacing={0}
                      type="secondary"
                      onClick={() => onEnableClick(desk.deskName)}
                    />
                  }
                  { 
                    (desk.username === null && hoveredIndex === index && desk.isEnabled) && (
                      <Range
                        title={"Booking timespan"}
                        suffix={"days"}
                        minValue={1}
                        maxValue={7}
                        step={1}
                        defaultValue={1}
                        onChange={onRangeChange}
                      />
                    )
                  }
              </div>  
            </li>
          )) 
          : <h5 className="text-danger">{"Nothing to display :("}</h5>
        }
      </ul>
    </>
  )
}
