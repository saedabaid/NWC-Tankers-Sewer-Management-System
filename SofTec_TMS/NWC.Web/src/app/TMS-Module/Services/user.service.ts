import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { Observable } from 'rxjs';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { PermissionDataModel } from 'src/app/shared/datamodels/permissions.data.model';
import { ProfileDTO, UserProfile } from '../Models/profile.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }  

  GetUserPermissions(): Observable<DescriptiveResponse<PermissionDataModel[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.User.GetUserPermissions}`;
    return this.http.get<DescriptiveResponse<PermissionDataModel[]>>(url);
  }

  GetUserProfile(): Observable<DescriptiveResponse<ProfileDTO>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.User.GetUserProfile}`;
    return this.http.get<DescriptiveResponse<ProfileDTO>>(url);
  }
  GetAccountProfile(): Observable<DescriptiveResponse<UserProfile>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.User.GetUserProfile}`;
    return this.http.get<DescriptiveResponse<UserProfile>>(url);
  }
}
