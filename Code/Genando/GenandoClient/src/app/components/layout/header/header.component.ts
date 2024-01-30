import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  Input,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import {
  HeaderTabIcon,
  profileImage,
} from 'src/app/constants/profile-image/profile-image';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { HeaderTabConstant, UserRole } from 'src/app/constants/system-constant';
import { IResponse } from 'src/app/models/shared/response';
import { AuthService } from 'src/app/services/auth.service';
import { HeaderService } from 'src/app/services/header.service';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { environment } from 'src/environments/environment';
import { NotificationResult } from 'src/app/shared/models/notification';
import * as signalR from '@microsoft/signalr';
import * as Hammer from 'hammerjs';
import { MessageConstant } from 'src/app/constants/message-constant';
import { SwipeSetting } from 'src/app/shared/models/swipe-setting';
import { Howl, Howler } from 'howler';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements AfterViewInit {
  @ViewChild('notificationsList') notificationsList!: ElementRef;
  @Input() avatar: string = profileImage.gridNoImg;
  userRole: string | null = '3';
  routingUrl!: string;
  links!: Array<{ text: string; icon: string; route: string }>;
  activeLinkIndex: number = 0;
  userName: string | null = '';
  isIconContainerVisible = true;
  isMobileScreen = false;
  mobileScreenWidth = 800;
  notificationCount: number = 0;
  notificationResult: NotificationResult[] = [];
  isDropdownOpen: { [key: number]: boolean } = {};
  openDropdownId: number | null = null;
  bellDropdownOpen: boolean = false;
  gearDropdownOpen: boolean = false;
  tempDeletedNotifications: boolean = false;
  showSwipeActions: boolean = false;
  swipeSetting!: SwipeSetting;
  notificationSound!: Howl;
  constructor(
    private router: Router,
    private authService: AuthService,
    private headerService: HeaderService,
    private notificationService: NotificationService,
    private renderer: Renderer2
  ) {
    router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.setActiveLinkBasedOnRoute(event.urlAfterRedirects);
      }
    });

    this.notificationSound = new Howl({
      src: ['assets/audio/notification-tone.wav'],
      volume: 1.0,
    });
  }

  ngOnInit() {
    this.getSwipeSettings();
    this.setupEventListeners();
    this.getNotificationData();
    this.signalRConnection();
    this.loadUserAvatar();
    this.setUserInfo();
    this.initializeLinks();
    this.checkScreenWidth();
  }
  ngAfterViewInit() {
    const nativeElement = this.renderer.selectRootElement(
      this.notificationsList.nativeElement
    );
    const mc = new Hammer(nativeElement);
    mc.get('swipe').set({ direction: Hammer.DIRECTION_HORIZONTAL });
    mc.on('swipeleft', () => this.swipeLeftHandler(null));
    mc.on('swiperight', () => this.swipeRightHandler(null));
  }

  ngOnDestroy() {
    this.removeEventListeners();
  }

  setupEventListeners() {
    document.body.addEventListener('click', this.onDocumentClick.bind(this));
  }

  removeEventListeners() {
    document.body.removeEventListener('click', this.onDocumentClick);
  }

  onDocumentClick(event: MouseEvent) {
    const bellDropdown = document.querySelector('.bell-icon-container');
    if (bellDropdown && !bellDropdown.contains(event.target as Node)) {
      this.closeToggleDropdown();
    }
  }

  closeToggleDropdown() {
    this.openDropdownId = null;
    this.gearDropdownOpen = false;
    this.showSwipeActions = false;
  }

  setActiveLink(index: number) {
    this.activeLinkIndex = index;
  }

  setActiveLinkBasedOnRoute(url: string) {
    for (let i = 0; i < this.links?.length; i++) {
      if (this.links[i].route && url.includes(this.links[i].route)) {
        this.activeLinkIndex = i;
        break;
      }
    }
  }

  setUserInfo() {
    this.userName = this.authService.getUserName();
    this.userRole = this.authService.getUserType();
    this.routingUrl = this.getRoutingUrlBasedOnUserRole(this.userRole);
  }

  loadUserAvatar() {
    this.headerService.getAvatar().subscribe({
      next: (data: IResponse<string>) => {
        if (data.data !== '' && data.data) {
          this.avatar = data.data;
        }
        this.avatar ??= profileImage.gridNoImg;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  initializeLinks() {
    this.links = [
      {
        text: HeaderTabConstant.dashboard,
        icon: HeaderTabIcon.dashboard,
        route: this.routingUrl + RoutingPathConstant.dashboardHead,
      },
      {
        text: HeaderTabConstant.patients,
        icon: HeaderTabIcon.patient,
        route: this.routingUrl + RoutingPathConstant.patientsHead,
      },
      {
        text: HeaderTabConstant.lab,
        icon: HeaderTabIcon.lab,
        route: this.routingUrl + RoutingPathConstant.labHead,
      },
      {
        text: HeaderTabConstant.faq,
        icon: HeaderTabIcon.faq,
        route: this.routingUrl + RoutingPathConstant.faqHead,
      },
      {
        text: HeaderTabConstant.contactUs,
        icon: HeaderTabIcon.contactUs,
        route: this.routingUrl + RoutingPathConstant.contactUsHead,
      },
      {
        text: HeaderTabConstant.contactDoctor,
        icon: HeaderTabIcon.contactDoctor,
        route: this.routingUrl + RoutingPathConstant.contactDoctorHead,
      },
    ];

    this.filterLinksBasedOnUserRole();
    this.setActiveLinkBasedOnRoute(this.router.url);
  }

  filterLinksBasedOnUserRole() {
    this.links = this.links.filter((link) => {
      if (this.userRole == UserRole.labRoleId) {
        return (
          link.text == HeaderTabConstant.dashboard ||
          link.text == HeaderTabConstant.faq ||
          link.text == HeaderTabConstant.contactUs
        );
      }
      if (this.userRole == UserRole.patientRoleId) {
        return (
          link.text == HeaderTabConstant.dashboard ||
          link.text == HeaderTabConstant.faq ||
          link.text == HeaderTabConstant.contactDoctor
        );
      }
      if (this.userRole == UserRole.doctorRoleId) {
        return (
          link.text == HeaderTabConstant.dashboard ||
          link.text == HeaderTabConstant.faq ||
          link.text == HeaderTabConstant.contactUs ||
          link.text == HeaderTabConstant.patients ||
          link.text == HeaderTabConstant.lab
        );
      }
      return true;
    });
  }

  getRoutingUrlBasedOnUserRole(userRole: string) {
    switch (userRole) {
      case UserRole.doctorRoleId:
        return RoutingPathConstant.doctorHead;
      case UserRole.patientRoleId:
        return RoutingPathConstant.patientHead;
      case UserRole.labRoleId:
        return RoutingPathConstant.labHead;
      default:
        return '';
    }
  }

  hasUnreadNotifications(): boolean {
    return this.notificationResult.some(
      (notification) => !notification.hasRead
    );
  }

  toggleDropdown(id: number) {
    if (this.openDropdownId === id) {
      this.openDropdownId = null;
    } else {
      this.openDropdownId = id;
    }
  }

  toggleGearDropdown() {
    this.gearDropdownOpen = !this.gearDropdownOpen;
  }

  onBellDropdownHidden() {
    this.bellDropdownOpen = false;
  }

  toggleBellDropdown(open: boolean) {
    this.bellDropdownOpen = open;
    this.closeToggleDropdown();
  }

  toggleSwipeActions() {
    this.showSwipeActions = true;
    this.tempDeletedNotifications = false;
  }

  toggleNotificationTone() {
    this.showSwipeActions = false;
    this.tempDeletedNotifications = false;
  }
  closeGearDropdown() {
    this.gearDropdownOpen = false;
  }
  signalRConnection() {
    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(environment.baseUrl + 'notify')
      .build();

    connection
      .start()
      .then(() => {
        console.log(MessageConstant.serviceConnected);
      })
      .catch((err) => {
        console.error(err.toString());
      });

    connection.on('BroadcastMessage', () => {
      setTimeout(() => {
        this.getNotificationCount();
      }, 500);
      setTimeout(() => {
        this.getNotificationMessage();
        this.notificationSound.play();
      }, 500);
    });
  }

  getNotificationData() {
    this.getNotificationCount();
    this.getNotificationMessage();
  }

  getNotificationCount() {
    this.notificationService.getNotificationCount().subscribe(
      (notificationCount: any) => {
        this.notificationCount = notificationCount.data;
      },
      (error) => {
        console.error(MessageConstant.errorFetchNotiCount, error);
      }
    );
  }

  getNotificationMessage() {
    this.notificationService.getNotificationMessage().subscribe({
      next: (notificationResult: any) => {
        this.notificationResult = notificationResult.data.filter(
          (item: any) => item.isTempDeleted === false
        );
        this.tempDeletedNotifications = false;
        this.gearDropdownOpen = false;
        this.showSwipeActions = false;
      },
      error: (error) =>
        console.error(MessageConstant.errorFetchNotiCount, error),
    });
  }

  showDeletedNotifications() {
    this.notificationService.getNotificationMessage().subscribe({
      next: (notificationResult: any) => {
        this.notificationResult = notificationResult.data.filter(
          (item: any) => item.isTempDeleted === true
        );
        this.tempDeletedNotifications = true;
        this.gearDropdownOpen = false;
        this.showSwipeActions = false;
      },
      error: (error) =>
        console.error(MessageConstant.errorFetchNotiCount, error),
    });
  }

  markAsReadNotifications(id: number | null) {
    this.notificationService.readNotifications(id).subscribe({
      next: () => {
        this.getNotificationData();
        this.openDropdownId = null;
      },
      error: (error) =>
        console.error(MessageConstant.errorFetchNotiCount, error),
    });
  }

  deleteNotifications(id: number | null) {
    this.notificationService.deleteNotifications(id).subscribe({
      next: () => {
        this.getNotificationData();
        this.openDropdownId = null;
      },
      error: (error) =>
        console.error(MessageConstant.errorFetchNotiCount, error),
    });
  }

  getSwipeSettings() {
    this.notificationService.getSwipeSetting().subscribe({
      next: (swipeSetting: any) => {
        this.swipeSetting = swipeSetting.data;
      },
      error: (error) =>
        console.error(MessageConstant.errorNotiCount, error),
    });
  }

  swipeLeftHandler(id: number | null) {
    if (this.swipeSetting.swipeLeftAction === 'Delete') {
      this.deleteNotifications(id);
    } else if (this.swipeSetting.swipeLeftAction === 'Read') {
      this.markAsReadNotifications(id);
    }
  }

  swipeRightHandler(id: number | null) {
    if (this.swipeSetting.swipeRightAction === 'Delete') {
      this.deleteNotifications(id);
    } else if (this.swipeSetting.swipeRightAction === 'Read') {
      this.markAsReadNotifications(id);
    }
  }

  saveSwipeActions() {
    this.notificationService.saveSwipeSetting(this.swipeSetting).subscribe({
      next: (swipeSetting: any) => {
        this.getSwipeSettings();
      },
      error: (error) =>
        console.error(MessageConstant.errorNotiCount, error),
    });
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event) {
    this.checkScreenWidth();
  }

  toggleIconContainer() {
    if (this.isMobileScreen) {
      this.isIconContainerVisible = !this.isIconContainerVisible;
    }
  }

  checkScreenWidth() {
    this.isMobileScreen = window.innerWidth <= this.mobileScreenWidth;
    this.isIconContainerVisible = !this.isMobileScreen;
  }

  logout() {
    this.authService.logOut();
  }
}
