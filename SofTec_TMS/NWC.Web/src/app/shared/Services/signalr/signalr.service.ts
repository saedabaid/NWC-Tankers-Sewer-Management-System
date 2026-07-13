import { Injectable } from "@angular/core";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { BehaviorSubject, Subject } from "rxjs";
import { Configuration } from "../../configurations/shared.config";

@Injectable({ providedIn: "root" })
export class SignalRService {
  get connection(): HubConnection {
    return this._connection;
  }
  private _connection: HubConnection;
  connected = new BehaviorSubject<boolean>(false);

  connect(token: string) {
    this._connection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Trace)
      .withUrl(
        Configuration.urls.signalREndpoint +
          Configuration.urls.signalRDriversHub,
        { accessTokenFactory: () => token }
      )
      .build();
    this._connection
      .start()
      .then(() => {
        this.connected.next(true);
        console.log("SignalR Connected!");
      })
      .catch(function (err) {
        return console.error(err.toString());
      });
  }
}
