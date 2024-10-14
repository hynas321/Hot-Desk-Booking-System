import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root',
})
export class LocationResolver implements Resolve<Location[]> {
  constructor(private apiService: ApiService) {}

  resolve(): Observable<Location[]> {
    return this.apiService.getAllLocationNames();
  }
}
