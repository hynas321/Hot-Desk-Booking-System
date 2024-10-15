import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-range',
  templateUrl: './range.component.html',
  styleUrls: ['./range.component.scss'],
  standalone: true,
  imports: [],
})
export class RangeComponent implements OnInit {
  @Input() title: string = '';
  @Input() suffix: string = '';
  @Input() minValue: number = 0;
  @Input() maxValue: number = 100;
  @Input() step: number = 1;
  @Input() defaultValue: number = 50;
  @Output() valueChange = new EventEmitter<number>();

  value: number = this.defaultValue;

  ngOnInit() {
    this.value = this.defaultValue;
  }

  onRangeChange(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.value = parseInt(inputElement.value);
    this.valueChange.emit(this.value);
  }
}
