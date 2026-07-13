import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { UserSearchCriteriaDTO } from '../Models/search-criteria/user-search-criteria';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { UserListDTO } from '../Models/user-listDTO';
import { UserInfo } from '../Models/user-listDTO';
import { Observable } from 'rxjs';
import { SearchResult } from '../Models/common/search-result';

@Injectable({ providedIn: 'root' })
export class userListService implements OnInit {
  ngOnInit(): void {

  }

  constructor(private httpClient: HttpClient) {

  }

  public Search(UserSearchCriteriaDTO: UserSearchCriteriaDTO): Observable<DescriptiveResponse<SearchResult<UserListDTO>>> {
   
    return this.httpClient.post<DescriptiveResponse<SearchResult<UserListDTO>>>
      (Configuration.urls.queryEndpoint + Configuration.urls.userlist.userSearch, UserSearchCriteriaDTO);
  }
  deleteUser(Name: string): Observable<DescriptiveResponse<Boolean>> {
    const headers = { 'content-type': 'application/json' }
    const body = JSON.stringify(Name);
    let url = Configuration.urls.commandEndpoint + Configuration.urls.userlist.Delete;
    return this.httpClient.post<DescriptiveResponse<Boolean>>(url, body, { 'headers': headers })
  }
 unlockUser(Name: string): Observable<DescriptiveResponse<Boolean>> {
   const headers = { 'content-type': 'application/json' }
   const body = JSON.stringify(Name);
   let url = Configuration.urls.commandEndpoint + Configuration.urls.userlist.unlock;
   return this.httpClient.post<DescriptiveResponse<Boolean>>(url, body, { 'headers': headers })
  }
  lockUser(Name: string): Observable<DescriptiveResponse<Boolean>> {
    const headers = { 'content-type': 'application/json' }
    const body = JSON.stringify(Name);
    let url = Configuration.urls.commandEndpoint + Configuration.urls.userlist.lock;
    return this.httpClient.post<DescriptiveResponse<Boolean>>(url, body, { 'headers': headers })
  }

}
