import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Configuration } from '../../configurations/shared.config';
import { PermissionDataModel } from '../../datamodels/permissions.data.model';

@Injectable()
export class AuthorizationService {

    constructor(private http :HttpClient){
    }
    public getPermissions(UserId:any ,SubId:any)
    {
        
        const options = UserId ? { params: new HttpParams().set('UserId', UserId).set('SubId',SubId) } : {};
        return this.http.get<PermissionDataModel[]>(Configuration.urls.authenticationEndpoint + Configuration.urls.apiAccountUrl.apiGetPermissionsURL, options);
    }
}