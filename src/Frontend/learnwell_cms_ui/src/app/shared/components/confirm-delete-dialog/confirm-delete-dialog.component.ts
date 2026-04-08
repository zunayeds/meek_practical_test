import { Component, input, output } from "@angular/core";
import { ButtonModule } from "primeng/button";
import { DialogModule } from "primeng/dialog";

@Component({
  selector: 'app-confirm-delete-dialog',
  imports: [DialogModule, ButtonModule],
  templateUrl: './confirm-delete-dialog.component.html'
})
export class ConfirmDeleteDialogComponent {
  visible = input(false);
  message = input('Are you sure you want to delete this item? This action cannot be undone.');
  loading = input(false);
  visibleChange = output<boolean>();
  confirmed = output<void>();
  cancelled = output<void>();

  onConfirm(): void {
    this.confirmed.emit();
  }

  onCancel(): void {
    this.visibleChange.emit(false);
    this.cancelled.emit();
  }
}
