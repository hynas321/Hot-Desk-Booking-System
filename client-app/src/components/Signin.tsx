import { useEffect, useState } from "react";
import Button from "./Button";
import TextForm from "./TextForm";
import { useNavigate } from "react-router-dom";
import ApiRequestHandler from "../http/ApiRequestHandler";
import config from './../config.json';
import useLocalStorageState from "use-local-storage-state";
import { useDispatch } from "react-redux";
import { updatedUsername } from "./redux/slices/user-slice";
import { AlertManager } from "../managers/AlertManager";
import { TokenOutput } from "../http/ApiInterfaces";

export default function Signin() {
  const [username, setUsername] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [isButtonActive, setIsButtonActive] = useState<boolean>(false);
  const [, setToken] = useLocalStorageState("token", { defaultValue: ""});
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const apiRequestHandler: ApiRequestHandler = new ApiRequestHandler();
  const alertManager: AlertManager = new AlertManager();

  useEffect(() => {
    if (username.length >= 5 && password.length >= 5) {
      setIsButtonActive(true);
    }
    else {
      setIsButtonActive(false);
    }
  }, [username, password]);

  const handleButtonClick = () => {
    const logInAsync = async () => {
      const tokenObj: TokenOutput = await apiRequestHandler.logIn(username, password);

      if (typeof(tokenObj.token) !== "string") {
        alertManager.displayAlert("Could not log in, check your username or password", "danger");
        return;
      }

      setToken(tokenObj.token);
      dispatch(updatedUsername(username));
      navigate(`${config.locationsViewClientEndpoint}`);
    }

    logInAsync();
  }

  return (
    <div className="col-lg-5 col-md-8 col-10 rounded p-3 bg-light mx-auto mt-3 ">
      <div className="form-inline mt-3">
        <h3 className="text-success"><b>Sign-In</b></h3>
        <div className="form-group mt-2">
          <label>Username</label>
          <TextForm
            placeholderValue="Min 5 characters"
            inputType="text"
            onChange={(value) => setUsername(value)}
          />
        </div>
        <div className="form-group mt-2">
          <label>Password</label>
          <TextForm
            placeholderValue={"Min 5 characters"}
            inputType="password"
            onChange={(value) => setPassword(value)}
          />
        </div>
        <div className="form-group mt-2">
          <Button 
            text="Sign in"
            active={isButtonActive}
            spacing={0}
            type="success"
            onClick={handleButtonClick}
          />
        </div>
      </div>
      <div className="card mt-4">
        <div className="card-body">
          {"If you do not have your credentials, please ask the supervisor"}
          <div className="text-secondary">{"Available in the README.md file :)"}</div>
        </div>
      </div>
    </div>
  )
}
