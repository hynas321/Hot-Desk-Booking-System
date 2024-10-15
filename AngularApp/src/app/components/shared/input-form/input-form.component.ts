import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-input-form',
  templateUrl: './input-form.component.html',
  styleUrls: ['./input-form.component.scss'],
  standalone: true,
  imports: [],
})
export class InputFormComponent implements OnInit {
  @Input() placeholderValue: string = '';
  @Input() inputType: string = 'text';
  @Output() valueChanged = new EventEmitter<string>();

  form: FormGroup;
  private valueSubject = new BehaviorSubject<string>('');

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      inputControl: [''],
    });
  }

  ngOnInit(): void {
    this.form.get('inputControl')?.valueChanges.subscribe((value) => {
      this.valueSubject.next(value);
      this.valueChanged.emit(value);
    });
  }

  get value$() {
    return this.valueSubject.asObservable();
  }
}
