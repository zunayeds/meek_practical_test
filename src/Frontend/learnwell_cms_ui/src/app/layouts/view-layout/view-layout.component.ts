import { Component, OnInit, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { BaseService } from '../../core/services/base.service';

@Component({
  selector: 'app-view-layout',
  imports: [ReactiveFormsModule, CardModule, InputTextModule, TextareaModule, ButtonModule, ToastModule],
  templateUrl: './view-layout.component.html'
})
export class ViewLayoutComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly messageService = inject(MessageService);

  entity = input.required<string>();
  service = input.required<BaseService<any>>();
  listRoute = input.required<string>();
  identifier = input.required<string>();
  dataLoaded = output<any>();

  id = signal('');
  item = signal<any>(null);
  loading = signal(false);

  ngOnInit(): void {
    this.id.set(this.route.snapshot.paramMap.get('id')!);
    this.service().getById(this.id()).subscribe({
      next: result => {
        this.dataLoaded.emit(result);
        this.item.set(result);
        this.loading.set(false);
      },
      error: () => { this.messageService.add({ severity: 'error', summary: 'Error', detail: `Failed to load ${this.entity()}.` }); this.loading.set(false); }
    });
  }

  onBack() {
    this.router.navigate([this.listRoute()]);
  }

  onEdit() {
    this.router.navigate([`${this.listRoute()}/${this.id()}`, 'edit']);
  }
}
