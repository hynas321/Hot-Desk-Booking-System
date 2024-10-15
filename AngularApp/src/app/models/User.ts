import { Desk } from "./Desk";

export interface User {
  username: string;
  isAdmin: boolean;
  bookedDesk: Desk | null;
  bookedDeskLocation: string | null;
}
