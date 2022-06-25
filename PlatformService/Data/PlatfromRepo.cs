using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlatformService.Data
{
    public class PlatfromRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatfromRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreatePlatform(Platform platform)
        {
            if(platform == null)
            {
                throw new ArgumentException("Null platform");
            }

            _context.Add(platform);
        }

        public Platform GetPlatformById(int id) => _context.Platforms.FirstOrDefault(p => p.Id == id);

        public IEnumerable<Platform> GetPlatforms() => _context.Platforms.ToList();

        public bool SaveChanges() => (_context.SaveChanges() >= 0);
    }
}
