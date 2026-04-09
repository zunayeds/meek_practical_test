import { Component, ViewChild, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Table, TableLazyLoadEvent } from 'primeng/table';
import { MessageService } from 'primeng/api';

import { StudentService } from '../../../../core/services/student.service';
import { StudentBase } from '../../../../core/models/student.model';
import { ListPageLayoutComponent } from '../../../../layouts/list-page-layout/list-page-layout.component';
import { ColumnModel } from '../../../../core/models/column.model';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-student-list',
  imports: [ListPageLayoutComponent, ButtonModule, InputTextModule, FormsModule],
  templateUrl: './student-list.component.html'
})
export class StudentListComponent {
  @ViewChild('dataTable') dataTable!: Table;

  readonly studentService = inject(StudentService);
  private readonly messageService = inject(MessageService);

  students = signal<StudentBase[]>([]);
  columns = signal<ColumnModel[]>([
    { header: 'First Name', field: 'firstName', type: 'string' },
    { header: 'Last Name', field: 'lastName', type: 'string' },
    { header: 'Email', field: 'emailAddress', type: 'string' },
    { header: 'Phone Number', field: 'phoneNumber', type: 'string' },
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
  emailFilter = '';
  phoneNumberFilter = '';

  readonly pageSize = PAGE_SIZE;

  deleteDialogVisible = signal(false);
  deleteLoading = signal(false);
  selectedId = signal('');

  onLazyLoad(event: TableLazyLoadEvent): void {
    const page = Math.floor((event.first ?? 0) / PAGE_SIZE) + 1;
    this.loadStudents(page);
  }

  loadStudents(page: number): void {
    this.loading.set(true);
    this.studentService.getAll(this.nameFilter, this.emailFilter, this.phoneNumberFilter, page, PAGE_SIZE).subscribe({
      next: result => {
        this.students.set(result.records);
        this.totalRecords.set(result.totalRecords);
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load students.' });
        this.loading.set(false);
      }
    });
  }

  search(): void {
    this.loadStudents(1);
  }

  clearFilters(): void {
    this.nameFilter = '';
    this.search();
  }
}
