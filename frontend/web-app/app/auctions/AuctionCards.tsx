import { Auction } from "../../types";
import CardImage from "./CardImage";
import CountDownTimer from "./CountDownTimer";

interface AuctionCardsProps {
  auction: Auction;
}
export default function AuctionCards({ auction }: AuctionCardsProps) {
  return (
    <a href="#" className="group">
      <div className="w-full bg-gray-200 rounded-lg overflow-hidden relative">
        <div>
          <CardImage imageUrl={auction.imageUrl}></CardImage>
          <div className="absolute bottom-2 left-2">
            <CountDownTimer auctionEnd={auction.auctionEnd} />
          </div>
        </div>
      </div>
      <div className="flex justify-between items-center mt-4">
        <h3 className="text-gray-700">
          {auction.make} {auction.model}
        </h3>
        <p className="font-semibold text-sm">{auction.year}</p>
      </div>
    </a>
  );
}
