import { useNavigate } from "react-router-dom";
import Button from "../components/Button";
import config from './../config.json';

export default function MainView() {
  const navigate = useNavigate();

  const handleOnStartButtonClick = () => {
    navigate(`${config.locationsViewClientEndpoint}`, { state: { locationName: "Abc" } });
  }

  return (
    <div className="text-center">
      <Button
        text={"Start"}
        active={true}
        spacing={0}
        type={"success"}
        onClick={handleOnStartButtonClick}
      />
    </div>
  )
}
  