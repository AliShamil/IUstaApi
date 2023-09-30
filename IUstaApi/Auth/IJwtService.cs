﻿using System.Security.Claims;

namespace IUstaApi.Auth;

public interface IJwtService
{
    string GenerateSecurityToken(string id, string email, IEnumerable<string> roles, IEnumerable<Claim> userClaims);
}
