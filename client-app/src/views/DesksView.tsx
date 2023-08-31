import DeskList from '../components/DeskList';
import Button from '../components/Button';
import { useEffect, useState } from 'react';
import { Desk } from '../types/Desk'
import Popup from '../components/Popup';
import { useLocation, useNavigate } from 'react-router-dom';
import config from './../config.json';
import ApiRequestHandler from '../http/ApiRequestHandler';
import TopBar from '../components/TopBar';
import useLocalStorageState from 'use-local-storage-state';
import { useAppSelector } from '../components/redux/hooks';
import { AlertManager } from '../managers/AlertManager';

export default function DesksView() {
  const [desks, setDesks] = useState<Desk[]>([]);
  const [isPopupVisible, setIsPopupVisible] = useState<boolean>(false);
  const [isDeskListVisible, setIsDeskListVisible] = useState<boolean>(false);
  const [bookingDays, setBookingDays] = useState<number>(1);
  const [token, setToken] = useLocalStorageState("token", { defaultValue: ""});
  const navigate = useNavigate();
  const location = useLocation();
  const { locationName } = location.state ?? "-";
  const isUserAdmin = useAppSelector((state) => state.user.isAdmin);

  const apiRequestHandler: ApiRequestHandler = new ApiRequestHandler();
  const alertManager: AlertManager = new AlertManager();

  useEffect(() => {
    if (locationName === undefined) {
      navigate(config.locationsViewClientEndpoint);
    }

    const fetchDesksAsync = async () => {
      const fetchedDesks: Desk[] = await apiRequestHandler.getDesks(token, locationName);

      if (!Array.isArray(fetchedDesks)) {
        alertManager.displayAlert("Could not load desks", "danger");
        navigate(config.locationsViewClientEndpoint);
        return;
      }

      setDesks(fetchedDesks);

      setTimeout(() => {
        setIsDeskListVisible(true);
      }, 500);
    }
    
    fetchDesksAsync();
  }, [])

  const handleAddButtonClick = () => {
    setIsPopupVisible(true);
  }

  const handleBookButtonClick = async (deskName: string) => {
    const bookedDesk: Desk = await apiRequestHandler.bookDesk(token, deskName, locationName, bookingDays);

    if (bookedDesk.deskName === undefined || !bookedDesk) {
      alertManager.displayAlert(`Could not book the desk: ${deskName}`, "danger");
      return;
    }

    const deskIndex = desks.findIndex(desk => desk.deskName === deskName);

    const updatedDesks = [...desks];
    updatedDesks[deskIndex] = bookedDesk;

    setDesks(updatedDesks);
  }

  const handleRemoveButtonClick = async (deskName: string) => {
    try {
      const removeDeskStatusCode = await apiRequestHandler.removeDesk(token, deskName, locationName);

      if (removeDeskStatusCode != 200) {
        alertManager.displayAlert(`Could not remove the desk: ${deskName}`, "danger");
        return;
      }

      const updatedDesks = desks.filter(desk => desk.deskName !== deskName);
      setDesks(updatedDesks);

    } catch (error) {
      alertManager.displayAlert(`Unexpected error`, "danger");
    }
  }

  const handlePopupSubmit = async (deskName: string) => {
    try {
      const addDeskStatusCode = await apiRequestHandler.addDesk(token, deskName, locationName);
      setIsPopupVisible(false);

      if (addDeskStatusCode != 201) {
        alertManager.displayAlert(`Could not add the desk: ${deskName}`, "danger");
        return;
      }

      const newDesk: Desk = {
        deskName: deskName,
        username: null,
        bookingStartTime: null,
        bookingEndTime: null,
      };

      setDesks([...desks, newDesk]);
    } catch (error) {
      alertManager.displayAlert(`Unexpected error`, "danger");
    }
  }

  const handleDaysRangeChange = (value: number) => {
    setBookingDays(value);
  }

  const handleGoBackButtonClick = () => {
    navigate(`${config.locationsViewClientEndpoint}`);
  }

  return (
    <div className="mb-5">
      <TopBar isUserInfoVisible={true} />
      <h4>{"Desks in location: "}<b>{`${locationName}`}</b></h4>
      <Popup 
        title={"Name a new desk"}
        inputFormPlaceholderText={"Insert the name here"}
        visible={isPopupVisible}
        onSubmit={handlePopupSubmit}
        onClose={() => setIsPopupVisible(false)}
      />
      {
        isDeskListVisible ?
        <>
        {
          isUserAdmin &&
          <Button
            text={"Add desk"}
            active={true}
            spacing={0}
            type={"success"}
            onClick={handleAddButtonClick}
          />
        }
          <Button
            text= {"Return to locations"}
            active={true}
            spacing={1}
            type={"secondary"}
            onClick={handleGoBackButtonClick}
          />
          <DeskList
            desks={desks}
            onBookClick={handleBookButtonClick}
            onRemoveClick={handleRemoveButtonClick}
            onRangeChange={handleDaysRangeChange}
          />
        </>
        : <h5 className="mt-3 text-success">Loading data...</h5>
      }
    </div>
  );
}
