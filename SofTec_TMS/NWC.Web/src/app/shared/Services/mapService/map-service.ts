
import { Injectable, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import MultiLineString from 'ol/geom/MultiLineString';
import Feature from 'ol/Feature';
import Map from 'ol/Map';
import View from 'ol/View';
import { LineString } from 'ol/geom';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer';
import { Vector, XYZ, OSM } from 'ol/source';
import { transform, fromLonLat } from 'ol/proj';
import { WKT, EsriJSON, GeoJSON } from 'ol/format';
import { Icon, Text, Style, Stroke } from 'ol/style'
import { Configuration } from '../../configurations/shared.config';
import { alertService } from '../alert/alert.service';
import Overlay from 'ol/Overlay.js';
import Fill from 'ol/style/Fill';
import { Vector as VectorSource } from 'ol/source.js';
import Draw from 'ol/interaction/Draw.js';
import Point from 'ol/geom/Point.js';
import { Circle as CircleStyle } from 'ol/style.js';
import { DirectionsServiceRequestObject } from '../../datamodels/DirectionsServiceRequestObject';
import { RouteObject } from '../../datamodels/RouteObject';
import { WorkOrderPlannedRoutDTO } from '../../../TMS-Module/Models/work-order-planned-rout';

declare var $: any;
@Injectable()
export class MapService {

  map: Map;
  View: View;
  zoom: number;
  mapType: string;
  centerX: number;
  centerY: number;
  mapUrl: string;
  minZoom: number;
  maxZoom: number;
  startIconSrc: string = "/src/assets/start-flags.png";
  endIconSrc: string = "/src/assets/End-Flags.png";
  dotIconSrc: string = "/src/assets/dot.png";
  public overlay: any;
  drawingGraphicsLayer: VectorLayer;
  private geofencesGraphicsLayer: VectorLayer;
  private drawingGraphicSource: Vector = new Vector({});
  geofencesGraphicSource: Vector = new Vector({});
  RoutingResultEvent: EventEmitter<any> = new EventEmitter();
  MapLoaded: EventEmitter<boolean> = new EventEmitter();
  ZoomLevelChange: EventEmitter<number> = new EventEmitter();
  @Output()
  change: EventEmitter<number> = new EventEmitter();
  @Output()
  startMode: EventEmitter<boolean> = new EventEmitter();

  @Output()
  changeLngLatStudent: EventEmitter<number> = new EventEmitter();

  routeLine: any = [];
  routeLineChange: EventEmitter<any> = new EventEmitter();
  selectedLocation: EventEmitter<any> = new EventEmitter();
  private wktFormat: WKT = new WKT({ splitCollection: true });
  Colors: any;
  currentFeature: any;
  @Output() revGeocodingResult: EventEmitter<any> = new EventEmitter();
  @Output() Move: EventEmitter<number> = new EventEmitter();
  _studentList: any[];
  _routesGraphicLayer;
  constructor(private alert: alertService, private _http: HttpClient) {
    this.drawingGraphicsLayer = new VectorLayer({ source: this.drawingGraphicSource });
    this.geofencesGraphicsLayer = new VectorLayer({ source: this.geofencesGraphicSource });
    this.Colors = {
      route1: '#d50082',
      route2: '#00ae63',
      route3: '#3b3b3b',
      route4: '#00cdeb',
      route5: '#a52a2a',
      route6: '#0041b9',
      route7: '#006400',
      route8: '#514325',
      route9: '#008b8b',
      route10: '#ff8700'
    };
  }

  setRoutesGraphicLayer(source) {
    this._routesGraphicLayer = source;
  }
  getRoutesGraphicLayer() {
    return this._routesGraphicLayer;
  }
  public setStudentObj(students: any[]): void {
    this._studentList = students;
  }

  public getStudentObj(): any[] {
    return this._studentList;
  }
  _drawing: any;


  public setDrawObj(drawing: any): void {
    this._drawing = drawing;
  }

  public getDrawObj(): any {
    return this._drawing;
  }
  removePopup() {
    let element = document.getElementById('popup');


    $(element).popover('destroy');
  }

  SetPopup(id: number, studentList: any[]) {

    var xthis = this;
    let content = "";
    let element = document.getElementById('popup');
    var popup = new Overlay({
      element: element,
      positioning: 'bottom-center',
      stopEvent: false,
      offset: [0, 0]
    });

    $(element).popover('destroy');

    let feature = xthis.getRoutesGraphicLayer().getFeatureById(id);
    if (feature) {

      content = studentList.find(a => a.Id == feature.getId()).Name;// +  xthis.datePipe.transform(planTime ,'shortTime')  ;

      var coordinates = feature.getGeometry().getCoordinates();
      xthis.map.addOverlay(popup);
      popup.setPosition(coordinates);

      var coords = transform(coordinates, 'EPSG:3857', 'EPSG:4326');

      xthis.centerMap(coords[0], coords[1], 15);

      $(element).popover('destroy');
      $(element).popover({
        placement: 'top',
        html: true,
        content: content
      });
      $(element).popover('show');


    } else {
      $(element).popover('destroy');
      // xthis.change.emit(null);
    }
  }

  public setMapSettings(_centerLong, _centerLat, _zoom, _containerId: string, _baseMapType, geocoding: boolean, MapUrl: string, minZoom: number, maxZoom: number) {
    this.drawingGraphicsLayer = new VectorLayer({ source: this.drawingGraphicSource });
    this.geofencesGraphicsLayer = new VectorLayer({ source: this.geofencesGraphicSource });
    // Check Map configuration params and if no params set default configurations
    this.checkMapParameters(_centerLong, _centerLat, _zoom, _baseMapType, MapUrl, minZoom, maxZoom);
    var self = this;
    var baseMapLayer;
    if (this.mapType.toLowerCase() == "osm") {
      baseMapLayer = new OSM();
    }
    else {
      baseMapLayer = new XYZ({ url: this.mapUrl });
    }
    this.map = new Map({
      /*
      controls: control.defaults({
          attributionOptions: ({
              collapsible: false
          })
      }),*/
      view: new View({
        center: fromLonLat([this.centerX, this.centerY], 'EPSG:3857'),
        zoom: this.zoom,
        minZoom: this.minZoom,
        maxZoom: this.maxZoom
      }),
      layers: [
        new TileLayer({
          source: baseMapLayer
        }), this.drawingGraphicsLayer,
        this.geofencesGraphicsLayer
      ],
      target: _containerId
    });
    this.map.updateSize();
    this.drawingGraphicsLayer.setZIndex(100);
    this.registerMapClick();

    // var elem = $("#topnav").find('a').eq(0);
    // if (elem) {
    //     elem.click(function () {
    //         setTimeout(function () { self.map.updateSize(); }, 200);
    //     });
    // }
    // if (geocoding) {
    //    //this.addGeocodingControl();
    // }
    this.MapLoaded.emit(true);
    var xthis = this;
    // this.map.getView().on('change:resolution', function (e) {
    //     xthis.hideOverlay(xthis.map.getView().getZoom());
    // });

  }
  public UpdateMapSize() {
    this.map.updateSize();
  }

  public registerMapClick() {
    // let element = document.getElementById('popup');
    // var popup = new Overlay({
    //     element: element,
    //     positioning: 'bottom-center',
    //     stopEvent: false,
    //     offset: [0, 0]
    // });
    var xthis = this;
    // var featureId = 0;

    this.map.on("click", function (e) {

      xthis.map.forEachFeatureAtPixel(e.pixel, function (feature, layer) {
        let content = "";

        if (feature && typeof feature.getId() == 'number') {
          xthis.change.emit(feature.getId());
          xthis._studentList = xthis.getStudentObj();
          xthis.SetPopup(feature.getId(), xthis._studentList);
          // // let planTime : Date = new Date(xthis._trip.lstStudents.find(a=>a.Id == feature.getId()).PlanTime);
          // content = xthis._studentList.find(a => a.Id == feature.getId()).Name;// +  xthis.datePipe.transform(planTime ,'shortTime')  ;


          // xthis.map.addOverlay(popup);

          // let coordinates = feature.getGeometry().getCoordinates();
          // popup.setPosition(coordinates);

          // // let coords =    transform(coordinates, 'EPSG:3857', 'EPSG:4326');
          // // xthis.centerMap(coords[0] , coords[1],12);
          // $(element).popover('destroy');
          // $(element).popover({
          //     placement: 'top',
          //     html: true,
          //     content: content
          // });
          // $(element).popover('show');

        } else {
          // $(element).popover('destroy');
          xthis.change.emit(null);

        }
      });
      if (!xthis.getStudentObj()) {
        var raster = new TileLayer({
          source: new OSM()
        });

        var source = new VectorSource({ wrapX: false });

        var vector = new VectorLayer({
          source: source
        });
        var draw; // global so we can remove it later
        draw = new Draw({
          source: source,
          type: "Point"
        });

        xthis.map.addInteraction(draw);
        var coords = e.coordinate;
        coords = transform(coords, 'EPSG:3857', 'EPSG:4326');
        let objStd;
        let drawing = xthis.getDrawObj();

        if (drawing) {
          if (drawing.isPickUp) {
            xthis.deleteFeature('source', drawing.Source);
            xthis.addFeatureOnMap('source', 'POINT(' + coords[0] + ' ' + coords[1] + ')', drawing.Source, 'white', " ", null, xthis.startIconSrc);
            objStd = { 'lng': coords[0], 'lat': coords[1], 'type': 'pickup' };
            xthis.changeLngLatStudent.emit(objStd);

          }
          if (drawing.isDropOff) {
            xthis.deleteFeature('destination', drawing.Source);
            xthis.addFeatureOnMap('destination', 'POINT(' + coords[0] + ' ' + coords[1] + ')', drawing.Source, 'white', " ", null, xthis.endIconSrc);
            objStd = { 'lng': coords[0], 'lat': coords[1], 'type': 'dropoff' };
            xthis.changeLngLatStudent.emit(objStd);

          }
        }
      }
    });
  }



  // public getLatLongFromMapClick() {
  //     var xthis = this;
  //     if (this.getMapObject() != null) {
  //         this.map.on('pointermove', function (e) {
  //             var mapElem = document.getElementById(xthis.map.getTarget());
  //             mapElem.style.cursor = 'crosshair';
  //         });
  //         this.map.on("click", function (e) {
  //             var coords = e.coordinate;
  //             coords = proj.transform(coords, 'EPSG:3857', 'EPSG:4326');
  //             // lat , lng
  //             var addressResult = { 'lat': coords[1], 'lng': coords[0] };
  //             if (xthis.drawingGraphicSource.getFeatures().length > 0)
  //                 xthis.deleteFeature(1, xthis.drawingGraphicSource);
  //             var feature = xthis.addFeatureOnMap(1, 'POINT(' + addressResult.lng + ' ' + addressResult.lat + ')', xthis.drawingGraphicSource, "red", "");
  //              xthis.map.getView().setCenter(feature.getGeometry().getLastCoordinate());
  //             xthis.map.getView().setZoom(11);
  //             console.log('result', addressResult);
  //             xthis.revGeocodingResult.emit(addressResult);
  //         });
  //     }
  // }
  // public selectPointFromMapClick() {
  //     var xthis = this;
  //     if (this.getMapObject() != null) {
  //         this.getMapObject().on("click", function selectClick(e) {
  //             var coords = e.coordinate;
  //             coords = proj.transform(coords, 'EPSG:3857', 'EPSG:4326');
  //             if (xthis.drawingGraphicSource.getFeatures().length > 0)
  //                 xthis.deleteFeature(1, xthis.drawingGraphicSource);
  //             var feature = xthis.addFeatureOnMap(1, 'POINT(' + coords[0] + ' ' + coords[1] + ')', xthis.drawingGraphicSource, "red", "");
  //             xthis.getMapObject().un('click', selectClick);
  //             xthis.selectedLocation.emit(coords);
  //         });
  //     }
  // }
  public unregisterMapClick() {
    this.map.removeEventListener('click');

  }

  public getMapObject() {

    return this.map;
  }

  public zoomToFeature(_feature: Feature) {
    if (!_feature.getGeometry()) {
      this.alert.error('Error, the system was not able to generate route. Please check your inputs. If the problem continue, please try again later or check with your provider support team.');
      return;
    }
    var extent = _feature.getGeometry().getExtent();
    this.map.getView().fit(extent, this.map.getSize());
  }

  public getGraphicSource(type) {
    if (type == 'draw') {
      return this.drawingGraphicSource;
    }
    else {
      return this.geofencesGraphicSource
    }
  }

  public addFeatureOnMap(featureId, geometryString, source, featureColor, labelText: string = "", offsetY: number = 0, src: string = "") {

    var geofenceFeature = this.wktFormat.readFeature(geometryString);
    var geometry = geofenceFeature.getGeometry().transform("EPSG:4326", "EPSG:3857");
    geofenceFeature.setGeometry(geometry);
    geofenceFeature.setId(featureId);

    var type = geofenceFeature.getGeometry().getType();
    if (type == 'Point') {
      //    geofenceFeature.setZIndex(8);
      geofenceFeature.setStyle(new Style({
        image: new Icon(({
          color: featureColor,
          crossOrigin: 'anonymous',
          src: src == "" ? this.dotIconSrc : src,
          anchor: labelText == "" ? [0.8, 0.8] : [0.5, 0.5]
        })),
        text: new Text(({
          text: labelText,
          textBaseline: 'middle',
          font: '14px sans-serif',
          Weight: 'Bold',
          offsetY: offsetY,
          fill: new Fill({ color: "#ffffff" }),
          // stroke: new Stroke({color: "#ffffff" , width: 4}),
          // stroke: new Stroke({
          //     color: '#FFE000',
          //     width: 4
          // })
        }))

      }));
    }
    else {
      geofenceFeature.setStyle(new Style({
        stroke: new Stroke(({
          color: featureColor,
          width: 5
        }))
      }));
    }

    source.addFeature(geofenceFeature);
    source.refresh();
    return geofenceFeature;
  }

  SendMove(id) {
    this.Move.emit(id);
  }

  public deleteFeature(featureId, source) {
    //debugger
    var feature = source.getFeatureById(featureId);
    if (feature)
      source.removeFeature(feature);
    source.refresh();
  }

  public deleteAllFeature(source) {
    //debugger
    var features = source.getFeatures();
    features.forEach(feature => {
      source.removeFeature(feature);
    });
    source.refresh();
  }

  public createGraphicLayer(index = 0) {
    //var graphicSource = new source.Vector({});Layer
    var graphicLayer = new VectorLayer({ source: new Vector({}) });
    if (index > 0)
      graphicLayer.setZIndex(index);
    this.map.addLayer(graphicLayer);
    return graphicLayer;
  }

  public DeleteGraphicLayer(layer) {
    this.map.removeLayer(layer);
  }

  public clearLayerGraphics(layer) {
    if (layer) {
      layer.getSource().clear();
      layer.getSource().refresh();
    }
  }

  public UpdateGraphicLayerSource(layer, features, isEsriFormat: boolean) {
    var wktFeatures = [];
    var formattedFeatures = features;
    var gSource = new Vector({});
    layer.setSource(gSource);
    if (isEsriFormat) {
      formattedFeatures = this.formatEsriJson(features);
      gSource.addFeatures(formattedFeatures);

      this.zoomToFeature(formattedFeatures[0]);

      gSource.refresh();
      for (var feat of formattedFeatures) {
        if (feat.getGeometry()) {
          var lineCords = feat.getGeometry().getCoordinates();
          if (lineCords[0][0].constructor === Array) {
            var cords = [];
            for (var item of lineCords)
              for (var elem of item)
                cords.push(elem);
            var newGeom = new LineString(cords);
            feat.setGeometry(newGeom);
          }

          var wktObject = { "BusId": feat.values_.Name.split('_')[1], "wkt": this.wktFormat.writeFeature(feat, { "dataProjection": 'EPSG:4326', "featureProjection": 'EPSG:3857' }), "attributes": feat };
          //var wktObject = { "BusId": feat.getId(), "wkt": this.wktFormat.writeFeature(feat, { "dataProjection": 'EPSG:4326', "featureProjection": 'EPSG:3857' }), "attributes": feat };
          wktFeatures.push(wktObject);
        }
      }
    }
    return wktFeatures;
  }
  public addGeoJsonOnMap(layer, geoJsonObject) {
    layer.setSource(this.formatGeoJson(geoJsonObject));
  }

  public formatEsriJson(features) {
    var esrijsonFormat = new EsriJSON();
    var formattedFeaturs = [];
    for (var feat of features) {
      let feature: Feature = esrijsonFormat.readFeature(feat, { dataProjection: 'EPSG:4326', featureProjection: 'EPSG:3857' });
      // set bus ID for Route Feature ID
      feature.setId(feat.attributes.Name.split("_")[1]);
      feature.Color = this.getRandomColor();
      feature.setStyle(new Style({
        stroke: new Stroke({
          color: feature.Color,
          width: 3.5
        })
      }));
      formattedFeaturs.push(feature);
    }
    return formattedFeaturs;
  }

  public formatGeoJson(geojsonObject) {
    var Vector = new Vector({
      features: (new GeoJSON()).readFeatures(geojsonObject)
    });
    return Vector;
  }

  centerMap(long, lat, zoom = null) {
    if (zoom)
      this.map.getView().setZoom(zoom);
    this.map.getView().setCenter(fromLonLat([long, lat], 'EPSG:3857'));
  }

  highlightFeature(WktString, id, clearFeatures = false) {
    if (clearFeatures)
      this.drawingGraphicSource.clear();
    else {
      if (this.drawingGraphicSource.getFeatures().length >= 2) {
        this.drawingGraphicSource.removeFeature(this.currentFeature);
        this.drawingGraphicSource.refresh();
      }
    }

    var color = "#FF00FF"; // FUCHSIA  #40E0D0
    this.currentFeature = this.addFeatureOnMap(id, WktString, this.drawingGraphicSource, color);
    this.zoomToFeature(this.currentFeature);
  }

  getRandomColor() {
    var result;
    var count = 0;
    for (var prop in this.Colors)
      if (Math.random() < 1 / ++count)
        result = this.Colors[prop];

    return result;
  }

  emitTodraw(routeLine, self, legWaypoints, distance, duration, wayPointsArrival) {
    var feature = new Feature({
      geometry: new MultiLineString(routeLine)
    });

    var wktRoute = self.wktFormat.writeFeature(feature);

    if (legWaypoints.length == 0) {
      self.RoutingResultEvent.emit({ route: wktRoute, distance: distance, duration: duration });
    }
    else {
      self.RoutingResultEvent.emit({ route: wktRoute, distance: distance, duration: duration, wayPointsArrival: wayPointsArrival });
    }
  }

  ngOnAfterViewInit() { }

  public getMapLayers() {

    var layer;
    return this.map.getLayers();
  }

  public createGraphicLayerWithId(id: string, Name: string): any {
    var graphicSource = new Vector({});
    var graphicLayer = new VectorLayer({ source: graphicSource });
    graphicLayer.id = id;
    graphicLayer.Name = Name;
    this.map.addLayer(graphicLayer);
    return graphicLayer;
  }

  public getCenter() {
    var cords = this.getMapObject().getView().getCenter();
    return transform(cords, 'EPSG:3857', 'EPSG:4326');
  }

  public getZoomLevel() {
    return this.getMapObject().getView().getZoom();
  }

  /* Private Methods */
  private decode(encoded) {
    // array that holds the points
    var points = []
    var index = 0, len = encoded.length;
    var lat = 0, lng = 0;

    while (index < len) {
      var b, shift = 0, result = 0;
      do {

        b = encoded.charAt(index++).charCodeAt(0) - 63;//finds ascii                                                                                    //and substract it by 63
        result |= (b & 0x1f) << shift;
        shift += 5;
      } while (b >= 0x20);


      var dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
      lat += dlat;
      shift = 0;
      result = 0;
      do {
        b = encoded.charAt(index++).charCodeAt(0) - 63;
        result |= (b & 0x1f) << shift;
        shift += 5;
      } while (b >= 0x20);
      var dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
      lng += dlng;

      points.push({ latitude: (lat / 1E5), longitude: (lng / 1E5) })

    }
    return points
  }

  // Check and validate map paramaters
  private checkMapParameters(centerLong, centerLat, zoom, baseMapType, MapUrl, minZoom, maxZoom): void {
    this.centerX = centerLong ? centerLong : Configuration.defaultMapParams.centerX;
    this.centerY = centerLat ? centerLat : Configuration.defaultMapParams.centerY;
    this.mapType = baseMapType ? baseMapType : Configuration.defaultMapParams.mapType;
    this.zoom = zoom ? zoom : Configuration.defaultMapParams.zoom;
    this.minZoom = minZoom ? minZoom : Configuration.defaultMapParams.minZoom;
    this.maxZoom = maxZoom ? maxZoom : Configuration.defaultMapParams.maxZoom;
    if (this.mapType.toLowerCase() != 'osm') {
      if (!MapUrl) {
        if (this.mapType == 'google')
          this.mapUrl = Configuration.urls.GoogleMapUrl;
        else if (this.mapType == 'esri')
          this.mapUrl = Configuration.urls.ESRIMapUrl;
      }
      else
        this.mapUrl = MapUrl;
    }
    else
      MapUrl = null;
  }
  speed = 1000;
  animating = false;

  now;
  route: any;

  moveFeature(event) {
    var styles = {
      'route': new Style({
        stroke: new Stroke({
          width: 6, color: [237, 212, 0, 0.8]
        })
      }),
      'icon': new Style({
        image: new Icon({
          anchor: [0.5, 1],
          src: 'data/icon.png'
        })
      }),
      'geoMarker': new Style({
        image: new CircleStyle({
          radius: 7,
          fill: new Fill({ color: 'black' }),
          stroke: new Stroke({
            color: 'white', width: 2
          })
        })
      })
    };

    var xthis = this;
    var routeCoords = this.route.getCoordinates();
    var vectorContext = event.vectorContext;
    var frameState = event.frameState;

    if (this.animating) {
      var elapsedTime = frameState.time - this.now;
      // here the trick to increase speed is to jump some indexes
      // on lineString coordinates
      var index = Math.round(this.speed * elapsedTime / 1000);

      // if (index >= routeLength) {
      //     //stopAnimation(true);
      //     return;
      // }

      var currentPoint = new Point(routeCoords[index]);
      var feature = new Feature(currentPoint);
      vectorContext.drawFeature(feature, styles.geoMarker);
    }
    // tell OpenLayers to continue the postcompose animation
    xthis.map.render();
  };
  PlayMode() {
    var xthis = this;
    this.route = xthis.getRoutesGraphicLayer().getFeatureById('BusOne');
    this.animating = true;
    this.now = new Date().getTime();
    // speed = speedInput.value;
    // startButton.textContent = 'Cancel Animation';
    // hide geoMarker
    // geoMarker.setStyle(null);
    // just in case you pan somewhere else
    //xthis.map.getView().setCenter(center);
    xthis.map.on('postcompose', this.moveFeature);
    xthis.map.render();
  }

  interval: any;
  busIconSrc: string = "/src/assets/bus.png";
  routesGraphicLayer: any;
  public setObj(interval: any): void {
    this.interval = interval;
  }

  public getObj() {
    return this.interval;
  }
  index: number = 0;
  public setIndex(i: number): void {
    this.index = i;
  }

  public getIndex() {
    return this.index;
  }

  calculateRoute(source: any, destination: any, wayPoints: string = "") {
    var requestObject = new DirectionsServiceRequestObject();
    var lineCords = [];

    var self = this;
    requestObject.source = source.Latitude + "," + source.Longitude;
    requestObject.destination = destination.Latitude + "," + destination.Longitude;

    if (wayPoints != "")
      requestObject.waypoints = wayPoints;

    this._http.post(Configuration.urls.queryEndpoint + Configuration.urls.calculateRouteUrl, requestObject).subscribe(response => {
      
      if (response) {
        var result: any = response;
        if (result) {
          result = result.Value;
          var steps = result.routes[0].legs[0].steps;
          var distance = result.routes[0].legs[0].distance.value;
          distance = distance ? distance / 1000 : 0;
          var duration = result.routes[0].legs[0].duration.value;
          duration = duration ? duration / 60 : 0;
          var routeLine: any = [];
          var wayPointsArrival = [];

          for (var leg of result.routes[0].legs) {
            var steps = leg.steps;
            // array of leg waypoints that holds step_index of each waypoint
            var legWaypoints = [];
            legWaypoints = leg.via_waypoint;

            if (steps) {
              var waypointArrivalTime = 0;
              for (var index in steps) {
                var step = steps[index];
                //waypointArrivalTime += step.duration.value; // value in seconds
                if (legWaypoints) {
                  var indexes = legWaypoints.filter(w => w.step_index == index);
                  if (indexes.length > 0) {
                    for (var i in indexes) {
                      var durationStep = (Number.parseInt(step.duration.value)) * (Number.parseInt(i) + 1);
                      waypointArrivalTime += durationStep;
                      wayPointsArrival.push(waypointArrivalTime / 60);
                      // waypointArrivalTime = 0;
                    }
                  }
                  else
                    waypointArrivalTime += step.duration.value
                }

                var polylinePoints = this.decode(step.polyline.points);
                lineCords = [];

                for (var point of polylinePoints)
                  lineCords.push([point.longitude, point.latitude]);

                routeLine.push(lineCords);
              }
            }
          }

          var feature = new Feature({
            geometry: new MultiLineString(routeLine)
          });

          var wktRoute = self.wktFormat.writeFeature(feature);
          if (legWaypoints.length == 0) {
            self.RoutingResultEvent.emit({ route: wktRoute, distance: distance, duration: duration });
          }
          else {
            self.RoutingResultEvent.emit({ route: wktRoute, distance: distance, duration: duration, wayPointsArrival: wayPointsArrival });
          }
        }
      }
    },
      error => {
        // alert('error');
      });
  }

  drawRouteInMap(orders: RouteObject, color, layer?) {
    let GraphicLayer = layer ? layer : this.createGraphicLayer();

    // for (let bus of buses) {
    //let orders: WorkOrderRoute = new WorkOrderRoute();
    let jsonStr;

    color = color ? color : '#228B22';
    jsonStr = JSON.parse(orders.RouteName);

    this.clearLayerGraphics(GraphicLayer);
    this.addFeatureOnMap('orders', jsonStr.RouteLine.LineGeometry, GraphicLayer.getSource(), color);

    //this.centerMap(orders.LandmarksIds[0].Longitude, orders.LandmarksIds[0].Latitude, 12);
  }

  SaveOrderRoute(WorkOrderPlannedRout: WorkOrderPlannedRoutDTO) {
    this._http.post(Configuration.urls.commandEndpoint + Configuration.urls.SaveOrderRouteUrl, WorkOrderPlannedRout).subscribe(response => {
    });
  }
}
