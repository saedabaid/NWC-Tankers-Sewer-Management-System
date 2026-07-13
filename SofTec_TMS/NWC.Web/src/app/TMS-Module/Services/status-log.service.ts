import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})

export class orderStatusLog {
    constructor(private http: HttpClient) { }

    getLogStatus(workorderId) { 
        return this.http.get(Configuration.urls.queryEndpoint + Configuration.urls.orders.GetWorkOrderStatusLogs +"?workOrderId=" + workorderId);
    }

}