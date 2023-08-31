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

export interface UserUsername {
  username: string;
}

export interface TokenOutput {
  token: string;
}