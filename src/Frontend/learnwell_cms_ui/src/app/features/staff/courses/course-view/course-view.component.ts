import { Component, inject, signal } from '@angular/core';

import { CourseService } from '../../../../core/services/course.service';
import { ViewLayoutComponent } from '../../../../layouts/view-layout/view-layout.component';
import { Course } from '../../../../core/models/course.model';
import { TabsModule } from 'primeng/tabs';
import { DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';


@Component({
  selector: 'app-course-view',
  imports: [ViewLayoutComponent, TabsModule, DatePipe, CardModule],
  templateUrl: './course-view.component.html'
})
export class CourseViewComponent {
  readonly courseService = inject(CourseService);

  course = signal<Course | undefined>(undefined);

  onDataLoaded(result: any) {
    this.course.set(result);
  }
}
