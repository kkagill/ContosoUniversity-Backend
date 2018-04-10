# ContosoUniversity.API

RESTful API using AspNetCore.Identity and OpenID Connect

- ASP.NET Core 2.0 
- Entity Framework Core 2.0
- Code First Migration
- OpenIdConnect.Server
- AspNetCore.Identity 2.0
- EntityFrameworkCore.SqlServer
- Automapper

Referenced [Microsoft](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro) and [chsakell](https://chsakell.com/2016/06/23/rest-apis-using-asp-net-core-and-entity-framework-core/)'s REST API best practice

## Installation

1. Clone or download this project
2. Click `ContosoUniversity.sln` and open with Visual Studio 2017
3. Navigate to Build - Rebuild Solution
4. Navigate to Tools - Extensions and Updates..
5. Click `Online` located on the left side and search for `Open Command Line` and install (restart Visual Studio)
6. Click on `ContosoUniversity.API` project on the Solution Explorer, and press `ALT + SPACE` to open up CLI
7. Run this migration command: `dotnet ef migrations add InitialDatabase -c ContosoContext`
8. Run another command which creates database: `dotnet ef database update -c ContosoContext` 
9. In `Startup.cs`, uncomment `seeder.InitializeData().Wait();` from Configure method 
10. Press F5 to run the project. (Make sure ContosoUniversity.API is set as startup project)

## Best Practice

- Separation of concerns is widely applied so the codes are not tightly coupled. (Ex: repository patterns)
- Used Automapper to prevent from over-posting attacks
- Fluent validation is used
- RESTful HTTP API using JSON as a data format
- JWT(JSON Web Tokens) based security (Authentication, Authorization & API calls)

## Test API

Use Postman or other API testing tools

- Retrieve all instructors:
  
  **GET** `http://localhost:51089/api/instructors`
  
- Access Apis based on a role (Admin or User):

  **POST** `http://localhost:51089/connect/token`
  
  Create Keys & Values in `Body` tab, `x-www-form-urlencoded` section using admin account and copy `access_token` value
  
  ![1](https://user-images.githubusercontent.com/7738916/34234683-6f08fb94-e5a1-11e7-8438-a39736fb1bc6.png)
  
  **GET** `http://localhost:51089/api/admin` where it is decorated with `[Authorize(Roles = "Admin")]` attribute.
  
  Create Key & Value clicking `Headers` tab. Paste the access_token value in the Value textbox: `Bearer eyjhb...`  
  
  ![2](https://user-images.githubusercontent.com/7738916/34234702-83d01242-e5a1-11e7-8d28-736d5d8bda95.png)


  
  
  
    
