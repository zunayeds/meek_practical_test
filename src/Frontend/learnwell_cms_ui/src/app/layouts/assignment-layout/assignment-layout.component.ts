import { Component, inject, input, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { PickListComponent } from '../../shared/components/pick-list/pick-list.component';
import { PickListItem } from '../../core/models/pick-list.mode';
import { Observable } from 'rxjs';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-assignment-layout',
  standalone: true,
  imports: [
    PickListComponent,
    CardModule,
    ButtonModule,
  ],
  templateUrl: './assignment-layout.component.html',
  styleUrl: './assignment-layout.component.scss'
})
export class AssignmentLayoutComponent {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly messageService = inject(MessageService);

  title = input.required<string>();
  itemName = input.required<string>();
  loadAssigned = input.required<(itemId: string) => Observable<PickListItem[]>>();
  loadSourcePage = input.required<(page: number) => Observable<PickListItem[]>>();
  saveAssignments = input.required<(itemId: string, addIds: string[], removeIds: string[]) => Observable<unknown>>();
  listRoute = input.required<string>();

  itemId = signal('');
  saving = signal(false);
  loadingSource = signal(false);
  sourceItems = signal<PickListItem[]>([]);
  targetItems = signal<PickListItem[]>([]);
  sourcePage = signal(1);
  sourceHasMore = signal(false);
  originalTargetIds = signal<Set<string>>(new Set());

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.itemId.set(id);
    if (!id) return;

    this.loadingSource.set(true);
    this.loadAssigned()(id).subscribe({
      next: target => {
        this.targetItems.set(target);
        this.originalTargetIds.set(new Set(target.map(t => t.id)));
        this.loadingSource.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load assigned items.' });
        this.loadingSource.set(false);
      }
    });

    this.fetchSourcePage(1);
  }

  private fetchSourcePage(page: number): void {
    this.loadingSource.set(true);
    this.loadSourcePage()(page).subscribe({
      next: items => {
        this.sourceItems.set(items);
        this.sourcePage.set(page);
        this.sourceHasMore.set(items.length > 0);
        this.loadingSource.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load available items.' });
        this.loadingSource.set(false);
      }
    });
  }

  onTargetChange(items: PickListItem[]) {
    this.targetItems.set(items);
  }

  onPageChange(page: number) {
    this.fetchSourcePage(page);
  }

  onSave() {
    this.saving.set(true);
    const currentIds = new Set(this.targetItems().map(i => i.id));
    const originalIds = this.originalTargetIds();
    const addIds = [...currentIds].filter(id => !originalIds.has(id));
    const removeIds = [...originalIds].filter(id => !currentIds.has(id));

    this.saveAssignments()(this.itemId(), addIds, removeIds).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Saved', detail: 'Assignments updated.' });
        setTimeout(() => this.goToList(), 800);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to save.' });
        this.saving.set(false);
      }
    });
  }

  onCancel() {
    this.goToList();
  }

  goToList() {
    this.router.navigate([this.listRoute()]);
  }
}
