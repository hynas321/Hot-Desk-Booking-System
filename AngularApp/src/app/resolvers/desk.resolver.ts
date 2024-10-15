import { Injectable } from '@angular/core';
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { Observable } from 'rxjs';
import { ApiService } from '../services/api.service';
import { Desk } from '../models/Desk';

@Injectable({
  providedIn: 'root',
})
export class DesksResolver implements Resolve<Desk[]> {
  constructor(private apiService: ApiService) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<Desk[]> {
    const locationName = route.paramMap.get('locationName');
    return this.apiService.getDesks(locationName!);
  }
}
