import { Component, ViewChild, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Table, TableLazyLoadEvent } from 'primeng/table';
import { MessageService } from 'primeng/api';

import { ClassService } from '../../../../core/services/class.service';
import { ClassBase } from '../../../../core/models/class.model';
import { ListPageLayoutComponent } from '../../../../layouts/list-page-layout/list-page-layout.component';
import { ColumnModel } from '../../../../core/models/column.model';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-class-list',
  imports: [ListPageLayoutComponent, ButtonModule, InputTextModule, FormsModule],   
  templateUrl: './class-list.component.html',
  providers: [MessageService]
})
export class ClassListComponent {
  @ViewChild('dataTable') dataTable!: Table;

  private readonly classService = inject(ClassService);
  private readonly messageService = inject(MessageService);

  classes = signal<ClassBase[]>([]);
  loading = signal(false);
  columns = signal<ColumnModel[]>([
    { header: 'Name', field: 'name', type: 'string' },
    { header: 'Description', field: 'description', type: 'string' },
    { header: 'Actions', field: 'actions', type: 'action' }
  ]);
  totalRecords = signal(0);
  nameFilter = '';

  readonly pageSize = PAGE_SIZE;

  deleteDialogVisible = signal(false);
  deleteLoading = signal(false);
  selectedId = signal('');

  onLazyLoad(event: TableLazyLoadEvent): void {
    const page = Math.floor((event.first ?? 0) / PAGE_SIZE) + 1;
    this.loadClasses(page);
  }

  loadClasses(page: number): void {
    this.loading.set(true);
    this.classService.getAll(this.nameFilter, page, PAGE_SIZE).subscribe({
      next: items => {
        this.classes.set(items);
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
    this.loadClasses(1);
  }

  clearFilters(): void {
    this.nameFilter = '';
    this.search();
  }
}
