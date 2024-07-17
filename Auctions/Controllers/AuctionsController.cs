using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
        {
            var query = _context.Auctions
                .OrderBy(x => x.Item.Make)
                .AsQueryable();

            if(!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0 );
            }

            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
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

            var newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
            
            
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to DB");

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

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auctionEntity));

           
            var results = await _context.SaveChangesAsync() > 0;

            if (results)
                return Ok();

            return BadRequest();
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
                await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString()});
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
