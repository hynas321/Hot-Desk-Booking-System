import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ApiService } from '../../services/api.service';
import { PopupComponent } from '../../components/popup/popup.component';
import { Location } from '../../models/Location';
import { LocationListComponent } from '../../components/location-list/location-list.component';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { User } from '../../models/User';
import { UserService } from '../../services/user.service';
import { ButtonComponent } from '../../components/shared/button/button.component';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-locations-view',
  templateUrl: './locations-view.component.html',
  styleUrl: './locations-view.component.scss',
  standalone: true,
  imports: [CommonModule, LocationListComponent, ButtonComponent],
})
export class LocationsViewComponent implements OnInit {
  locations: Location[] = [];
  isAddLocationPopupVisible = false;
  isLocationListVisible = false;
  user$: Observable<User | null>;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private dialog: MatDialog,
    private userService: UserService,
    private route: ActivatedRoute,
    private alertService: AlertService
  ) {
    this.user$ = userService.user$;
  }

  ngOnInit(): void {
    const resolvedLocations = this.route.snapshot.data['locations'];
    this.locations = resolvedLocations.map((location: Location) => ({
      locationName: location.locationName,
      totalDeskCount: location.totalDeskCount,
      availableDeskCount: location.availableDeskCount,
    }));

    setTimeout(() => {
      this.isLocationListVisible = true;
    }, 250);
  }

  handleAddButtonClick(): void {
    const dialogRef = this.dialog.open(PopupComponent, {
      data: {
        title: 'Name a new location',
        inputFormPlaceholderText: 'Insert the name here',
      },
    });

    dialogRef.componentInstance.formSubmitted.subscribe(
      (locationName: string) => {
        this.handlePopupSubmit(locationName);
      }
    );
  }

  handlePopupSubmit(locationName: string): void {
    this.apiService.addLocation(locationName).subscribe(() => {
      const newLocation: Location = {
        locationName: locationName,
        totalDeskCount: 0,
        availableDeskCount: 0,
      };
      this.locations.push(newLocation);
      this.alertService.showAlert(
        `Location "${locationName}" has been added`,
        'success'
      );
    });
  }

  handleChooseLocationButtonClick(locationName: string): void {
    this.router.navigate([`/locations/${locationName}/desks`]);
  }

  handleRemoveButtonClick(locationName: string): void {
    this.apiService.removeLocation(locationName).subscribe(() => {
      this.locations = this.locations.filter(
        (location) => location.locationName !== locationName
      );
      this.alertService.showAlert(
        `Location "${locationName}" has been removed`,
        'success'
      );
    });
  }
}
