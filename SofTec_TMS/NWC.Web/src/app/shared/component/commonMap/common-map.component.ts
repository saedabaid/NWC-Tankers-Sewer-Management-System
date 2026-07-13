import { Component, ViewEncapsulation, EventEmitter, Output, ElementRef, Input } from '@angular/core';
import { MapService } from '../../Services/mapService/map-service';
@Component({
    selector: 'common-map',
    templateUrl: './common-map.component.html',
  styleUrls: ['./common-map.component.scss'],
    encapsulation: ViewEncapsulation.None,
})

export class CommonMapComponent {
    @Input() zoom: number;
    @Input() mapType: string;
    centerX: number;
    centerY: number;
    mapUrl: string;
    minZoom: number;
    maxZoom: number;
    @Output() MapComponentLoaded: EventEmitter<any> = new EventEmitter();
    constructor(private _mapService: MapService, private elRef: ElementRef) { }
    async  ngOnInit() {

        // var result = await this._appConfigService.getSubscriberConfiguration();
        // if (result) {
        //     // get Longitude Value
        //     var long = result.Configurations.find((x) => x.Key == ConfigurationKeyEnum.DefaultLongitude.toString());
        //     this.centerX = long ? parseFloat(long.Value) : null;
        //     // get Latitude Value
        //     var lat = result.Configurations.find((x) => x.Key == ConfigurationKeyEnum.DefaultLatitude.toString());
        //     this.centerY = lat ? parseFloat(lat.Value) : null;
        //     // get Zoom Value
        //     var zoomResult = result.Configurations.find((x) => x.Key == ConfigurationKeyEnum.DefaultZoom.toString());
        //     this.zoom = zoomResult ? parseInt(zoomResult.Value) : null;
        //     // get Map Url Value
        //     var MapUrlResult = result.Configurations.find((x) => x.Key == ConfigurationKeyEnum.MapUrl.toString());
        //     //this.mapUrl = MapUrlResult ? MapUrlResult.Value : null;
        //     this.mapUrl = MapUrlResult ? MapUrlResult.Value : null;
        //     // get Map type Value
        //     var mapTypeResult = result.Configurations.find((x) => x.Key == ConfigurationKeyEnum.MapProvider.toString())
        //     this.mapType = mapTypeResult ? mapTypeResult.Value : null;
        //     // get min zoom Value
        //     var minZoomResult = result.Configurations.find((x) => x.Key == ConfigurationKeyEnum.MinZoomLevel.toString());
        //     this.minZoom = minZoomResult ? parseInt(minZoomResult.Value) : null;
        //     // get max zoom Value
        //     var maxZoomResult = result.Configurations.find((x) => x.Key == ConfigurationKeyEnum.MaxZoomLevel.toString());
        //     this.maxZoom = maxZoomResult ? parseInt(maxZoomResult.Value) : null;

        this._mapService.setMapSettings(this.centerX, this.centerY, this.zoom, this.elRef.nativeElement.firstChild.id, this.mapType, false, this.mapUrl, this.minZoom, this.maxZoom);
        // var layer = this._mapService.createGraphicLayer();
        // var feature = this._mapService.addFeatureOnMap(1,'LINESTRING (30.25 29.15, 30.15 29.005, 31.58 30.225)',layer.getSource(),'red');
        // this._mapService.zoomToFeature(feature);
        // }
        //this._PlanService.GetRoutesResults('jaf1d20ca3eff46fbae150d8e7dc89a56');

    }
    zoomToPoint(x, y, zoom) {
        this._mapService.centerMap(parseFloat(x), parseFloat(y), parseInt(zoom));
    }

    getZoomLevel() {
        return this._mapService.getZoomLevel();
    }
    getCenter() {
        return this._mapService.getCenter();
    }
    ngAfterViewInit() {

    }
}


