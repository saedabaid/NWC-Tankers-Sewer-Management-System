import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";
//import { BsModalService, BsModalRef } from "ngx-bootstrap";
import { OrderComment } from "../../../Models/order-comment";
import { OrderDetailsService } from "../../../Services/order-details.service";
import { ActivatedRoute } from "@angular/router";
import { DatePipe } from "@angular/common";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { LoaderService } from "src/app/shared/loader.service";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";

@Component({
  selector: "app-add-order-comment",
  templateUrl: "./add-order-comment.component.html",
  styleUrls: ["./add-order-comment.component.scss"]
})
export class AddOrderCommentComponent implements OnInit {
  Comment: OrderComment = new OrderComment();
  @Input() orderId: number;
  OrderCommentsList: OrderComment[];
  constructor(
    private _alert: alertService,
    private datePipe: DatePipe,
    private route: ActivatedRoute,
    private OrderDetailsService: OrderDetailsService,
    private modalService: BsModalService,
    private modalRef: BsModalRef,
    private mainloading: LoaderService
  ) {}

  ngOnInit() {}

  @Output() OrderComments: EventEmitter<OrderComment[]> = new EventEmitter<OrderComment[]>();

  save() {
    this.Comment.WorkOrderID = this.orderId;

    this.mainloading.PreloaderIcreaseCount();
    this.OrderDetailsService.addComment(this.Comment).subscribe(
      res => {
        if (res.IsErrorState == false) {
          this._alert.showSuccess();
          this.OrderDetailsService.getOrderComments(this.orderId).subscribe(
            res => {
              if (res.Value != null) {
                this.OrderCommentsList = res.Value;
                this.OrderComments.emit(this.OrderCommentsList);
                this.closePopup();
              }
            }
          );
        } else {
          this._alert.showError();
        }
      },
      err => {
        this.mainloading.PreloaderDecreaseCount();
      },
      () => {
        this.mainloading.PreloaderDecreaseCount();
      }
    );
  }

  doTextareaValueChange($event) {
    this.Comment.Comment = $event.target.value;
  }
  closePopup() {
    this.modalRef.hide();
  }
}
