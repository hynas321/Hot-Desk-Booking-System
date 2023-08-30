import Signin from "../components/Signin";
import TopBar from "../components/TopBar";

export default function MainView() {
  return (
    <div className="text-center">
      <TopBar isUserInfoVisible={false} />
      <Signin />
    </div>
  )
}
  