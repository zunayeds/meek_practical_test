export interface ColumnModel {
  field: string;
  header: string;
  type: 'string' | 'number' | 'date' | 'dateTime' | 'action';
  actions?: RowActionsType[];
}

export type RowActionsType = 'view' | 'edit' | 'delete';