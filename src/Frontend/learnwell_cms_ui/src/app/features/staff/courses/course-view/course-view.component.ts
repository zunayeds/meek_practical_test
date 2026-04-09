import { Component, inject, signal } from '@angular/core';

import { CourseService } from '../../../../core/services/course.service';
import { ViewLayoutComponent } from '../../../../layouts/view-layout/view-layout.component';
import { Course } from '../../../../core/models/course.model';
import { TabsModule } from 'primeng/tabs';
import { DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ListTableComponent } from '../../../../shared/components/list-table/list-table.component';
import { ClassBase } from '../../../../core/models/class.model';
import { ColumnModel } from '../../../../core/models/column.model';
import { StudentBase } from '../../../../core/models/student.model';


@Component({
  selector: 'app-course-view',
  imports: [ViewLayoutComponent, TabsModule, DatePipe, CardModule, ListTableComponent, ButtonModule],
  templateUrl: './course-view.component.html'
})
export class CourseViewComponent {
  readonly courseService = inject(CourseService);

  course = signal<Course | undefined>(undefined);

  classes = signal<ClassBase[]>([]);
  classesloading = signal(false);
  classColumns: ColumnModel[] = [
    { field: 'name', header: 'Name', type: 'string' },
    { field: 'description', header: 'Description', type: 'string' },
  ];
  classLoaded = signal(false);

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
    if (tab === 'basic' || !this.course()) {
      return;
    } else if (tab === 'classes' && !this.classLoaded()) {
      this.classLoaded.set(true);
      this.classesloading.set(true);
      this.courseService.getClasses(this.course()!.courseId).subscribe(result => {
        this.classesloading.set(false);
        this.classes.set(result);
      });
    } else if (tab === 'students' && !this.studentLoaded()) {
      this.studentLoaded.set(true);
      this.studentsloading.set(true);
      this.courseService.getStudents(this.course()!.courseId).subscribe(result => {
        this.studentsloading.set(false);
        this.students.set(result);
      });
    }
  }

  onDataLoaded(result: any) {
    this.course.set(result);
  }
}
