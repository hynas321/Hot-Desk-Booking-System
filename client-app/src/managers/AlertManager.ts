import { useDispatch } from "react-redux";
import { updatedAlert, updatedVisible } from "../components/redux/slices/alert-slice";

export class AlertManager {
  private dispatch = useDispatch();

  displayAlert = (message: string, type: string) => {
    this.dispatch(updatedAlert({
      text: message,
      visible: true,
      type: type
    }));
    
    setTimeout(() => {
      this.dispatch(updatedVisible(false));
    }, 3000);
  }
}