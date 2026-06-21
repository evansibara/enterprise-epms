// Mencerminkan EPMS.Application.DTOs.Common.PagedResult<T> dari backend.
// Dipakai bersama oleh project-api.ts, user-api.ts, dll — ditaruh di shared
// supaya tidak ada feature yang mengimpor tipe dari feature lain secara langsung.
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
