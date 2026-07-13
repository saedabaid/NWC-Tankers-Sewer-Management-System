import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
// import { Observable } from 'openlayers';
import { Observable, BehaviorSubject, Subject } from 'rxjs';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';
import { ContractSearchCriteria } from '../Models/search-criteria/contract-search-criteria';
import { Contract } from '../Models/contract';
import { ContractTariff } from '../Models/contract-tariff';
import { FilterModel } from '../Models/common/filter-model';
import { searchCriteriaContractStationDTO } from '../Models/search-criteria/search-Criteria-Contract-StationDTO';
import { ContractStationListDTO } from '../Models/Contract-Station-ListDTO';
import { ContractStationDTO } from '../Models/contract-stationDTO';
import { AddItemsResponse } from '../Models/common/AddItemsResponse';
import { contractAccessorySC } from '../Models/search-criteria/contractAccessory-SC';
import { contractAccessory } from '../Models/contract-Accessory';
import { ContractPriceDTO } from '../Models/contract-priceDTO';
import { vw_NWC_ContractTermsDTO } from '../Models/vw_NWC_Contract-TermsDTO';
import { ContractTermDTO } from '../Models/contract-termsDTO';
import { searchCriteriaContractDTO } from '../Models/search-criteria/search-Criteria-Contract-DTO';
import { AttachmentDTO } from 'src/app/shared/datamodels/attachment-dto';
import { ContractApprovalViolation, ContractViolationSC } from '../Models/search-criteria/contract-violation-SC.model';
import { ContractTermsViolationsDTo } from '../Models/contract-terms-violations.model';
import { ContractTermsApprovalViolationsLogsDTO, ContractTermsViolationsLogsDTO } from '../Models/contract-terms-violations-logs.model';
import { ContractTermsViolationsInVoiceDTO } from '../Models/contract-terms-violations-invoice.model';
import { violationapprovalsDto } from '../Models/violation-approvals';



@Injectable({
  providedIn: 'root'
})
export class ContractService {

  updateContractId: number;

  updateTariffModel$ = new Subject<ContractTariff>();
  refreshTariffGV$ = new Subject<boolean>();
  isTariffEditMode = false;

  changeTab$ = new Subject<string>();

  constructor(private http: HttpClient) { }

  searchContractList(filter: ContractSearchCriteria): Observable<DescriptiveResponse<SearchResult<Contract>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.SearchContractList}`;
    return this.http.post<DescriptiveResponse<SearchResult<Contract>>>(url, filter);
  }

  GetContractAttachments(contractId: number): Observable<DescriptiveResponse<AttachmentDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractAttachments}?contractId=${contractId}`;
    return this.http.get<DescriptiveResponse<AttachmentDTO[]>>(url);
  }

  addContract(contract: Contract): Observable<DescriptiveResponse<number>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddContract}`;
    return this.http.post<DescriptiveResponse<number>>(url, contract);
  }

  EditContract(contract: Contract): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.EditContract}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contract);
  }

  DeleteContract(contractId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.DeleteContract}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractId);
  }

  TerminateContract(contractId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.TerminateContract}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractId);
  }

  SearchTariffList(filter: FilterModel<number>): Observable<DescriptiveResponse<SearchResult<ContractTariff>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.SearchTariffList}`;
    return this.http.post<DescriptiveResponse<SearchResult<ContractTariff>>>(url, filter);
  }

  AddTariff(tariff: ContractTariff): Observable<DescriptiveResponse<AddItemsResponse>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddTariff}`;
    return this.http.post<DescriptiveResponse<AddItemsResponse>>(url, tariff);
  }

  EditTariff(tariff: ContractTariff): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.EditTariff}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, tariff);
  }

  DeleteTariff(tariffId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.DeleteTariff}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, tariffId);
  }

  AddTariffRange(tariff: ContractTariff[]): Observable<DescriptiveResponse<ContractTariff[]>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddTariffRange}`;
    return this.http.post<DescriptiveResponse<ContractTariff[]>>(url, tariff);
  }

  SearchStattionList(searchCriteria: searchCriteriaContractStationDTO): Observable<DescriptiveResponse<SearchResult<ContractStationListDTO>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.SearchStattionList}`;
    return this.http.post<DescriptiveResponse<SearchResult<ContractStationListDTO>>>(url, searchCriteria);
  }

  AddStation(dto: ContractStationDTO): Observable<DescriptiveResponse<AddItemsResponse>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddStation}`;
    return this.http.post<DescriptiveResponse<AddItemsResponse>>(url, dto);
  }
  UpdateStation(dto: ContractStationDTO): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.UpdateStation}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, dto);
  }

  DeleteStation(dto: ContractStationListDTO): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.DeleteStation}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, dto);
  }

  SearchContractAccessories(searchCriteria: contractAccessorySC): Observable<DescriptiveResponse<SearchResult<contractAccessory>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.SearchContractAccessories}`;
    return this.http.post<DescriptiveResponse<SearchResult<contractAccessory>>>(url, searchCriteria);
  }

  DeleteContractAccessory(contractAccID: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.DeleteContractAccessory}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, contractAccID);
  }

  AddContractAccessories(dto: contractAccessory): Observable<DescriptiveResponse<AddItemsResponse>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddContractAccessories}`;
    return this.http.post<DescriptiveResponse<AddItemsResponse>>(url, dto);
  }

  UpdateContractAccessory(dto: contractAccessory): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.UpdateContractAccessory}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, dto);
  }

  GetContractPriceList(searchCriteria: searchCriteriaContractDTO): Observable<DescriptiveResponse<SearchResult<ContractPriceDTO>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractPriceList}`;
    return this.http.post<DescriptiveResponse<SearchResult<ContractPriceDTO>>>(url, searchCriteria);
  }

  UpdatePriceList( ContractPriceList: ContractPriceDTO[]): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.UpdatePriceList}`;
    return this.http.put<DescriptiveResponse<boolean>>(url, ContractPriceList);
  }
  GetContractTermsList( searchCriteriaContractTermDTO: searchCriteriaContractDTO): Observable<DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractTermsList}`;
    return this.http.post<DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>>>(url, searchCriteriaContractTermDTO);
  }

  GetTermValueUnit(termId: number) {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetTermValueUnit}?termId=${termId}`;
    return this.http.get<DescriptiveResponse<vw_NWC_ContractTermsDTO>>(url);
  }

  AddTerm(ContractTermDTO: ContractTermDTO): Observable<DescriptiveResponse<AddItemsResponse>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddTerm}`;
    return this.http.post<DescriptiveResponse<AddItemsResponse>>(url, ContractTermDTO);
  }

  UpdateTerm(ContractTermDTO: ContractTermDTO): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.UpdateTerm}`;
    return this.http.put<DescriptiveResponse<boolean>>(url, ContractTermDTO);
  }

  DeleteTerm(TermID: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.DeleteTerm + '?TermID=' + TermID}`;
    return this.http.delete<DescriptiveResponse<boolean>>(url);
  }


  GetContractViolations(filter: ContractViolationSC): Observable<DescriptiveResponse<SearchResult<ContractTermsViolationsDTo>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractViolations}`;
    return this.http.post<DescriptiveResponse<SearchResult<ContractTermsViolationsDTo>>>(url, filter);
  }
  GetContractViolationsAttachments(violationId: number): Observable<DescriptiveResponse<AttachmentDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractViolationsAttachments}?violationId=${violationId}`;
    return this.http.get<DescriptiveResponse<AttachmentDTO[]>>(url);
  }

  GetContractViolationLogs(violationId: number): Observable<DescriptiveResponse<ContractTermsViolationsLogsDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractViolationLogs}?violationId=${violationId}`;
    return this.http.get<DescriptiveResponse<ContractTermsViolationsLogsDTO[]>>(url);
  }
  GetContractApprovalViolationLogs(violationId: number): Observable<DescriptiveResponse<ContractTermsApprovalViolationsLogsDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractApprovalViolationLogs}?violationId=${violationId}`;
    return this.http.get<DescriptiveResponse<ContractTermsApprovalViolationsLogsDTO[]>>(url);
  }
  GetTermViolationInvoice(violationId: number): Observable<DescriptiveResponse<ContractTermsViolationsInVoiceDTO>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetTermViolationInvoice}?violationId=${violationId}`;
    return this.http.get<DescriptiveResponse<ContractTermsViolationsInVoiceDTO>>(url);
  }

  AddContractViolation(violation: ContractTermsViolationsDTo): Observable<DescriptiveResponse<number>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddContractViolation}`;
    return this.http.post<DescriptiveResponse<number>>(url, violation);
  }

  AddViolationApproval(violation: violationapprovalsDto): Observable<DescriptiveResponse<number>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddViolationApproval}`;
    return this.http.post<DescriptiveResponse<number>>(url, violation);
  }

  //GetContractApprovalViolation(ContractApprovalViolationsc: ContractApprovalViolation): Observable<DescriptiveResponse<number>> {
  //  const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractApprovalViolation}`;
  //  return this.http.get<DescriptiveResponse<violationapprovalsDto[]>>(url);
  //}

  GetContractApprovalViolation(filter: ContractApprovalViolation): Observable<DescriptiveResponse<SearchResult<violationapprovalsDto>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.contracts.GetContractApprovalViolation}`;
    return this.http.post<DescriptiveResponse<SearchResult<violationapprovalsDto>>>(url, filter);
  }

  EditContractViolation(violation: ContractTermsViolationsDTo): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.EditContractViolation}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, violation);
  }
  AddViolationDecision(violationId: number, Approval: Boolean ): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.AddViolationDecision}`;
    const ViolationApproveReject = { violationId: violationId, Approval: Approval }

    return this.http.post<DescriptiveResponse<number>>(url, ViolationApproveReject);
  }
  
  DeleteContractViolation(violationId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.DeleteContractViolation}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, violationId);
  }
  DeleteViolationApproval(ViolationApprovalId: number): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.contracts.DeleteViolationApproval}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, ViolationApprovalId);
  }


}
