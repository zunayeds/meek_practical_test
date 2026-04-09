import { Component, input, OnChanges, output } from "@angular/core";
import { PickListItem } from "../../../core/models/pick-list.mode";
import { ButtonModule } from "primeng/button";
import { ProgressSpinnerModule } from "primeng/progressspinner";

@Component({
  selector: 'app-pick-list',
  imports: [ButtonModule, ProgressSpinnerModule],
  templateUrl: './pick-list.component.html',
  styleUrl: './pick-list.component.scss'
})
export class PickListComponent implements OnChanges {
  sourceItems = input<PickListItem[]>([]);
  targetItems = input<PickListItem[]>([]);
  sourcePage = input(1);
  sourceHasMore = input(false);
  loading = input(false);

  targetChange = output<PickListItem[]>();
  pageChange = output<number>();

  selectedSource: PickListItem[] = [];
  selectedTarget: PickListItem[] = [];
  filteredSource: PickListItem[] = [];

  ngOnChanges() {
    this.refreshFilteredSource();
  }

  private refreshFilteredSource() {
    const targetIds = new Set(this.targetItems().map(i => i.id));
    this.filteredSource = this.sourceItems().filter(i => !targetIds.has(i.id));
    this.selectedSource = this.selectedSource.filter(s => this.filteredSource.some(f => f.id === s.id));
  }

  toggleSourceSelection(item: PickListItem) {
    const idx = this.selectedSource.findIndex(s => s.id === item.id);
    if (idx >= 0) {
      this.selectedSource.splice(idx, 1);
    } else {
      this.selectedSource.push(item);
    }
  }

  toggleTargetSelection(item: PickListItem) {
    const idx = this.selectedTarget.findIndex(s => s.id === item.id);
    if (idx >= 0) {
      this.selectedTarget.splice(idx, 1);
    } else {
      this.selectedTarget.push(item);
    }
  }

  isSelectedSource(item: PickListItem) {
    return this.selectedSource.some(s => s.id === item.id);
  }

  isSelectedTarget(item: PickListItem) {
    return this.selectedTarget.some(s => s.id === item.id);
  }

  addSelected() {
    const updated = [...this.targetItems(), ...this.selectedSource];
    this.selectedSource = [];
    this.targetChange.emit(updated);
  }

  addAll() {
    const updated = [...this.targetItems(), ...this.filteredSource];
    this.selectedSource = [];
    this.targetChange.emit(updated);
  }

  removeSelected() {
    const removeIds = new Set(this.selectedTarget.map(i => i.id));
    const updated = this.targetItems().filter(i => !removeIds.has(i.id));
    this.selectedTarget = [];
    this.targetChange.emit(updated);
  }

  removeAll() {
    this.selectedTarget = [];
    this.targetChange.emit([]);
  }

  prevPage() {
    if (this.sourcePage() > 1) {
      this.selectedSource = [];
      this.pageChange.emit(this.sourcePage() - 1);
    }
  }

  nextPage() {
    if (this.sourceHasMore()) {
      this.selectedSource = [];
      this.pageChange.emit(this.sourcePage() + 1);
    }
  }
}
