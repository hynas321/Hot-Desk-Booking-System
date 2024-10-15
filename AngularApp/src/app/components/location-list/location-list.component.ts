import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Location } from '../../models/Location';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '../shared/button/button.component';
import { User } from '../../models/User';

@Component({
  selector: 'app-location-list',
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.scss'],
  standalone: true,
  imports: [CommonModule, ButtonComponent],
})
export class LocationListComponent {
  @Input() locations: Location[] = [];
  @Input() user: User | null = null;

  @Output() onChooseClick = new EventEmitter<string>();
  @Output() onRemoveClick = new EventEmitter<string>();

  handleChooseClick(locationName: string): void {
    this.onChooseClick.emit(locationName);
  }

  handleRemoveClick(locationName: string): void {
    this.onRemoveClick.emit(locationName);
  }
}
