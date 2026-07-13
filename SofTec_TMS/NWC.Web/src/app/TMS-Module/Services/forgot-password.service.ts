import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { passwordDTO } from '../Models/passwordDTO';
import { Observable } from 'rxjs';
import { SearchResult } from '../Models/common/search-result';

@Injectable({ providedIn: 'root' })
export class forgotPasswordService implements OnInit {
  ngOnInit(): void {

  }

  constructor(private httpClient: HttpClient) {

  }
  forgotPassword(email: string) {
    return this.httpClient.post<DescriptiveResponse<SearchResult<passwordDTO>>>
      (Configuration.urls.commandEndpoint + Configuration.urls.account.ForgotPassword, email);
  }

}
