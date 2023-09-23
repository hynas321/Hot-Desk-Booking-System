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
import { useDispatch } from 'react-redux';
import { updatedBookedDesk, updatedBookedDeskLocation } from '../components/redux/slices/user-slice';

export default function DesksView() {
  const [desks, setDesks] = useState<Desk[]>([]);
  const [isPopupVisible, setIsPopupVisible] = useState<boolean>(false);
  const [isDeskListVisible, setIsDeskListVisible] = useState<boolean>(false);
  const [bookingDays, setBookingDays] = useState<number>(1);
  const [token] = useLocalStorageState("token", { defaultValue: ""});
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { locationName } = location.state ?? "-";
  const isUserAdmin = useAppSelector((state) => state.user.isAdmin);

  const apiRequestHandler: ApiRequestHandler = new ApiRequestHandler();
  const alertManager: AlertManager = new AlertManager();

  // eslint-disable-next-line
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
      }, 250);
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
    console.log(bookedDesk);
    const deskIndex = desks.findIndex(desk => desk.deskName === deskName);

    const updatedDesks = [...desks];
    updatedDesks[deskIndex] = bookedDesk;

    dispatch(updatedBookedDesk(bookedDesk));
    dispatch(updatedBookedDeskLocation(locationName));
    setDesks(updatedDesks);
  }

  const handleUnbookButtonClick = async (deskName: string) => {
    const unbookedDesk: Desk = await apiRequestHandler.unbookDesk(token, deskName, locationName);

    if (unbookedDesk.deskName === undefined || !unbookedDesk) {
      alertManager.displayAlert(`Could not unbook the desk: ${deskName}`, "danger");
      return;
    }

    const deskIndex = desks.findIndex(desk => desk.deskName === deskName);

    const updatedDesks = [...desks];
    updatedDesks[deskIndex] = unbookedDesk;

    dispatch(updatedBookedDesk(null));
    dispatch(updatedBookedDeskLocation(locationName));
    setDesks(updatedDesks);
  }

  const handleRemoveButtonClick = async (deskName: string) => {
    try {
      const removeDeskStatusCode = await apiRequestHandler.removeDesk(token, deskName, locationName);

      if (removeDeskStatusCode !== 200) {
        alertManager.displayAlert(`Could not remove the desk: ${deskName}`, "danger");
        return;
      }

      const updatedDesks = desks.filter(desk => desk.deskName !== deskName);
      setDesks(updatedDesks);

    } catch (error) {
      alertManager.displayAlert(`Unexpected error`, "danger");
    }
  }

  const handleEnableButtonClick = async (deskName: string) => {
    try {
      const returnedDesk: Desk = await apiRequestHandler.setDeskAvailability(token, deskName, locationName, true);

      if (returnedDesk.deskName === undefined || !returnedDesk) {
        alertManager.displayAlert(`Could not enable the desk: ${deskName}`, "danger");
        return;
      }

      const deskIndex = desks.findIndex(desk => desk.deskName === deskName);

      const updatedDesks = [...desks];
      updatedDesks[deskIndex] = returnedDesk;
      setDesks(updatedDesks);

    } catch (error) {
      alertManager.displayAlert(`Unexpected error`, "danger");
    }
  }

  const handleDisableButtonClick = async (deskName: string) => {
    try {
      const returnedDesk: Desk = await apiRequestHandler.setDeskAvailability(token, deskName, locationName, false);

      if (returnedDesk.deskName === undefined || !returnedDesk) {
        alertManager.displayAlert(`Could not disable the desk: ${deskName}`, "danger");
        return;
      }

      const deskIndex = desks.findIndex(desk => desk.deskName === deskName);

      const updatedDesks = [...desks];
      updatedDesks[deskIndex] = returnedDesk;
      setDesks(updatedDesks);

    } catch (error) {
      alertManager.displayAlert(`Unexpected error`, "danger");
    }
  }

  const handlePopupSubmit = async (deskName: string) => {
    try {
      const addDeskStatusCode = await apiRequestHandler.addDesk(token, deskName, locationName);
      setIsPopupVisible(false);

      if (addDeskStatusCode !== 201) {
        alertManager.displayAlert(`Could not add the desk: ${deskName}`, "danger");
        return;
      }

      const newDesk: Desk = {
        deskName: deskName,
        isEnabled: true,
        username: null,
        startTime: null,
        endTime: null
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
    <div className="container col-lg-6 col-md-10 col-12 mb-5">
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
            onUnbookClick={handleUnbookButtonClick}
            onRemoveClick={handleRemoveButtonClick}
            onEnableClick={handleEnableButtonClick}
            onDisableClick={handleDisableButtonClick}
            onRangeChange={handleDaysRangeChange}
          />
        </>
        : <h5 className="mt-3 text-success">Loading data...</h5>
      }
    </div>
  );
}
