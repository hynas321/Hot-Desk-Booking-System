import { useNavigate } from "react-router-dom";
import Button from "../components/Button";
import config from './../config.json';

export default function MainView() {
  const navigate = useNavigate();

  const handleOnStartButtonClick = () => {
    navigate(`${config.locationsViewClientEndpoint}`);
  }

  return (
    <>
      <h3 className="text-center mt-5">Todo signup/signin screen</h3>
      <div className="text-center mt-2">
        <Button
          text={"Start"}
          active={true}
          spacing={0}
          type={"success"}
          onClick={handleOnStartButtonClick}
        />
      </div>
    </>
  )
}
  