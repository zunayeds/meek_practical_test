import { Component, ViewChild, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Table, TableLazyLoadEvent, TableModule } from 'primeng/table';
import { MessageService } from 'primeng/api';

import { CourseService } from '../../../../core/services/course.service';
import { CourseBase } from '../../../../core/models/course.model';
import { ListPageLayoutComponent } from '../../../../layouts/list-page-layout/list-page-layout.component';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-course-list',
  imports: [ListPageLayoutComponent, TableModule, ButtonModule, InputTextModule, FormsModule],
  templateUrl: './course-list.component.html',
  providers: [MessageService]
})
export class CourseListComponent {
  @ViewChild('dataTable') dataTable!: Table;

  private readonly courseService = inject(CourseService);
  private readonly messageService = inject(MessageService);

  courses = signal<CourseBase[]>([]);
  loading = signal(false);
  totalRecords = signal(0);
  nameFilter = '';

  readonly pageSize = PAGE_SIZE;

  deleteDialogVisible = signal(false);
  deleteLoading = signal(false);
  selectedId = signal('');

  onLazyLoad(event: TableLazyLoadEvent): void {
    const page = Math.floor((event.first ?? 0) / PAGE_SIZE) + 1;
    this.loadCourses(page);
  }

  loadCourses(page: number): void {
    this.loading.set(true);
    this.courseService.getAll(this.nameFilter, page, PAGE_SIZE).subscribe({
      next: items => {
        this.courses.set(items);
        this.totalRecords.set(
          items.length === PAGE_SIZE
            ? page * PAGE_SIZE + 1
            : (page - 1) * PAGE_SIZE + items.length
        );
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load courses.' });
        this.loading.set(false);
      }
    });
  }

  search(): void {
    this.dataTable.first = 0;
    this.loadCourses(1);
  }

  clearFilters(): void {
    this.nameFilter = '';
    this.search();
  }
}
