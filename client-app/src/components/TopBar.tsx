import useLocalStorageState from "use-local-storage-state";
import ApiRequestHandler from "../http/ApiRequestHandler";
import Button from "./Button";
import { useNavigate } from "react-router-dom";
import config from './../config.json';
import { useAppSelector } from "./redux/hooks";

interface TopBarProps {
  isUserInfoVisible: boolean;
}

export default function TopBar({isUserInfoVisible}: TopBarProps) {
  const [token, setToken] = useLocalStorageState("token", { defaultValue: ""});
  const navigate = useNavigate();
  const username = useAppSelector((state) => state.user.username)

  const apiRequestHandler = new ApiRequestHandler();
  
  const handleButtonClick = () => {
    apiRequestHandler.logOut(token);
    navigate(config.mainViewClientEndpoint);
  }

  return (
    <div>
      <h3 className="text-center text-primary mt-3 mx-auto"><b>Hot Desk Booking System</b></h3>
      {
        isUserInfoVisible &&
        <div className="d-flex justify-content-end align-items-center">
        <h6 className="mx-3">{`${username}`}</h6>
        <Button
          text={"Sign out"}
          active={true}
          spacing={0}
          type="secondary"
          onClick={handleButtonClick}
        />
      </div>
      }
    </div>
  )
}
