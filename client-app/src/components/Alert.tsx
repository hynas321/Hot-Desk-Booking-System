import { useAppSelector } from "./redux/hooks";

function Alert() {
  const alert = useAppSelector((state) => state.alert);
  
  return (
    <>
      {alert.visible && (
        <div className={`alert alert-${alert.type} text-center`} role="alert">
          <h6><b>{alert.text}</b></h6>
        </div>
      )}
    </>
  );
}

export default Alert;