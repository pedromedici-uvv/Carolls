using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _context.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();
            

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            //TODO: add current user as seller
            auction.Seller = "test";

            _context.Auctions.Add(auction);
            try
            {
                var result = await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
            }

            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, _mapper.Map<AuctionDto>(auction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auction)
        {
            var auctionEntity = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if(auctionEntity == null)
                return NotFound();

            //TODO: check seller = username
            auctionEntity.Item.Make = auction.Make ?? auctionEntity.Item.Make;
            auctionEntity.Item.Model = auction.Model ?? auctionEntity.Item.Model;
            auctionEntity.Item.Color = auction.Color ?? auctionEntity.Item.Color;
            auctionEntity.Item.Mileage = auction.Mileage ?? auctionEntity.Item.Mileage;
            auctionEntity.Item.Year = auction.Year ?? auctionEntity.Item.Year;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch { 

                return BadRequest("Problem saving changes");
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            //TODO: check if seller = username

            _context.Auctions.Remove(auction);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception )
            {

                throw;
            }

            return Ok();
        }
    }
}
