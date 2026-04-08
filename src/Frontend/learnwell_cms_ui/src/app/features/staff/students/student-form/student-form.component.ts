import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

import { FormLayoutComponent } from '../../../../layouts/form-layout/form-layout.component';
import { TextareaModule } from 'primeng/textarea';
import { StudentService } from '../../../../core/services/student.service';


@Component({
  selector: 'app-student-form',
  imports: [FormLayoutComponent, InputTextModule, TextareaModule, FormsModule, ReactiveFormsModule],
  templateUrl: './student-form.component.html'
})
export class StudentFormComponent {
  private readonly builder = inject(FormBuilder);
  
  form = this.builder.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    emailAddress: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
    phoneNumber: ['', [Validators.required, Validators.maxLength(20)]],
    address: ['', [Validators.required, Validators.maxLength(200)]]
  });

  readonly studentService = inject(StudentService);
}
