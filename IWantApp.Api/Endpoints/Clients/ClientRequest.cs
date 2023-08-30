namespace IWantApp.Api.Endpoints.Clients;

public sealed record ClientRequest(string Email, string Password, string Name, string Cpf);