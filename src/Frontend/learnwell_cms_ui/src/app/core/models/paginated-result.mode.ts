export interface PaginatedResult<T> {
  totalRecords: number;
  records: T[];
}