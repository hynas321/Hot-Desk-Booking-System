# Hot Desk Booking System

## Introduction

**Hot Desk Booking System** is a website that allows booking a desk in the chosen location. Access to this system is possible only through the account. There are both **Users** and **Administrators**, each with different permissions.

Sample credentials (username, password) to log in and test the program:

- User1 user1 (role: User)
- User2 user2 (role: User)
- Admin admin (role: Admin)

## Technological overview

**Frontend**: `Angular` `Typescript` `Bootstrap` \
**Backend**: `ASP.NET Core` `Sql Server (Docker)` `Entity Framework Core` \
**Testing**: `XUnit` `TestContainers` `Docker`

## Configuration commands

Frontend (AngularApp)
- `npm install`
- `ng serve`

Backend (WebApi): 
- `docker-compose up` (to run Sql Server database)
- `dotnet run` (with http for local environment)

Integration Tests (WebApi.IntegrationTests): 
- `dotnet test`

## Role: Administrator

Administrators are able to:

- **add a location**,
- **remove a location** - if they have no desks assigned to them,
- **add a desk** - to an already existing location,
- **remove a desk** - if available,
- **disable a desk** - if available,
- **enable a desk**,
- **book a desk** - if available,
- **unbook a desk** - if its their own.

These are the things that Administrators can accomplish directly on the website via UI. However, there are more functionalities available for them only through **endpoints** (which are described in more detail later). They can, as well, among others:

- add a user,
- remove a user,
- get full information about every location and its desks.

## Role: User

**Users** are able to:

- **book a desk** - if available,
- **unbook a desk** - if its their own.

Of course, users see locations and desks, however they cannot modify them in anyway. Booking is the only exception.

## Booking

Booking rules:

- only one booking at the time,
- the maximum timespan is 7 days,
- booking starts immediately and lasts until the end of the day.
- expired bookings are erased automatically after midnight

## Endpoints

Endpoints can be accessed easily via Postman or **Swagger**. Swagger is extremely convenient. Here is the sample URL to access it: `http://localhost:5062/swagger/index.html`.

Endpoints cannot be accessed without a JSON Web Token. The token is generated when the  user logs in and is removed from the session storage when the user logs out.

## User interface - screenshots

- Main view
  ![image](https://github.com/user-attachments/assets/9fc0c5a2-e776-453e-a5e8-a0aa6243c803)

- Locations view (from Employee's POV)
  ![image](https://github.com/user-attachments/assets/0c79ba9a-7cfc-49dc-ad01-df2caa95fa8e)

- Locations view (from Admin's POV - adding a new location popup)
  ![image](https://github.com/user-attachments/assets/e19b0819-8951-4fd8-a533-46029148812b)

- Locations view (from Admin's POV - removing a location snackbar)
  ![image](https://github.com/user-attachments/assets/44ba753a-c4e6-493e-9eea-98cb6b84e1fa)

- Desks view (from Administrator's POV)
  ![image](https://github.com/user-attachments/assets/d799c3ad-deff-4391-9f3b-2514e6320091)


- Desks view (from Employee's POV - current booking)
  ![image](https://github.com/user-attachments/assets/10347503-bc32-4b8f-8c2d-a697d78bc04c)


