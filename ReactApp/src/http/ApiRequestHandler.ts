import config from "./../config.json";
import ApiEndpoints from "./ApiEndpoints";
import {
  BookingInformation,
  DeskAvailabilityInformation,
  DeskInformation,
  LocationName,
  TokenOutput,
  UserCredentials,
  UserInfoOutput,
} from "./ApiInterfaces";
import { Location } from "../types/Location";
import { Desk } from "../types/Desk";
import { TokenManager } from "../managers/TokenManager";

export class ApiRequestHandler {
  private static instance: ApiRequestHandler;
  private readonly httpServerUrl: string = config.httpServerURL;

  private constructor() {
    this.httpServerUrl = config.httpServerURL;
  }

  public static getInstance(): ApiRequestHandler {
    if (!ApiRequestHandler.instance) {
      ApiRequestHandler.instance = new ApiRequestHandler();
    }
    return ApiRequestHandler.instance;
  }

  // User requests
  async logIn(username: string, password: string): Promise<any> {
    const requestBody: UserCredentials = {
      username: username,
      password: password,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.logIn}`, {
        method: "POST",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      const tokenOutput = (await response.json()) as TokenOutput;
      TokenManager.setToken(tokenOutput.token);

      return tokenOutput;
    } catch (error) {
      return error;
    }
  }

  async logOut(): Promise<any> {
    try {
      const token = TokenManager.getToken();
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.logOut}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      TokenManager.clearToken();

      return response.status;
    } catch (error) {
      return error;
    }
  }

  async getUserInfo(): Promise<any> {
    try {
      const token = TokenManager.getToken();
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.getUserInfo}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return (await response.json()) as UserInfoOutput;
    } catch (error) {
      return error;
    }
  }

  async addLocation(name: string): Promise<any> {
    const token = TokenManager.getToken();
    const requestBody: LocationName = {
      name: name,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.addLocation}`, {
        method: "POST",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return response.status;
    } catch (error) {
      return error;
    }
  }

  async removeLocation(name: string): Promise<any> {
    const token = TokenManager.getToken();
    const requestBody: LocationName = {
      name: name,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.removeLocation}`, {
        method: "DELETE",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return response.status;
    } catch (error) {
      return error;
    }
  }

  async getAllLocationNames(): Promise<any> {
    const token = TokenManager.getToken();
    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.getAllLocationNames}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return (await response.json()) as Location[];
    } catch (error) {
      return error;
    }
  }

  async getDesks(locationName: string): Promise<any> {
    const token = TokenManager.getToken();
    try {
      const response = await fetch(
        `${this.httpServerUrl}${ApiEndpoints.getDesks}/${locationName}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Error");
      }

      return (await response.json()) as Desk[];
    } catch (error) {
      return error;
    }
  }

  async addDesk(deskName: string, locationName: string): Promise<any> {
    const token = TokenManager.getToken();
    const requestBody: DeskInformation = {
      deskName: deskName,
      locationName: locationName,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.addDesk}`, {
        method: "POST",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return response.status;
    } catch (error) {
      return error;
    }
  }

  async removeDesk(deskName: string, locationName: string): Promise<any> {
    const token = TokenManager.getToken();
    const requestBody: DeskInformation = {
      deskName: deskName,
      locationName: locationName,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.removeDesk}`, {
        method: "DELETE",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return response.status;
    } catch (error) {
      return error;
    }
  }

  async bookDesk(deskName: string, locationName: string, days: number): Promise<any> {
    const token = TokenManager.getToken();
    const requestBody: BookingInformation = {
      deskName: deskName,
      locationName: locationName,
      days: days,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.bookDesk}`, {
        method: "PUT",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return (await response.json()) as Desk;
    } catch (error) {
      return error;
    }
  }

  async unbookDesk(deskName: string, locationName: string): Promise<any> {
    const token = TokenManager.getToken();
    const requestBody: DeskInformation = {
      deskName: deskName,
      locationName: locationName,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.unbookDesk}`, {
        method: "PUT",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return (await response.json()) as Desk;
    } catch (error) {
      return error;
    }
  }

  async setDeskAvailability(
    deskName: string,
    locationName: string,
    isEnabled: boolean
  ): Promise<any> {
    const token = TokenManager.getToken();
    const requestBody: DeskAvailabilityInformation = {
      deskName: deskName,
      locationName: locationName,
      isEnabled: isEnabled,
    };

    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.setDeskAvailability}`, {
        method: "PUT",
        body: JSON.stringify(requestBody),
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error");
      }

      return (await response.json()) as Desk;
    } catch (error) {
      return error;
    }
  }

  async refreshToken(): Promise<void> {
    const token = TokenManager.getToken();
    try {
      const response = await fetch(`${this.httpServerUrl}${ApiEndpoints.refreshToken}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Error refreshing token");
      }

      const tokenOutput = (await response.json()) as TokenOutput;
      TokenManager.setToken(tokenOutput.token);
    } catch (error) {
      console.error("Token refresh failed", error);
      throw error;
    }
  }
}

export default ApiRequestHandler;
