using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data.Context;

public class AppDbContext(DbContextOptions options) 
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options);
