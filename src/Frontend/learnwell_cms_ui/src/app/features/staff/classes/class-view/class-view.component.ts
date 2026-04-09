import { Component, inject, signal } from '@angular/core';

import { ClassService } from '../../../../core/services/class.service'; 
import { ViewLayoutComponent } from '../../../../layouts/view-layout/view-layout.component';
import { Class } from '../../../../core/models/class.model';
import { TabsModule } from 'primeng/tabs';
import { DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';


@Component({
  selector: 'app-class-view',
  imports: [ViewLayoutComponent, TabsModule, DatePipe, CardModule],
  templateUrl: './class-view.component.html'
})
export class ClassViewComponent {
  readonly classService = inject(ClassService);

  class = signal<Class | undefined>(undefined);

  onDataLoaded(result: any) {
    this.class.set(result);
  }
}
