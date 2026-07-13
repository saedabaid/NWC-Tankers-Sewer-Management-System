import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';
import { Observable } from 'rxjs';
import { ReportSC } from '../Models/search-criteria/report-SC.model';
import { ContractTariff } from '../Models/contract-tariff';
import { ReportTankersPermissionsStatusSC } from '../Models/search-criteria/report-tankers-P-S-SC.model';
import { Report_TankersPermissionsStatus } from '../Models/report-tanker-P-S.model';
import { Report_SoqyaScheduledPerDay } from '../Models/report-soqya-scheduled-per-day.model';
import { ReportScheduledPerDaySC } from '../Models/search-criteria/report-scheduled-per-day-SC.model';
import { Report_OrderPerZone } from '../Models/report-order-per-zone.model';
import { contractTariffReport } from '../Models/search-criteria/contract-tariff-report';
@Injectable({
    providedIn: 'root'
})

export class ReportService {
    constructor(private http: HttpClient) { }

    GetOrdersPerZone(searchCriteria: ReportSC): Observable<DescriptiveResponse<SearchResult<Report_OrderPerZone>>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Report.GetOrdersPerZone}`;
        return this.http.post<DescriptiveResponse<SearchResult<Report_OrderPerZone>>>(url, searchCriteria)
    }

    GetStationOrderCapacity(searchCriteria: ReportSC): Observable<DescriptiveResponse<SearchResult<Report_OrderPerZone>>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Report.GetStationOrderCapacity}`;
        return this.http.post<DescriptiveResponse<SearchResult<Report_OrderPerZone>>>(url, searchCriteria)
    }

    GetStationServiceTime(searchCriteria: ReportSC): Observable<DescriptiveResponse<SearchResult<Report_OrderPerZone>>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Report.GetStationServiceTime}`;
        return this.http.post<DescriptiveResponse<SearchResult<Report_OrderPerZone>>>(url, searchCriteria)
    }

    GetTankerPermissionStatus(searchCriteria: ReportTankersPermissionsStatusSC): Observable<DescriptiveResponse<SearchResult<Report_TankersPermissionsStatus>>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Report.GetTankerPermissionStatus}`;
        return this.http.post<DescriptiveResponse<SearchResult<Report_TankersPermissionsStatus>>>(url, searchCriteria)
    }

    GetSoqyaSchedulePerDay(searchCriteria: ReportScheduledPerDaySC): Observable<DescriptiveResponse<Report_SoqyaScheduledPerDay[]>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Report.GetSoqyaSchedulePerDay}`;
        return this.http.post<DescriptiveResponse<Report_SoqyaScheduledPerDay[]>>(url, searchCriteria)
  }

  ContractTariffReport(searchCriteria: contractTariffReport): Observable<DescriptiveResponse<ContractTariff[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Report.ContractTariffReport}`;
    return this.http.post<DescriptiveResponse<ContractTariff[]>>(url, searchCriteria)
  }


}
