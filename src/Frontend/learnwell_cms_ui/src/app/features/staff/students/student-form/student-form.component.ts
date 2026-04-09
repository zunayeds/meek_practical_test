import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

import { FormLayoutComponent } from '../../../../layouts/form-layout/form-layout.component';
import { TextareaModule } from 'primeng/textarea';
import { StudentService } from '../../../../core/services/student.service';
import { MessageService } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { CreateStudentResponse, Student } from '../../../../core/models/student.model';
import { ToastModule } from 'primeng/toast';


@Component({
  selector: 'app-student-form',
  imports: [FormLayoutComponent, InputTextModule, TextareaModule, FormsModule, ReactiveFormsModule, ToastModule],
  templateUrl: './student-form.component.html'
})
export class StudentFormComponent {
  private readonly builder = inject(FormBuilder);
  readonly studentService = inject(StudentService);
  private readonly messageService = inject(MessageService);
  private readonly route = inject(ActivatedRoute);
  
  form = this.builder.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    emailAddress: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
    phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]+'), Validators.maxLength(20)]],
    address: ['', [Validators.required, Validators.maxLength(200)]]
  });
  id: string = '';

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id' ) || '';
  }

  onSuccessfullySaved({ item, response }: { item: any, response: any }): void {
    if (this.id === '') {
      const studentResponse = response as CreateStudentResponse;
      const student = item as Student;

      this.messageService.add({
        severity: 'info',
        summary: 'Password generated',
        detail: `Initial password for ${student.firstName} is ${studentResponse.password}`,
        icon: 'pi pi-key',
        sticky: true
      });
    }
  }
}
