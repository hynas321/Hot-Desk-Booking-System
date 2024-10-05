class ApiEndpoints {
  //UserController
  static readonly logIn: string = "/api/User/LogIn";
  static readonly logOut: string = "/api/User/LogOut";
  static readonly getUserInfo: string = "/api/User/GetUserInfo";
  static readonly refreshToken: string = "/api/User/RefreshToken";

  //LocationController
  static readonly addLocation: string = "/api/Location/Add";
  static readonly removeLocation: string = "/api/Location/Remove";
  static readonly getAllLocationNames: string = "/api/Location/GetAllNames";
  static readonly getDesks: string = "/api/Location/GetDesks";

  //DeskController
  static readonly addDesk: string = "/api/Desk/Add";
  static readonly removeDesk: string = "/api/Desk/Remove";
  static readonly setDeskAvailability: string = "/api/Desk/SetDeskAvailability";

  //BookingController
  static readonly bookDesk: string = "/api/Booking/Book";
  static readonly unbookDesk: string = "/api/Booking/Unbook";
}

export default ApiEndpoints;
