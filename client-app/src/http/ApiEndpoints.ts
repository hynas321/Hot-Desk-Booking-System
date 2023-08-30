class ApiEndpoints {
  //UserController
  static logIn: string = "/api/User/LogIn";
  static logOut: string = "/api/User/LogOut";
  static isAdmin: string = "/api/User/IsAdmin";

  //LocationController
  static addLocation: string = "/api/Location/Add";
  static removeLocation: string = "/api/Location/Remove";
  static getAllLocationNames: string = "/api/Location/GetAllNames";
  static getDesks: string = "/api/Location/GetDesks";

  //DeskController
  static addDesk: string = "/api/Desk/Add";
  static removeDesk: string = "/api/Desk/Remove";
  static bookDesk: string = "/api/Desk/Book";
  static unbookDesk: string = "/api/Desk/Unbook";
}
  
  export default ApiEndpoints;