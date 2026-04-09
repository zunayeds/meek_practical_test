import { Component, inject, signal } from '@angular/core';

import { StudentService } from '../../../../core/services/student.service'; 
import { ViewLayoutComponent } from '../../../../layouts/view-layout/view-layout.component';
import { Student } from '../../../../core/models/student.model';
import { TabsModule } from 'primeng/tabs';
import { DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';


@Component({
  selector: 'app-student-view',
  imports: [ViewLayoutComponent, TabsModule, DatePipe, CardModule],
  templateUrl: './student-view.component.html'
})
export class StudentViewComponent {
  readonly studentService = inject(StudentService);

  student = signal<Student | undefined>(undefined);

  onDataLoaded(result: any) {
    this.student.set(result);
  }
}
