import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../material/material.module';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.scss'],
  standalone: true,
  imports: [CommonModule, MaterialModule, ReactiveFormsModule],
})
export class PopupComponent implements OnInit {
  @Output() formSubmitted = new EventEmitter<string>();
  popupForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<PopupComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: { title: string; inputFormPlaceholderText: string }
  ) {}

  ngOnInit(): void {
    this.popupForm = this.fb.group({
      inputValue: ['', [Validators.required, Validators.minLength(1)]],
    });

    this.popupForm.valueChanges.subscribe(() => {
      this.checkFormValidity();
    });
  }

  checkFormValidity(): boolean {
    return this.popupForm.valid;
  }

  onSubmit(): void {
    if (this.popupForm.invalid) {
      return;
    }

    const inputValue = this.popupForm.get('inputValue')?.value;
    this.formSubmitted.emit(inputValue);
    this.dialogRef.close();
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
