import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DescriptiveResponse } from 'src/app/TMS-Module/Models/common/descriptive-response';
import { AttachmentDTO } from '../datamodels/attachment-dto';


@Injectable({
    providedIn: 'root'
})
export class FileUploadService {
    constructor(private http: HttpClient) { }


    uploadFiles(files: File[]): Observable<DescriptiveResponse<AttachmentDTO[]>> {
        const url = `${Configuration.urls.commandEndpoint + Configuration.urls.File.UploadFiles}`;

        let formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append("uploadFile" + i, files[i], files[i]['name']);
        }

        return this.http.post<DescriptiveResponse<AttachmentDTO[]>>(url, formData);
    }

    deleteFiles(fileName: string): Observable<DescriptiveResponse<boolean>> {
        const url = `${Configuration.urls.commandEndpoint + Configuration.urls.File.DeleteFile}`;

        let Params = new HttpParams();
        Params = Params.append('fileName', fileName);
        return this.http.delete<DescriptiveResponse<boolean>>(url, { params: Params });
    }


    download(type: number, relativePath: string, id: number) {
        const url = `${Configuration.urls.commandEndpoint + Configuration.urls.File.DownloadFile}`;

        let Params = new HttpParams();
        // Begin assigning parameters
        Params = Params.append('type', `${type}`);
        Params = Params.append('relativePath', relativePath);
        Params = Params.append('id', `${id}`);

        return this.http.get(url, { params: Params, responseType: "blob" });
    }


}
