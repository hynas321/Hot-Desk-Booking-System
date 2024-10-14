import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TokenService } from './token.service';
import { Desk } from '../models/Desk';
import {
  BookingInformation,
  DeskInformation,
  LocationName,
  TokenOutput,
  UserCredentials,
  UserInfoOutput,
} from '../api/models';
import { ApiEndpoints } from '../api/endpoints';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly httpServerUrl: string = 'http://127.0.0.1:5062';

  constructor(private http: HttpClient, private tokenService: TokenService) {}

  private getHeaders(): HttpHeaders {
    const token = this.tokenService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  logIn(username: string, password: string): Observable<TokenOutput> {
    const requestBody: UserCredentials = { username, password };
    return this.http.post<TokenOutput>(
      `${this.httpServerUrl}${ApiEndpoints.logIn}`,
      requestBody,
      {
        headers: { 'Content-Type': 'application/json' },
      }
    );
  }

  logOut(): Observable<void> {
    return this.http
      .put<void>(
        `${this.httpServerUrl}${ApiEndpoints.logOut}`,
        {},
        {
          headers: this.getHeaders(),
        }
      )
      .pipe(
        catchError((error) => {
          console.error('Error during logout:', error);
          throw error;
        })
      );
  }

  getUserInfo(): Observable<UserInfoOutput> {
    return this.http.get<UserInfoOutput>(
      `${this.httpServerUrl}${ApiEndpoints.getUserInfo}`,
      {
        headers: this.getHeaders(),
      }
    );
  }

  addLocation(name: string): Observable<any> {
    const requestBody: LocationName = { name };
    return this.http.post(
      `${this.httpServerUrl}${ApiEndpoints.addLocation}`,
      requestBody,
      {
        headers: this.getHeaders(),
      }
    );
  }

  removeLocation(name: string): Observable<any> {
    const requestBody: LocationName = { name };
    return this.http.delete(
      `${this.httpServerUrl}${ApiEndpoints.removeLocation}`,
      {
        headers: this.getHeaders(),
        body: requestBody,
      }
    );
  }

  getAllLocationNames(): Observable<Location[]> {
    return this.http.get<Location[]>(
      `${this.httpServerUrl}${ApiEndpoints.getAllLocationNames}`,
      {
        headers: this.getHeaders(),
      }
    );
  }

  getDesks(locationName: string): Observable<Desk[]> {
    return this.http.get<Desk[]>(
      `${this.httpServerUrl}${ApiEndpoints.getDesks}/${locationName}`,
      {
        headers: this.getHeaders(),
      }
    );
  }

  addDesk(deskName: string, locationName: string): Observable<any> {
    const requestBody: DeskInformation = { deskName, locationName };
    return this.http.post(
      `${this.httpServerUrl}${ApiEndpoints.addDesk}`,
      requestBody,
      {
        headers: this.getHeaders(),
      }
    );
  }

  removeDesk(deskName: string, locationName: string): Observable<any> {
    const requestBody: DeskInformation = { deskName, locationName };
    return this.http.delete(`${this.httpServerUrl}${ApiEndpoints.removeDesk}`, {
      headers: this.getHeaders(),
      body: requestBody,
    });
  }

  bookDesk(
    deskName: string,
    locationName: string,
    days: number
  ): Observable<Desk> {
    const requestBody: BookingInformation = { deskName, locationName, days };
    return this.http.put<Desk>(
      `${this.httpServerUrl}${ApiEndpoints.bookDesk}`,
      requestBody,
      {
        headers: this.getHeaders(),
      }
    );
  }

  unbookDesk(deskName: string, locationName: string): Observable<Desk> {
    const requestBody: DeskInformation = { deskName, locationName };
    return this.http.put<Desk>(
      `${this.httpServerUrl}${ApiEndpoints.unbookDesk}`,
      requestBody,
      {
        headers: this.getHeaders(),
      }
    );
  }

  setDeskAvailability(
    deskName: string,
    locationName: string,
    isEnabled: boolean
  ): Observable<Desk> {
    const requestBody: any = { deskName, locationName, isEnabled };
    return this.http.put<Desk>(
      `${this.httpServerUrl}${ApiEndpoints.setDeskAvailability}`,
      requestBody,
      {
        headers: this.getHeaders(),
      }
    );
  }

  refreshToken(): Observable<TokenOutput> {
    return this.http
      .post<TokenOutput>(
        `${this.httpServerUrl}${ApiEndpoints.refreshToken}`,
        {},
        {
          headers: this.getHeaders(),
        }
      )
      .pipe(
        catchError((error) => {
          console.error('Error refreshing token', error);
          throw error;
        })
      );
  }
}
