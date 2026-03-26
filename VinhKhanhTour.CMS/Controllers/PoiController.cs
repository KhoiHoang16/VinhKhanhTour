using Microsoft.AspNetCore.Mvc;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/poi")]
    public class PoiController : ControllerBase
    {
        private readonly PoiRepository _repo;

        public PoiController(PoiRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pois = await _repo.GetAllPoisAsync();
            return Ok(pois);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Poi poi)
        {
            await _repo.SavePoiAsync(poi);
            return Created($"/api/poi/{poi.Id}", poi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Poi poi)
        {
            poi.Id = id;
            await _repo.SavePoiAsync(poi);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pois = await _repo.GetAllPoisAsync();
            var poi = pois.FirstOrDefault(p => p.Id == id);
            if (poi != null)
            {
                await _repo.DeletePoiAsync(poi);
                return NoContent();
            }
            return NotFound();
        }
    }
}
