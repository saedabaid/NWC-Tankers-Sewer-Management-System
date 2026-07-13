
import { Configuration } from './../../shared/configurations/shared.config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SoqyaScheduleSC } from '../Models/search-criteria/Soqya-search-criteria';
import { SoqyaScheduleDTO, SoqyaBalanceDTO } from '../Models/SoqyaScheduleDTO';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { SearchResult } from '../Models/common/search-result';
import { Observable } from 'rxjs';
import { SoqyaScheduleReportSC } from '../Models/search-criteria/soqya-schecule-report-SC.model';
import { SoqyaScheduleReportDTO } from '../Models/soqya-schedule-report.model';

@Injectable({
    providedIn: 'root'
})

export class SoqyaService {
    constructor(private http: HttpClient) { }

    SearchSoqyaSchedules(searchCriteria: SoqyaScheduleSC): Observable<DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Soqya.SearchSoqyaSchedules}`;
        return this.http.post<DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>>(url, searchCriteria)
    }

    GetBalanceAndUsed(customerAccountId: number, monthYear: number): Observable<DescriptiveResponse<SoqyaBalanceDTO>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Soqya.GetBalanceAndUsed}?customerAccountId=${customerAccountId}&monthYear=${monthYear}`;
        return this.http.get<DescriptiveResponse<SoqyaBalanceDTO>>(url)
    }

    AddSoqyaSchedule(dto: SoqyaScheduleDTO): Observable<DescriptiveResponse<boolean>> {
        const url = `${Configuration.urls.commandEndpoint + Configuration.urls.Soqya.AddSoqyeScheduleRecord}`;
        return this.http.post<DescriptiveResponse<boolean>>(url, dto);
    }

    DeleteSoqyeScheduleRecord(scheduleId: number): Observable<DescriptiveResponse<boolean>> {
        const url = `${Configuration.urls.commandEndpoint + Configuration.urls.Soqya.DeleteSoqyeScheduleRecord}`;
        return this.http.post<DescriptiveResponse<boolean>>(url, scheduleId)
    }

    EditSoqyeScheduleRecord(dto: SoqyaScheduleDTO): Observable<DescriptiveResponse<boolean>> {
        const url = `${Configuration.urls.commandEndpoint + Configuration.urls.Soqya.EditSoqyeScheduleRecord}`;
        return this.http.post<DescriptiveResponse<boolean>>(url, dto)
    }

    AddRange(SoqyaScheduleDTO: SoqyaScheduleDTO[]): Observable<DescriptiveResponse<SoqyaScheduleDTO[]>> {
        let url = Configuration.urls.commandEndpoint + Configuration.urls.Soqya.AddRange;
        return this.http.post<DescriptiveResponse<SoqyaScheduleDTO[]>>(url, SoqyaScheduleDTO);
    }

    GetSoqyaSchedulesReport(searchCriteria: SoqyaScheduleReportSC): Observable<DescriptiveResponse<SearchResult<SoqyaScheduleReportDTO>>> {
        const url = `${Configuration.urls.queryEndpoint + Configuration.urls.Soqya.GetSoqyaSchedulesReport}`;
        return this.http.post<DescriptiveResponse<SearchResult<SoqyaScheduleReportDTO>>>(url, searchCriteria)
    }

}

