class ApiEndpoints {
  //UserController
  static logIn: string = "/api/User/LogIn";
  static logOut: string = "/api/User/LogOut";
  static getUserInfo: string = "/api/User/GetUserInfo";

  //LocationController
  static addLocation: string = "/api/Location/Add";
  static removeLocation: string = "/api/Location/Remove";
  static getAllLocationNames: string = "/api/Location/GetAllNames";
  static getDesks: string = "/api/Location/GetDesks";

  //DeskController
  static addDesk: string = "/api/Desk/Add";
  static removeDesk: string = "/api/Desk/Remove";
  static setDeskAvailability: string = "/api/Desk/SetDeskAvailability";

  //BookingController
  static bookDesk: string = "/api/Booking/Book";
  static unbookDesk: string = "/api/Booking/Unbook";
}
  
  export default ApiEndpoints;