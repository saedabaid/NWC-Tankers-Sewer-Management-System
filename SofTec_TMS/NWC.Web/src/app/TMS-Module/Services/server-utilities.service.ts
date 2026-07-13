import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { Configuration } from './../../shared/configurations/shared.config';


@Injectable({
    providedIn: 'root'
})
export class ServerUtilitiesService {

    constructor(private http: HttpClient) { }

    GetDateTimeNow(): Observable<DescriptiveResponse<Date>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ServerUtilities.GetDateTimeNow}`;
        return this.http.get<DescriptiveResponse<Date>>(url)
    }


}
