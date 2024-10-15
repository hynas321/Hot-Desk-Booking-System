import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

type AlertType = 'success' | 'error' | 'warning' | 'info' | 'secondary';
type VerticalPosition = 'top' | 'bottom';
type HorizontalPosition = 'left' | 'center' | 'right';

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  constructor(private snackBar: MatSnackBar) {}

  showAlert(
    message: string,
    type: AlertType = 'success',
    duration: number = 3000,
    horizontalPosition: HorizontalPosition = 'left',
    verticalPosition: VerticalPosition = 'top'
  ): void {
    this.snackBar.open(message, 'Close', {
      duration: duration,
      panelClass: [`${type}-snackbar`],
      horizontalPosition: horizontalPosition,
      verticalPosition: verticalPosition,
    });
  }
}
