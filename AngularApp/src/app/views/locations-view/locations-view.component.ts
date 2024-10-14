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
    private route: ActivatedRoute
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

    dialogRef.afterClosed().subscribe((result: string) => {
      if (result) {
        this.handlePopupSubmit(result);
      }
    });
  }

  handleChooseLocationButtonClick(locationName: string): void {
    this.router.navigate([`/locations/${locationName}/desks`]);
  }

  handleRemoveButtonClick(locationName: string): void {
    this.apiService.removeLocation(locationName).subscribe(
      (statusCode: number) => {
        if (statusCode === 200) {
          this.locations = this.locations.filter(
            (location) => location.locationName !== locationName
          );
        } else {
          console.error(`Could not remove the location ${locationName}`);
        }
      },
      (error: any) => {
        console.error('Unexpected error', error);
      }
    );
  }

  handlePopupSubmit(locationName: string): void {
    this.apiService.addLocation(locationName).subscribe(
      (statusCode: number) => {
        if (statusCode === 201) {
          const newLocation: Location = {
            locationName,
            totalDeskCount: 0,
            availableDeskCount: 0,
          };
          this.locations = [...this.locations, newLocation];
        } else {
          console.error(`Could not add the location: ${locationName}`);
        }
      },
      (error: any) => {
        console.error('Unexpected error', error);
      }
    );
  }
}
