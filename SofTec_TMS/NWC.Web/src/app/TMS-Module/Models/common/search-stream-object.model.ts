import { Subject, interval } from 'rxjs';
import { throttle, mergeMap, map } from 'rxjs/operators';
import { Configuration } from 'src/app/shared/configurations/shared.config';

export class SearchStreamObject {
    constructor(public StreamName: string) { }

    Stream: Subject<string>;
}

export class SearchStream {

    StreamList: SearchStreamObject[] = [];

    initStream(streamName: string, func) {

        let myStream = this.StreamList.find(a => a.StreamName === streamName);
        if (!myStream) {
            let newStream = new SearchStreamObject(streamName);
            this.StreamList.push(newStream);
            myStream = newStream;
        }

        if (myStream.Stream)
            return myStream.Stream;

        myStream.Stream = new Subject<string>();
        myStream.Stream.pipe(throttle(ev => interval(Configuration.DropDownListSearch.Miliseconds),
         { leading: false, trailing: true })).subscribe((a) => func(a));

        return myStream.Stream;
    }

    DestroyStreams() {
        this.StreamList.forEach(element => {
            element.Stream.unsubscribe();
        });
    }

}