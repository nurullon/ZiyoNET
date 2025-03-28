import { Injectable } from "@angular/core";
import { ApiService } from "./api.service";
import { API_ROUTES } from "../constants/api-routes";
import { ApiResponse } from "../models/api-response";
import { environment } from "../../../environments/environment";

@Injectable({
    providedIn: 'root',
})
export class ExcelService {
    http: any;
    constructor(private api: ApiService) { }

    async uploadExcel(file: File): Promise<boolean> {
        const formData = new FormData();
        formData.append('file', file);

        const result = await this.api.post<boolean>(API_ROUTES.EXCEL.UPLOAD, formData);
        if (result.success)
            return true;

        return false;
    }

    async downloadAndOpenExcel(): Promise<void> {

        const result = await this.api.get<ApiResponse<string>>(API_ROUTES.EXCEL.DOWNLOAD);
        if (!result.success || !result.data) return;

        const fileName = result.data as unknown as string;
        console.log(fileName);
        const fileUrl = `${environment.baseUrl}${fileName}`;
        console.log(fileUrl);

        const link = document.createElement("a");
        link.href = fileUrl;
        link.download = fileName ?? "";
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}