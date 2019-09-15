import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {}

  sendLike(id: number) {
    this.userService
      .sendLike(this.authService.decodedToken.nameid, id)
      .subscribe(
        () => {
          this.alertify.success('You liked: ' + this.user.knownAs);
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
