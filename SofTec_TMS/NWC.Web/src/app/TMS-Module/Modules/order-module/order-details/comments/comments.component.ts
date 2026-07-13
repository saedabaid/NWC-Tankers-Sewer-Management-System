import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { OrderComment } from '../../../../Models/order-comment';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";

@Component({
  selector: 'comments',
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.scss']
})
export class CommentsComponent implements OnInit {
comment = {count:1, listComments:[]}
  constructor() { }

  ngOnInit() {
    
  }
  
  @Input('OrderComments') public set OrderComments(value: OrderComment[] ) {
    if (!isNullOrUndefined(value)) {
      this.comment.listComments = value;
      this.comment.count = value.length;
    }
  }
  
  @Output() private addComment = new EventEmitter();
  
 add(){
  this.addComment.emit()
 }

 removeComment(){
   this.comment.listComments.splice(0, 1);
 }
}
