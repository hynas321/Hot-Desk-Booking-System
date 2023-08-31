import useLocalStorageState from "use-local-storage-state";
import ApiRequestHandler from "../http/ApiRequestHandler";
import Button from "./Button";
import { useNavigate } from "react-router-dom";
import config from './../config.json';
import { useAppSelector } from "./redux/hooks";
import Alert from "./Alert";
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { updatedIsAdmin, updatedUsername } from "./redux/slices/user-slice";
import { UserIsAdminOutput } from "../http/ApiInterfaces";

interface TopBarProps {
  isUserInfoVisible: boolean;
}

export default function TopBar({isUserInfoVisible}: TopBarProps) {
  const [token, setToken] = useLocalStorageState("token", { defaultValue: ""});
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const user = useAppSelector((state) => state.user)

  const apiRequestHandler = new ApiRequestHandler();
  
  useEffect(() => {
    if (!isUserInfoVisible && user.username == "User") {
      return;
    }

    const checkIsAdminAsync = async () => {
      const isAdminObj: UserIsAdminOutput = await apiRequestHandler.checkIsAdminByToken(token);
      
      if (!isAdminObj) {
        dispatch(updatedUsername("User"));
        return;
      }

      dispatch(updatedIsAdmin(isAdminObj.isAdmin));
      dispatch(updatedUsername(isAdminObj.username));
    }

    checkIsAdminAsync();
  }, []);

  const handleButtonClick = () => {
    apiRequestHandler.logOut(token);
    navigate(config.mainViewClientEndpoint);
  }

  return (
    <div>
      <div className="row mb-3">
        <div className="col-12 col-md-3"></div>
        <div className="col-12 col-md-6">
          <h3 className="text-center mt-3">
            <b className="text-primary">Hot Desk Booking System</b>
          </h3>
          <Alert />
        </div>
        <div className="col-12 col-md-3 mt-3 justify-content-md-end">
          <div className="d-flex justify-content-center justify-content-md-end">
            {
              isUserInfoVisible &&
              <Button
                text={"Sign out"}
                active={true}
                spacing={0}
                type="danger"
                onClick={handleButtonClick}
              />
            }
          </div>
        </div>
      </div>
      {
        isUserInfoVisible && (
          <div className="mb-3">
            <h5 className="text-secondary">{`${user.isAdmin ? "Administrator" : "Employee"}:`} <b className="text-secondary">{user.username}</b></h5>
          </div>
            ) 
          }
    </div>
  )
}