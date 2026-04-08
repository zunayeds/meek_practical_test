import { Component, inject, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ColumnModel } from '../../core/models/column.model';
import { MessageService } from 'primeng/api';
import { ConfirmDeleteDialogComponent } from '../../shared/components/confirm-delete-dialog/confirm-delete-dialog.component';
import { Router } from '@angular/router';
import { BaseService } from '../../core/services/base.service';

@Component({
  selector: 'app-list-page-layout',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    ToastModule,
    ConfirmDeleteDialogComponent
  ],
  templateUrl: './list-page-layout.component.html',
  styleUrl: './list-page-layout.component.scss'
})
export class ListPageLayoutComponent<T = unknown> {
  private readonly router = inject(Router);
  private readonly messageService = inject(MessageService);

  title = input.required<string>();
  entity = input.required<string>();
  items = input<T[]>([]);
  loading = input(false);
  totalRecords = input(0);
  pageSize = input(10);
  emptyMessage = input('No records found.');
  columns = input<ColumnModel[]>([]);

  lazyLoad = output<TableLazyLoadEvent>();
  search = output<void>();
  clearFilters = output<void>();

  baseRoute = input<string>();
  canCreate = input(false);

  idField = input.required<string>();
  service = input<BaseService<T>>();

  selectedId = signal('');
  isDeleteDialogVisible = signal(false);
  deleteLoading = signal(false);

  onCreate() {
    this.router.navigate([this.baseRoute(), 'create']);
  }

  onView(item: T) {
    this.router.navigate([this.baseRoute(), (item as any)[this.idField()], 'view']);

  }

  onEdit(item: T) {
    this.router.navigate([this.baseRoute(), (item as any)[this.idField()], 'edit']);
  }

  onDelete(item: T) {
    this.selectedId.set((item as any)[this.idField()] as string);
    this.isDeleteDialogVisible.set(true);
  }

  onDeleteConfirmed() {
    this.deleteLoading.set(true);
    this.service()?.delete(this.selectedId()).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: `${this.entity()} deleted.` });
        this.isDeleteDialogVisible.set(false);
        this.deleteLoading.set(false);
        this.search.emit();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: `Failed to delete ${this.entity().toLowerCase()}.` });
        this.deleteLoading.set(false);
      }
    });
  }
}
