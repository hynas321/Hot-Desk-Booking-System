import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
  standalone: true,
  imports: [CommonModule],
})
export class ButtonComponent {
  @Input() text: string = '';
  @Input() active: boolean = false;
  @Input() spacing: number = 0;
  @Input() type: string = 'primary';
  @Input() icon?: string;

  @Output() onClick = new EventEmitter<void>();

  handleClick() {
    if (this.active) {
      this.onClick.emit();
    }
  }

  getButtonClasses(): string {
    const typeClass = `btn-${this.type ? this.type : 'primary'}`;
    const spacingClass = `mx-${this.spacing}`;
    const disabledClass = this.active ? '' : 'disabled';

    return `btn ${typeClass} ${spacingClass} ${disabledClass}`;
  }
}
