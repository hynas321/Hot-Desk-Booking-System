import { useEffect, useState } from 'react'
import { Desk } from '../types/Desk'
import Button from './Button'

export default function DeskList() {
  const [desks, setDesks] = useState<Desk[]>([]);

  useEffect(() => {
    //TODO: Fetch desks from the server based on location name

    const desk1: Desk = {
      deskName: 'Desk 1',
      locationName: 'Office A',
      holderName: null
    }
  
    const desk2: Desk = {
      deskName: 'Desk 2',
      locationName: 'Office B',
      holderName: "A"
    }
  
    setDesks([...desks, desk1, desk2]);
  }, [])

  const handleAddButtonClick = () => {
    //TODO: Open a pop-up window and ask for a name

    let desk3: Desk = {
      deskName: 'Desk 3',
      locationName: 'Office C',
      holderName: null
    }

    setDesks([...desks, desk3])
  }

  const handleBookButtonClick = (index: number) => {
    //TODO: API call

    const modifiedDesk = { ...desks[index], holderName: "YOUR_HOLDER_NAME" };
    const newDesks = desks.map((desk, i) => (i === index ? modifiedDesk : desk));
    setDesks(newDesks);
  }

  const handleRemoveButtonClick = (index: number) => {
    //TODO: API call

    const newDesks = desks.filter((_, i) => i !== index);
    setDesks(newDesks);
  }

  return (
    <>
      <Button
        text={"Add desk"}
        active={true}
        spacing={0}
        type={"success"}
        onClick={handleAddButtonClick}
      />
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
                  onClick={() => handleBookButtonClick(index)}
                />
                <Button
                  text="Remove"
                  active={desk.holderName === null}
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
