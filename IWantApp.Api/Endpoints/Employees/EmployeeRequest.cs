namespace IWantApp.Api.Endpoints.Employees;

public sealed record EmployeeRequest(string Name, string Email, string Password, string EmployeeCode);