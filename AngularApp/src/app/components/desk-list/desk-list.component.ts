import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Desk } from '../../models/Desk';
import { User } from '../../models/User';
import { CommonModule } from '@angular/common';
import { RangeComponent } from '../shared/range/range.component';
import { ButtonComponent } from '../shared/button/button.component';

@Component({
  selector: 'app-desk-list',
  templateUrl: './desk-list.component.html',
  styleUrls: ['./desk-list.component.scss'],
  standalone: true,
  imports: [CommonModule, RangeComponent, ButtonComponent],
})
export class DeskListComponent implements OnInit {
  @Input() desks: Desk[] = [];
  @Input() user: User | null = null;

  @Output() onBookClick = new EventEmitter<string>();
  @Output() onUnbookClick = new EventEmitter<string>();
  @Output() onRemoveClick = new EventEmitter<string>();
  @Output() onEnableClick = new EventEmitter<string>();
  @Output() onDisableClick = new EventEmitter<string>();
  @Output() onRangeChange = new EventEmitter<number>();

  hoveredIndex: number | null = null;
  isHoverSupported: boolean = false;

  ngOnInit(): void {
    const mediaQuery = window.matchMedia('(hover: hover)');
    this.isHoverSupported = mediaQuery.matches;

    mediaQuery.addEventListener('change', (event) => {
      this.isHoverSupported = event.matches;
    });
  }

  handleMouseEnter(index: number): void {
    this.hoveredIndex = index;
  }

  handleMouseLeave(): void {
    this.hoveredIndex = null;
  }
}
