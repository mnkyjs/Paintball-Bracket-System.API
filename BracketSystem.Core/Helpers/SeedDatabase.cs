using BracketSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BracketSystem.Core.Helpers
{
    public class SeedDatabase
    {
        private readonly IConfiguration _config;
        private readonly BracketContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public SeedDatabase(BracketContext context, UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public void SeedLocation()
        {
            // Look for any locations.
            if (_context.Locations.Any()) return; // DB has been seeded

            _context.Locations.AddRange(
                new Location
                {
                    Name = "Baden-Württemberg"
                },
                new Location
                {
                    Name = "Bayern"
                },
                new Location
                {
                    Name = "Berlin"
                },
                new Location
                {
                    Name = "Brandenburg"
                },
                new Location
                {
                    Name = "Bremen"
                },
                new Location
                {
                    Name = "Hamburg"
                },
                new Location
                {
                    Name = "Hessen"
                },
                new Location
                {
                    Name = "Mecklenburg-Vorpommern"
                },
                new Location
                {
                    Name = "Niedersachsen"
                },
                new Location
                {
                    Name = "Nordrhein-Westfalen"
                },
                new Location
                {
                    Name = "Rheinland-Pfalz"
                },
                new Location
                {
                    Name = "Saarland"
                },
                new Location
                {
                    Name = "Baden-Württemberg"
                },
                new Location
                {
                    Name = "Sachsen"
                },
                new Location
                {
                    Name = "Sachsen-Anhalt"
                },
                new Location
                {
                    Name = "Schleswig-Holstein"
                },
                new Location
                {
                    Name = "Thüringen"
                }
            );
            _context.SaveChanges();
        }

        public void SeedPaintballField()
        {
            // Look for any fields.
            if (_context.Paintballfields.Any()) return; // DB has been seeded

            _context.Paintballfields.AddRange(
                new Paintballfield
                {
                    Name = "Paintball Battlefields Hildesheim",
                    Street = "Lerchenkamp",
                    HouseNumber = "60",
                    PostalCode = 31137,
                    Place = "Hildesheim",
                    PhoneNumber = "+4951216904922",
                    Location = _context.Locations.FirstOrDefault(x => x.Name == "Niedersachsen"),
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                },
                new Paintballfield
                {
                    Name = "Paintball Base Hessen",
                    Street = "Laubacher Weg",
                    HouseNumber = "26-28",
                    PostalCode = 35606,
                    Place = "Solms-Albshausen",
                    PhoneNumber = "+4951216904922",
                    Location = _context.Locations.FirstOrDefault(x => x.Name == "Hessen"),
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                }
            );
            _context.SaveChanges();
        }

        public void SeedRole()
        {
            // Look for any roles.
            if (_roleManager.Roles.Any()) return; // DB has been seeded

            var roles = new List<Role>
            {
                new Role {Name = "Member"},
                new Role {Name = "Admin"},
                new Role {Name = "RootAdmin"},
                new Role {Name = "Moderator"},
            };

            foreach (var role in roles)
            {
                _roleManager.CreateAsync(role).Wait();
            }
        }

        public void SeedTeams()
        {
            // Look for any teams.
            if (_context.Teams.Any()) return; // DB has been seeded

            _context.Teams.AddRange(
                new Team
                {
                    Name = "Team Braindead",
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                },
                new Team
                {
                    Name = "Ballistic Göttingen",
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                },
                new Team
                {
                    Name = "Hannover Painthers",
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                },
                new Team
                {
                    Name = "Unbreakable Hannover",
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                },
                new Team
                {
                    Name = "pause",
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                },
                new Team
                {
                    Name = "Team Ghostbusters",
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                },
                new Team
                {
                    Name = "Rio Bravo",
                    Creator = _context.Users.FirstOrDefault(i => i.UserName == _config["User:Name"])
                }
            );
            _context.SaveChanges();
        }

        public void SeedUsers()
        {
            // Look for any users.
            // if (_userManager.Users.Any()) return; // DB has been seeded

            var adminUser = new User
            {
                UserName = _config["User:Name"],
                Created = DateTime.Now,
                // RoleId = (int) Role.RootAdmin,
                TeamName = "Team Braindead",
            };

            var adminAccount = _userManager.CreateAsync(adminUser, _config["User:Password"]).Result;

            if (!adminAccount.Succeeded) return;

            var admin = _userManager.FindByNameAsync(_config["User:Name"]).Result;
            _userManager.AddToRolesAsync(admin, new[] { "Member", "Admin", "RootAdmin", "Moderator" });
            Log.Information("Admin account created!");
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}