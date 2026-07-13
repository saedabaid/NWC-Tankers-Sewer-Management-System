import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserStationPermissionDTO } from '../Models/user-landmark-permissionDTO';
import { DescriptiveResponse } from '../Models/common/descriptive-response';
import { Observable } from 'rxjs';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { SearchResult } from '../Models/common/search-result';
import { StationSettingsSC } from '../Models/search-criteria/station-settings-SC.model';
import { StationSettingsNWC } from '../Models/station-settings-nwc.model';
import { VehicleSettingsSC } from '../Models/search-criteria/vehicle-settings-SC.model';
import { VehicleSettingsNWC } from '../Models/vehicle-settings-nwc.model';
import { UserStationPermissionSC } from '../Models/search-criteria/user-station-permissionSC';
import { VehicleSettingsBulkUpdate } from '../Models/vehicle-settings-bulk-update.model';
import { BranchSettingsDTO } from '../Models/branch-settings.model';
import { VehicleType } from '@tms-models/vehicle-type';
import { AreaModel } from '@tms-models/area-model';
import { BranchesSearchCriteriaDTO } from '@tms-models/search-criteria/branches-search-criteria';
import { StationDefaultTankersDTO } from '@tms-models/station-default-tankersDTO';

@Injectable({
  providedIn: 'root'
})
export class ControlPanelService {

  constructor(private http: HttpClient) { }

  getUserPermittedLandmarks(filter: UserStationPermissionSC): Observable<DescriptiveResponse<SearchResult<UserStationPermissionDTO>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.GetUserPermittedLandmarks}`;
    return this.http.post<DescriptiveResponse<SearchResult<UserStationPermissionDTO>>>(url, filter);
  }


  getAreaById(id): Observable<DescriptiveResponse<AreaModel>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.getAreaById}?id=${id}`;
    return this.http.get<DescriptiveResponse<boolean>>(url);
  }
  addArea(areaModel: AreaModel): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.addArea}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, areaModel);
  }
  editArea(areaModel: AreaModel): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.editArea}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, areaModel);
  }
  deleteArea(id: string): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.deleteArea}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, {Id: id});
  }

  getCityById(id): Observable<DescriptiveResponse<AreaModel>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.getAreaById}?id=${id}`;
    return this.http.get<DescriptiveResponse<boolean>>(url);
  }
  addCity(areaModel: AreaModel): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.addCity}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, areaModel);
  }
  editCity(areaModel: AreaModel): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.editCity}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, areaModel);
  }
  deleteCity(id: string): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.deleteCity}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, {Id: id});
  }

  getLandmarkById(id): Observable<DescriptiveResponse<AreaModel>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.getLandmarkById}?id=${id}`;
    return this.http.get<DescriptiveResponse<boolean>>(url);
  }
  addLandmark(areaModel: AreaModel): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.addLandmark}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, areaModel);
  }
  editLandmark(areaModel: AreaModel): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.editLandmark}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, areaModel);
  }
  deleteLandmark(id: string): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.deleteLandmark}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, {Id: id});
  }

  saveVehicleType(vehicleType: VehicleType): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.saveVehicleType}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, vehicleType);
  }

  saveVehicleBrand(vehicleType: VehicleType): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.saveVehicleType}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, vehicleType);
  }
  saveLandmarkType(vehicleType: VehicleType): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.saveVehicleType}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, vehicleType);
  }

  updateVehicleType(vehicleType: VehicleType): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.updateVehicleType}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, vehicleType);
  }


  getStationNWCSettings(filter: StationSettingsSC): Observable<DescriptiveResponse<SearchResult<StationSettingsNWC>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.GetStationNWCSettings}`;
    return this.http.post<DescriptiveResponse<SearchResult<StationSettingsNWC>>>(url, filter);
  }

  getStationNWCSetting(stationId: string): Observable<DescriptiveResponse<StationSettingsNWC>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.GetStationNWCSetting}?stationId=${stationId}`;
    return this.http.get<DescriptiveResponse<StationSettingsNWC>>(url);
  }

  saveStationNWCSettings(saveobj: StationSettingsNWC): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.SaveStationNWCSettings}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, saveobj);
  }

  saveUserPermittedLandmarks(saveobj: UserStationPermissionDTO[]): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.SaveUserPermittedLandmarks}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, saveobj);
  }

  // Station Settings - Default List Tab
  saveStationDefaultTankers(saveObj: StationDefaultTankersDTO): Observable<DescriptiveResponse<boolean>>{
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.SaveStationDefaultTankers}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, saveObj);
  }

  GetVehicleNWCSettings(filter: VehicleSettingsSC): Observable<DescriptiveResponse<SearchResult<VehicleSettingsNWC>>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.GetVehicleNWCSettings}`;
    return this.http.post<DescriptiveResponse<SearchResult<VehicleSettingsNWC>>>(url, filter);
  }

  SaveVehicleNWCSettings(saveobj: VehicleSettingsNWC[]): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.SaveVehicleNWCSettings}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, saveobj);
  }

  SaveVehicleNWCSettingsBulk(saveobj: VehicleSettingsBulkUpdate): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.SaveVehicleNWCSettingsBulk}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, saveobj);
  }

  SearchCitySettings(scDto: BranchesSearchCriteriaDTO): Observable<DescriptiveResponse<BranchSettingsDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.SearchCitySettings}`;
    return this.http.post<DescriptiveResponse<BranchSettingsDTO[]>>(url, scDto);
  }

  GetCitySetting(cityId: string): Observable<DescriptiveResponse<BranchSettingsDTO>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.GetCitySetting}?cityId=${cityId}`;
    return this.http.get<DescriptiveResponse<BranchSettingsDTO>>(url);
  }

  AddCitySettings(branchSettingsDTO: BranchSettingsDTO): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.AddCitySettings}`;
    return this.http.post<DescriptiveResponse<BranchSettingsDTO[]>>(url, branchSettingsDTO);
  }

  UpdateCitySettings(branchSettingsDTO: BranchSettingsDTO): Observable<DescriptiveResponse<boolean[]>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.UpdateCitySettings}`;
    return this.http.put<DescriptiveResponse<BranchSettingsDTO[]>>(url, branchSettingsDTO);
  }

  GetBranchSettings(branchName: string): Observable<DescriptiveResponse<BranchSettingsDTO[]>> {
    const url = `${Configuration.urls.queryEndpoint + Configuration.urls.ControlPanel.GetBranchSettings}?branchName=${branchName}`;
    return this.http.get<DescriptiveResponse<BranchSettingsDTO[]>>(url);
  }

  SaveBranchSettings(saveobj: BranchSettingsDTO[]): Observable<DescriptiveResponse<boolean>> {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.SaveBranchSettings}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, saveobj);
  }


  deleteBranch(id) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.deleteBranch}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, id);
  }

  deleteVehicleType(id: string): Observable<DescriptiveResponse<Boolean>> {
    const headers = { 'content-type': 'application/json' }
    const body = JSON.stringify(id);
    let url = Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.deleteVehicleType;
    return this.http.post<DescriptiveResponse<Boolean>>(url, body, { 'headers': headers })
  }

  deleteLandMarkType(id) {
    const url = `${Configuration.urls.commandEndpoint + Configuration.urls.ControlPanel.deleteLandMarkType}`;
    return this.http.post<DescriptiveResponse<boolean>>(url, id);
  }

 

  
}
