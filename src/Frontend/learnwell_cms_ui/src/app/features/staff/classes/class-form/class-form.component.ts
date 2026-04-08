import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

import { FormLayoutComponent } from '../../../../layouts/form-layout/form-layout.component';
import { TextareaModule } from 'primeng/textarea';
import { ClassService } from '../../../../core/services/class.service';


@Component({
  selector: 'app-class-form',
  imports: [FormLayoutComponent, InputTextModule, TextareaModule, FormsModule, ReactiveFormsModule],
  templateUrl: './class-form.component.html'
})
export class ClassFormComponent {
  private readonly builder = inject(FormBuilder);
  
  form = this.builder.group({
    name: ['', [Validators.required, Validators.maxLength(30)]],
    description: ['', Validators.maxLength(100)]
  });

  readonly classService = inject(ClassService);
}
