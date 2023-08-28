import { Desk } from '../types/Desk';
import DeskList from '../components/DeskList';

export default function LocationView() {
  let desks: Desk[] = [];

  let desk1: Desk = {
    deskName: 'Desk 1',
    locationName: 'Office A',
    holderName: null
  }

  let desk2: Desk = {
    deskName: 'Desk 2',
    locationName: 'Office B',
    holderName: "A"
  }

  desks.push(desk1);
  desks.push(desk2);

  return (
    <div className="container">
      <DeskList desks={[desk1, desk2, desk2]} location={{
        locationName: 'Abc'
      }} />
    </div>
  );
}
