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

  useEffect(() => {
    if (locationName === undefined) {
      navigate(config.locationsViewClientEndpoint);
    }

    const fetchDesksAsync = async () => {
      const fetchedDesks = await apiRequestHandler.getDesks(locationName);
      console.log(fetchedDesks);
      setDesks(fetchedDesks);

      setTimeout(() => {
        setIsDeskListVisible(true);
      }, 1000);
    }
    
    fetchDesksAsync();
  }, [])

  const handleAddButtonClick = () => {
    setIsPopupVisible(true);
  }

  const handleBookButtonClick = async (deskName: string) => {
    //TODO: get date from the server to update desk
    await apiRequestHandler.bookDesk(token, deskName, locationName, bookingDays);

    const updatedDesk: Desk = {
      deskName: deskName,
      username: "YOUR_USERNAME",
      bookingStartDate: new Date(),
      bookingEndTime: new Date()
    };

    const deskIndex = desks.findIndex(desk => desk.deskName === deskName);

    const updatedDesks = [...desks];
    updatedDesks[deskIndex] = updatedDesk;

    setDesks(updatedDesks);
  }

  const handleRemoveButtonClick = async (deskName: string) => {
    try {
      //TODO: boolean
      await apiRequestHandler.removeDesk(token, deskName, locationName);
      const updatedDesks = desks.filter(desk => desk.deskName !== deskName);
      setDesks(updatedDesks);
    } catch (error) {
      console.error("Error removing desk:", error);
    }
  }

  const handlePopupSubmit = async (deskName: string) => {
    try {
      //TODO: boolean
      await apiRequestHandler.addDesk(token, deskName, locationName);
      setIsPopupVisible(false);

      const newDesk: Desk = {
        deskName: deskName,
        username: null,
        bookingStartDate: null,
        bookingEndTime: null,
      };

      setDesks([...desks, newDesk]);
    } catch (error) {
      console.error("Error adding desk:", error);
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
            spacing={2}
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
