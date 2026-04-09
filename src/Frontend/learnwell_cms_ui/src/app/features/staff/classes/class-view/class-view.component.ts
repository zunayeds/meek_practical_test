import { Component, inject, signal } from '@angular/core';

import { ClassService } from '../../../../core/services/class.service'; 
import { ViewLayoutComponent } from '../../../../layouts/view-layout/view-layout.component';
import { Class } from '../../../../core/models/class.model';
import { TabsModule } from 'primeng/tabs';
import { DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';
import { CourseBase } from '../../../../core/models/course.model';
import { ColumnModel } from '../../../../core/models/column.model';
import { StudentBase } from '../../../../core/models/student.model';
import { ListTableComponent } from '../../../../shared/components/list-table/list-table.component';


@Component({
  selector: 'app-class-view',
  imports: [ViewLayoutComponent, TabsModule, ListTableComponent, DatePipe, CardModule],
  templateUrl: './class-view.component.html'
})
export class ClassViewComponent {
  readonly classService = inject(ClassService);

  class = signal<Class | undefined>(undefined);

  courses = signal<CourseBase[]>([]);
  coursesloading = signal(false);
  courseColumns: ColumnModel[] = [
    { field: 'name', header: 'Name', type: 'string' },
    { field: 'description', header: 'Description', type: 'string' },
  ];
  courseLoaded = signal(false);

  students = signal<StudentBase[]>([]);
  studentsloading = signal(false);
  studentColumns: ColumnModel[] = [
    { field: 'firstName', header: 'First Name', type: 'string' },
    { field: 'lastName', header: 'Last Name', type: 'string' },
    { field: 'emailAddress', header: 'Email', type: 'string' },
    { field: 'phoneNumber', header: 'Phone', type: 'string' },
  ];
  studentLoaded = signal(false);

  onTabChange(tab: any) {
    if (tab === 'basic' || !this.class()) {
      return;
    } else if (tab === 'courses' && !this.courseLoaded()) {
      this.courseLoaded.set(true);
      this.coursesloading.set(true);
      this.classService.getCourses(this.class()!.classId).subscribe(result => {
        this.coursesloading.set(false);
        this.courses.set(result);
      });
    } else if (tab === 'students' && !this.studentLoaded()) {
      this.studentLoaded.set(true);
      this.studentsloading.set(true);
      this.classService.getStudents(this.class()!.classId).subscribe(result => {
        this.studentsloading.set(false);
        this.students.set(result);
      });
    }
  }

  onDataLoaded(result: any) {
    this.class.set(result);
  }
}
