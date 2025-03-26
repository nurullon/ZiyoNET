export interface ListResponse<T> {
    data: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    isFirst: boolean;
    isLast: boolean;
}