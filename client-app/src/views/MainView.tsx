import { Location } from '../types/Location'
import LocationList from '../components/LocationList';

export default function MainView() {
  let locations: Location[] = [];

  let location1: Location = {
    locationName: "Abc 1"
  }

  let location2: Location = {
    locationName: "Abc 2"
  }

  locations.push(location1);
  locations.push(location2);

  return (
    <div className="container">
      <LocationList locations={[location1, location2]} />
    </div>
  )
}
