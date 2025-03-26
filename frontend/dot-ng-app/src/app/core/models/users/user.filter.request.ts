export interface UserFilterRequest {
    PageNumber?: number;
    PageSize?: number;
    SortColumn ?: string;
    SortOrder ?: boolean;
    Name?: string;
    Email?: string;
    UserName?: string;
}