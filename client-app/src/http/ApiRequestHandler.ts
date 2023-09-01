import config from './../config.json';
import ApiEndpoints from './ApiEndpoints';
import ApiHeaders from './ApiHeaders';
import { BookingInformation, DeskAvailabilityInformation, DeskInformation, LocationName, TokenOutput, UserCredentials, UserInfoOutput } from './ApiInterfaces';
import { Location } from '../types/Location'
import { Desk } from '../types/Desk';

class HttpRequestHandler {
  private httpServerUrl: string = config.httpServerURL;

  //User requests
  async logIn(username: string, password: string): Promise<any> {
    const requestBody: UserCredentials = {
        username: username,
        password: password
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.logIn}`, {
        method: 'POST',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return await response.json() as TokenOutput;
    }
    catch (error) {
      return error;
    }
  }

  async logOut(token: string): Promise<any> {
    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.logOut}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return response.status;
    }
    catch (error) {
      return error;
    }
  }

  async getUserInfo(token: string): Promise<any> {
    try {
        const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.getUserInfo}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            [ApiHeaders.token]: token
          }
        });
  
        if (!response.ok) {
          throw new Error("Error");
        } 
  
        return await response.json() as UserInfoOutput;
    }
    catch (error) {
      return error;
    }
  }
  

  //Location requests
  async addLocation(token: string, name: string): Promise<any> {
    const requestBody: LocationName = {
      name: name
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.addLocation}`, {
        method: 'POST',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return response.status;
    }
    catch (error) {
      return error;
    }
  }

  async removeLocation(token: string, name: string): Promise<any> {
    const requestBody: LocationName = {
      name: name
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.removeLocation}`, {
        method: 'DELETE',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });
      
      if (!response.ok) {
        throw new Error("Error");
      } 

      return response.status;
    }
    catch (error) {
      return error;
    }
  }

  async getAllLocationNames(): Promise<any> {
    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.getAllLocationNames}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return await response.json() as Location[];
    }
    catch (error) {
      return error;
    }
  }

  async getDesks(token: string, locationName: string): Promise<any> {
    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.getDesks}/${locationName}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return await response.json() as Desk[];
    }
    catch (error) {
      return error;
    }
  }

  //Desk requests
  async addDesk(token: string, deskName: string, locationName: string): Promise<any> {
    const requestBody: DeskInformation = {
      deskName: deskName,
      locationName: locationName
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.addDesk}`, {
        method: 'POST',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return response.status;
    }
    catch (error) {
      return error;
    }
  }

  async removeDesk(token: string, deskName: string, locationName: string): Promise<any> {
    const requestBody: DeskInformation = {
      deskName: deskName,
      locationName: locationName
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.removeDesk}`, {
        method: 'DELETE',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return response.status;
    }
    catch (error) {
      return error;
    }
  }

  async bookDesk(token: string, deskName: string, locationName: string, days: number): Promise<any> {
    const requestBody: BookingInformation = {
      deskName: deskName,
      locationName: locationName,
      days: days
    }

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.bookDesk}`, {
        method: 'PUT',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return await response.json() as Desk;
    }
    catch (error) {
      return error;
    }
  }

  async unbookDesk(token: string, deskName: string, locationName: string): Promise<any> {
    const requestBody: DeskInformation = {
      deskName: deskName,
      locationName: locationName
    }

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.unbookDesk}`, {
        method: 'PUT',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return await response.json() as Desk;
    }
    catch (error) {
      return error;
    }
  }

  async setDeskAvailability(token: string, deskName: string, locationName: string, isEnabled: boolean): Promise<any> {
    const requestBody: DeskAvailabilityInformation = {
      deskName: deskName,
      locationName: locationName,
      isEnabled: isEnabled
    }

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.setDeskAvailability}`, {
        method: 'PUT',
        body: JSON.stringify(requestBody),
        headers: {
          'Content-Type': 'application/json',
          [ApiHeaders.token]: token
        }
      });

      if (!response.ok) {
        throw new Error("Error");
      } 

      return await response.json() as Desk;
    }
    catch (error) {
      return error;
    }
  }
}

export default HttpRequestHandler;