import { Component, contentChild, Directive, input, output, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';

@Directive({
  selector: 'ng-template[listBody]'
})
export class ListBodyDirective {
  constructor(public template: TemplateRef<unknown>) {}
}

@Component({
  selector: 'app-list-page-layout',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    ToastModule
  ],
  templateUrl: './list-page-layout.component.html',
  styleUrl: './list-page-layout.component.scss'
})
export class ListPageLayoutComponent<T = unknown> {
  title = input.required<string>();
  items = input<T[]>([]);
  loading = input(false);
  totalRecords = input(0);
  pageSize = input(10);
  emptyMessage = input('No records found.');
  columnCount = input(1);

  lazyLoad = output<TableLazyLoadEvent>();
  search = output<void>();
  clearFilters = output<void>();

  bodyTemplate = contentChild(ListBodyDirective);
}