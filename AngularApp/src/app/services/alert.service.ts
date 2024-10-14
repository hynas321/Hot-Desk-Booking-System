import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  constructor(private snackBar: MatSnackBar) {}

  showAlert(
    message: string,
    type: string = 'success',
    duration: number = 3000
  ): void {
    this.snackBar.open(message, 'Close', {
      duration: duration,
      panelClass: [`mat-${type}`],
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }
}
