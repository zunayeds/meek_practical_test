import { Component, OnInit, inject, input, signal } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { BaseService } from '../../core/services/base.service';

@Component({
  selector: 'app-form-layout',
  imports: [ReactiveFormsModule, CardModule, InputTextModule, TextareaModule, ButtonModule, ToastModule],
  templateUrl: './form-layout.component.html',
  styleUrl: './form-layout.component.scss',
  providers: [MessageService]
})
export class FormLayoutComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly messageService = inject(MessageService);

  entity = input.required<string>();
  form = input.required<FormGroup>();
  service = input.required<BaseService<any>>();
  listRoute = input.required<string>();

  isEdit = signal(false);
  id = signal('');
  loading = signal(false);
  saving = signal(false);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true);
      this.id.set(id);
      this.loadItem(id);
    }
  }

  private loadItem(id: string): void {
    this.loading.set(true);
    this.service()?.getById(id).subscribe({
      next: item => {
        this.form().patchValue(item);
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: `Failed to load ${this.entity()}.` });
        this.loading.set(false);
      }
    });
  }

  onSave(): void {
    if (this.form().invalid) return;
    this.saving.set(true);

    const payload = this.form().value;

    const request = this.isEdit()
      ? this.service()?.update(this.id(), payload)
      : this.service()?.create(payload);

    request.subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Saved', detail: `${this.entity()} ${this.isEdit() ? 'updated' : 'created'}.` });
        setTimeout(() => this.goToList(), 800);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: `Failed to save ${this.entity()}.` });
        this.saving.set(false);
      }
    });
  }

  onCancel(): void {
    this.goToList();
  }

  goToList(): void {
    this.router.navigate([this.listRoute()]);
  }
}
