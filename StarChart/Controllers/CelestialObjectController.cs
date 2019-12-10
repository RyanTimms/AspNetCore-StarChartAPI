using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            var satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == celestialObject.Id).ToList();
            celestialObject.Satellites = satellites;

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(o => o.Name.Equals(name)).ToList();

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                var satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = satellites;
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in allObjects)
            {
                var satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = satellites;
            }

            return Ok(allObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var foundObject = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);

            if (foundObject == null)
            {
                return NotFound();
            }

            foundObject.Name = celestialObject.Name;
            foundObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            foundObject.OrbitalPeriod = celestialObject.OrbitalPeriod;

            _context.CelestialObjects.Update(foundObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;
            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var matchingObjects = _context.CelestialObjects.Where(o => o.Id == id).ToList();
            var matchingSatellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == id).ToList();

            if (!matchingObjects.Any() && !matchingSatellites.Any()) return NotFound();

            if (matchingObjects.Any())
            {
                _context.CelestialObjects.RemoveRange(matchingObjects);
                _context.SaveChanges();
            }

            return NoContent();
        }
    }
}
