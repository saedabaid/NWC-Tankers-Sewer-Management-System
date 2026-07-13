export class  RouteObject {
    Id: number;
    RouteLine: LineShape;
    RouteName: string;
    TotalTime: number; //in minutes
    TotalDistance: number;
    RouteStyle: string = null;
    POITypeId: number=1006;
    BufferSize: number = 20;

}
export class LineShape {
    LineGeometry: string;
}