import { apiClient } from "@/shared/api/axios-instance";
import type { User } from "@/entities/user/user.types";
import type { PagedResult } from "@/shared/api/paged-result";

export interface UserListParams {
  search?: string;
  page?: number;
  pageSize?: number;
}

export const userApi = {
  // GET /api/v1/users — dibuka untuk semua role terautentikasi (lihat
  // UsersController.List di backend), dipakai untuk dropdown Assignee.
  list: (params?: UserListParams) =>
    apiClient
      .get<PagedResult<User>>("/users", { params: { pageSize: 100, ...params } })
      .then((res) => res.data),
};
