using System.Net;

namespace InvestmentControl.Domain.Models;

public sealed record ApiErrorResponse(HttpStatusCode HttpStatusCode, string Description);
