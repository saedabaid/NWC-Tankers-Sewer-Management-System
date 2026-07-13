import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';

import { Customer } from '../Models/customer.model';
import { SearchResult } from '../Models/common/search-result';
import { CustomerSC } from '../Models/search-criteria/customer-sc.model';
import { CustomerAccount } from '../Models/customer-account.model';
import { CustomerAccountSC } from '../Models/search-criteria/customer-account-SC.model';



@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private http: HttpClient) { }

  SearchCustomerList(searchCriteria: CustomerSC): Observable<DescriptiveResponse<SearchResult<Customer>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.customer.SearchCustomerList}`;
    return this.http.post<DescriptiveResponse<SearchResult<Customer>>>(url, searchCriteria);
  }

  SearchCustomerAccountList(searchCriteria: CustomerAccountSC): Observable<DescriptiveResponse<SearchResult<CustomerAccount>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.customer.SearchCustomerAccountList}`;
    return this.http.post<DescriptiveResponse<SearchResult<CustomerAccount>>>(url, searchCriteria);
  }



  CreateCustomerAndLocations(contractor: Customer): Observable<DescriptiveResponse<Customer>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.customer.CreateCustomerAndLocations}`;
    return this.http.post<DescriptiveResponse<Customer>>(url, contractor);
  }

  EditCustomerAndLocations(contractor: Customer): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.customer.EditCustomerAndLocations}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractor);
  }

  DeleteCustomer(customerId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.customer.DeleteCustomer}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, customerId);
  }




 
}
