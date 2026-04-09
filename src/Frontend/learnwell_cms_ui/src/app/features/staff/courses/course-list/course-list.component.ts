import { Component, ViewChild, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Table, TableLazyLoadEvent } from 'primeng/table';
import { MessageService } from 'primeng/api';

import { CourseService } from '../../../../core/services/course.service';
import { CourseBase } from '../../../../core/models/course.model';
import { ListPageLayoutComponent } from '../../../../layouts/list-page-layout/list-page-layout.component';
import { ColumnModel } from '../../../../core/models/column.model';
import { ListExtraActionsDirective } from '../../../../shared/directives/list-extra-actions.directive';
import { Router } from '@angular/router';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-course-list',
  imports: [ListPageLayoutComponent, ListExtraActionsDirective, ButtonModule, InputTextModule, FormsModule],
  templateUrl: './course-list.component.html'
})
export class CourseListComponent {
  @ViewChild('dataTable') dataTable!: Table;

  readonly courseService = inject(CourseService);
  private readonly messageService = inject(MessageService);
  private readonly router = inject(Router);

  courses = signal<CourseBase[]>([]);
  columns = signal<ColumnModel[]>([
    { header: 'Name', field: 'name', type: 'string' },
    { header: 'Description', field: 'description', type: 'string' },
    {
      header: 'Actions',
      field: 'actions',
      type: 'action',
      actions: ['view', 'edit', 'delete']
    }
  ]);
  loading = signal(false);
  totalRecords = signal(0);
  nameFilter = '';

  readonly pageSize = PAGE_SIZE;

  deleteDialogVisible = signal(false);
  deleteLoading = signal(false);
  selectedId = signal('');

  onLazyLoad(event: TableLazyLoadEvent) {
    const page = Math.floor((event.first ?? 0) / PAGE_SIZE) + 1;
    this.loadCourses(page);
  }

  loadCourses(page: number) {
    this.loading.set(true);
    this.courseService.getAll(this.nameFilter, page, PAGE_SIZE).subscribe({
      next: result => {
        this.courses.set(result.records);
        this.totalRecords.set(result.totalRecords);
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load courses.' });
        this.loading.set(false);
      }
    });
  }

  search() {
    this.loadCourses(1);
  }

  onAssignClasses(item: CourseBase) {
    this.router.navigate(['/staff/courses', item.courseId, 'assign-classes']);
  }

  onAssignStudents(item: CourseBase) {
    this.router.navigate(['/staff/courses', item.courseId, 'assign-students']);
  }

  clearFilters() {
    this.nameFilter = '';
    this.search();
  }
}
