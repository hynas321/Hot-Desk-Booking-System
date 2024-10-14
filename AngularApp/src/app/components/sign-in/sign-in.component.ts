import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from "../shared/button/button.component";

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent],
})
export class SigninComponent implements OnInit {
  signinForm!: FormGroup;
  isButtonActive: boolean = false;

  @Output() formSubmitted: EventEmitter<{
    username: string;
    password: string;
  }> = new EventEmitter();

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.signinForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(5)]],
      password: ['', [Validators.required, Validators.minLength(5)]],
    });

    this.signinForm.valueChanges.subscribe(() => {
      this.checkButtonActive();
    });
  }

  checkButtonActive(): void {
    this.isButtonActive = this.signinForm.valid;
  }

  submitForm(): void {
    if (this.signinForm.invalid) {
      return;
    }

    const { username, password } = this.signinForm.value;
    this.formSubmitted.emit({ username, password });
  }
}
