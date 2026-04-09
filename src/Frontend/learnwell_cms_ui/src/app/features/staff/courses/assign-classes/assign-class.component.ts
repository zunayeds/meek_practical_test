import { Component, inject, OnInit, signal } from '@angular/core';

import { CourseService } from '../../../../core/services/course.service';
import { AssignmentLayoutComponent } from '../../../../layouts/assignment-layout/assignment-layout.component';
import { PickListItem } from '../../../../core/models/pick-list.mode';
import { map } from 'rxjs';
import { ClassService } from '../../../../core/services/class.service';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-assign-class',
  imports: [AssignmentLayoutComponent],
  templateUrl: './assign-class.component.html'
})
export class AssignClassComponent implements OnInit {
  private readonly courseService = inject(CourseService);
  private readonly classService = inject(ClassService);
  private readonly route = inject(ActivatedRoute);

  entityName = signal('');

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.courseService.getById(id).subscribe(course => this.entityName.set(course.name));
  }

  loadAssignedClasses = (courseId: string) =>
    this.courseService.getClasses(courseId).pipe(
      map(items => items.map(c => ({ id: c.classId, label: c.name } as PickListItem)))
    );

  loadClassSourcePage = (page: number) =>
    this.classService.getAll('', page, 10).pipe(
      map(res => res.records.map(c => ({ id: c.classId, label: c.name } as PickListItem)))
    );

  saveClassAssignments = (courseId: string, addIds: string[], removeIds: string[]) =>
    this.courseService.addRemoveClasses(courseId, { addClassIds: addIds, removeClassIds: removeIds });
}
