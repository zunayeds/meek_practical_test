import { Component, input, output } from "@angular/core";
import { ButtonModule } from "primeng/button";
import { DialogModule } from "primeng/dialog";
import { ColumnModel } from "../../../core/models/column.model";
import { TableLazyLoadEvent, TableModule } from "primeng/table";
import { CommonModule } from "@angular/common";

@Component({
  selector: 'app-list-table',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, DialogModule],
  templateUrl: './list-table.component.html'
})
export class ListTableComponent<T> {
  items = input<T[]>([]);
  loading = input(false);
  totalRecords = input(0);
  pageSize = input(10);
  emptyMessage = input('No records found');
  columns = input<ColumnModel[]>([]);
  paginationEnabled = input(true);
  isLazyLoad = input(false);

  lazyLoad = output<TableLazyLoadEvent>();
  onView = output<T>();
  onEdit = output<T>();
  onDelete = output<T>();
 }
