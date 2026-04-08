import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

import { CourseService } from '../../../../core/services/course.service';
import { FormLayoutComponent } from '../../../../layouts/form-layout/form-layout.component';
import { TextareaModule } from 'primeng/textarea';


@Component({
  selector: 'app-course-form',
  imports: [FormLayoutComponent, InputTextModule, TextareaModule, FormsModule, ReactiveFormsModule],
  templateUrl: './course-form.component.html'
})
export class CourseFormComponent {
  private readonly builder = inject(FormBuilder);
  
  form = this.builder.group({
    name: ['', [Validators.required, Validators.maxLength(30)]],
    description: ['', Validators.maxLength(100)]
  });

  readonly courseService = inject(CourseService);
}
