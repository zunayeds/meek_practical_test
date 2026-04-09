import { Directive, TemplateRef, inject } from '@angular/core';

@Directive({
  selector: 'ng-template[listExtraActions]',
  standalone: true
})
export class ListExtraActionsDirective<T = unknown> {
  readonly template = inject<TemplateRef<{ $implicit: T }>>(TemplateRef);
}
