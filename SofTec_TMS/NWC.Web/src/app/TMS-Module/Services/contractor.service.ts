import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';

import { AttachmentDTO } from 'src/app/shared/datamodels/attachment-dto';
import { Contractor } from '../Models/contractor';
import { ContractorSearchCriteria } from '../Models/search-criteria/contractor-search-criteria';



@Injectable({
  providedIn: 'root'
})
export class ContractorService {

  constructor(private http: HttpClient) { }

  searchContractorList(filter: ContractorSearchCriteria): Observable<DescriptiveResponse<SearchResult<Contractor>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contractor.SearchContractorList}`;
    return this.http.post<DescriptiveResponse<SearchResult<Contractor>>>(url, filter);
  }

  GetContractorAttachments(contractorId: number): Observable<DescriptiveResponse<AttachmentDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contractor.GetContractorAttachments}?contractorId=${contractorId}`;
    return this.http.get<DescriptiveResponse<AttachmentDTO[]>>(url);
  }

  addContractor(contractor: Contractor): Observable<DescriptiveResponse<number>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contractor.AddContractor}`;
    return this.http.post<DescriptiveResponse<number>>(url, contractor);
  }

  EditContractor(contractor: Contractor): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contractor.EditContractor}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractor);
  }

  ActivateContractor(contractId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contractor.ActivateContractor}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractId);
  }
  
  DeActivateContractor(contractId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contractor.DeActivateContractor}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractId);
  }
  
  AddContractorToBlackListed(contractId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contractor.AddContractorToBlackListed}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractId);
  }

  RemoveContractorFromBlackListed(contractId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contractor.RemoveContractorFromBlackListed}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractId);
  }
 
}
