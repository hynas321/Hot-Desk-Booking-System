import DeskList from '../components/DeskList';
import Button from '../components/Button';
import { useEffect, useState } from 'react';
import { Desk } from '../types/Desk'
import Popup from '../components/Popup';
import { useLocation, useNavigate } from 'react-router-dom';
import config from './../config.json';

export default function DesksView() {
  const [desks, setDesks] = useState<Desk[]>([]);
  const [isPopupVisible, setIsPopupVisible] = useState<boolean>(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { locationName } = location.state ?? "-";

  useEffect(() => {
    //TODO: Fetch desks from the server based on location name

    const desk1: Desk = {
      deskName: 'Desk 1',
      locationName: 'Office A',
      bookerName: null
    }
  
    const desk2: Desk = {
      deskName: 'Desk 2',
      locationName: 'Office B',
      bookerName: "A"
    }
  
    setDesks([...desks, desk1, desk2]);
  }, [])

  const handleAddButtonClick = () => {
    setIsPopupVisible(true);
  }

  const handleBookButtonClick = (index: number) => {
    //TODO: API call, add bookerName

    const modifiedDesk = { ...desks[index], bookerName: "YOUR_HOLDER_NAME" };
    const newDesks = desks.map((desk, i) => (i === index ? modifiedDesk : desk));
    setDesks(newDesks);
  }

  const handleRemoveButtonClick = (index: number) => {
    //TODO: API call

    const newDesks = desks.filter((_, i) => i !== index);
    setDesks(newDesks);
  }

  const handlePopupSubmit = (deskName: string) => {
    //TODO: API CALL

    setIsPopupVisible(false);

    const newDesk: Desk = {
      deskName: deskName,
      locationName: locationName,
      bookerName: null
    };

    setDesks([...desks, newDesk]);
  }

  const handleGoBackButtonClick = () => {
    navigate(`${config.locationsViewClientEndpoint}`);
  }

  return (
    <>
      <h4>{"Desks in location: "}<b>{`${locationName}`}</b></h4>
      <Popup 
        title={"Name a new desk"}
        inputFormPlaceholderText={"Insert the name here"}
        visible={isPopupVisible}
        onSubmit={handlePopupSubmit}
        onClose={() => setIsPopupVisible(false)}
      />
      <Button
        text={"Add desk"}
        active={true}
        spacing={0}
        type={"success"}
        onClick={handleAddButtonClick}
      />
      <Button
        text= {"Return to locations"}
        active={true}
        spacing={2}
        type={"secondary"}
        onClick={handleGoBackButtonClick}
      />
      <DeskList
        desks={desks}
        onBookClick={handleBookButtonClick}
        onRemoveClick={handleRemoveButtonClick}
      />
    </>
  );
}
