import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Desk } from '../../models/Desk';
import { PopupComponent } from '../../components/popup/popup.component';
import { CommonModule } from '@angular/common';
import { DeskListComponent } from '../../components/desk-list/desk-list.component';
import { UserService } from '../../services/user.service';
import { Observable } from 'rxjs';
import { User } from '../../models/User';
import { ButtonComponent } from '../../components/shared/button/button.component';

@Component({
  selector: 'app-desks-view',
  templateUrl: './desks-view.component.html',
  styleUrls: ['./desks-view.component.scss'],
  standalone: true,
  imports: [CommonModule, DeskListComponent, ButtonComponent],
})
export class DesksViewComponent implements OnInit {
  desks: Desk[] = [];
  isPopupVisible: boolean = false;
  isDeskListVisible: boolean = false;
  bookingDays: number = 1;
  locationName: string = '-';
  user$: Observable<User | null>;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private userService: UserService
  ) {
    this.user$ = userService.user$;
  }

  ngOnInit(): void {
    this.locationName = this.route.snapshot.paramMap.get('locationName') ?? '-';
    this.desks = this.route.snapshot.data['desks'];

    setTimeout(() => {
      this.isDeskListVisible = true;
    }, 250);
  }

  handleAddButtonClick(): void {
    const dialogRef = this.dialog.open(PopupComponent, {
      data: {
        title: 'Name a new desk',
        inputFormPlaceholderText: 'Insert the name here',
      },
    });

    dialogRef.componentInstance.formSubmitted.subscribe((deskName: string) => {
      this.handlePopupSubmit(deskName); // Handle popup submission
    });
  }

  handlePopupSubmit(deskName: string): void {
    this.apiService.addDesk(deskName, this.locationName).subscribe(() => {
      const newDesk: Desk = {
        deskName,
        isEnabled: true,
        username: null,
        startTime: null,
        endTime: null,
      };
      this.desks.push(newDesk);
    });
  }

  handleBookButtonClick(deskName: string): void {
    this.apiService
      .bookDesk(deskName, this.locationName, this.bookingDays)
      .subscribe((bookedDesk: Desk) => {
        this.updateDeskState(deskName, bookedDesk);
        const user = this.userService.getUser();
        if (user) {
          const updatedUser = {
            ...user,
            bookedDesk: bookedDesk,
            bookedDeskLocation: this.locationName,
          };
          this.userService.updateUser(updatedUser);
        }
      });
  }

  handleUnbookButtonClick(deskName: string): void {
    this.apiService
      .unbookDesk(deskName, this.locationName)
      .subscribe((unbookedDesk: Desk) => {
        this.updateDeskState(deskName, unbookedDesk);
        const user = this.userService.getUser();
        if (user) {
          const updatedUser = {
            ...user,
            bookedDesk: null,
            bookedDeskLocation: null,
          };
          this.userService.updateUser(updatedUser);
        }
      });
  }

  handleRemoveButtonClick(deskName: string): void {
    this.apiService.removeDesk(deskName, this.locationName).subscribe(() => {
      this.desks = this.desks.filter((desk) => desk.deskName !== deskName);
    });
  }

  handleEnableButtonClick(deskName: string): void {
    this.apiService
      .setDeskAvailability(deskName, this.locationName, true)
      .subscribe((enabledDesk: Desk) =>
        this.updateDeskState(deskName, enabledDesk)
      );
  }

  handleDisableButtonClick(deskName: string): void {
    this.apiService
      .setDeskAvailability(deskName, this.locationName, false)
      .subscribe((disabledDesk: Desk) =>
        this.updateDeskState(deskName, disabledDesk)
      );
  }

  handleDaysRangeChange(value: number): void {
    this.bookingDays = value;
  }

  handleGoBackButtonClick(): void {
    this.router.navigate(['/locations']);
  }

  private updateDeskState(deskName: string, updatedDesk: Desk): void {
    const deskIndex = this.desks.findIndex(
      (desk) => desk.deskName === deskName
    );

    this.desks[deskIndex] = updatedDesk;
  }
}
