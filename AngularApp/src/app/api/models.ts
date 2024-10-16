import { Desk } from '../models/Desk';

export interface BookingInformation {
  deskName: string;
  locationName: string;
  days: number;
}

export interface DeskInformation {
  deskName: string;
  locationName: string;
}

export interface LocationName {
  name: string;
}

export interface UserCredentials {
  username: string;
  password: string;
}

export interface UserInfoOutput {
  username: string;
  isAdmin: boolean;
  bookedDesk: Desk;
  bookedDeskLocation: string;
}

export interface TokenOutput {
  token: string;
}

export interface DeskAvailabilityInformation {
  deskName: string;
  locationName: string;
  isEnabled: boolean;
}

export interface DeskIsEnabled {
  isEnabled: boolean;
}
