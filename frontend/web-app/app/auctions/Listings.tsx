"use client";
import { Spinner } from "flowbite-react";
import qs from "query-string";
import { useEffect, useState } from "react";
import { shallow } from "zustand/shallow";
import { useParamsStore } from "../../hooks/useParamsStore";
import { Auction, PageResult } from "../../types";
import { getData } from "../actions/auctionsAction";
import AppPagination from "../components/AppPagination";
import EmptyFilter from "../components/EmptyFilter";
import AuctionCards from "./AuctionCards";
import Filters from "./Filters";

export default function Listings() {
  const [data, setData] = useState<PageResult<Auction>>();
  const params = useParamsStore(
    (state) => ({
      pageNumber: state.pageNumber,
      pageSize: state.pageSize,
      searchTerm: state.searchTerm,
      orderBy: state.orderBy,
      filterBy: state.filterBy,
    }),
    shallow
  );

  const setParams = useParamsStore((state) => state.setParams);
  const url = qs.stringifyUrl({ url: "", query: params });

  function setPageNumber(pageNumber: number) {
    setParams({ pageNumber });
  }

  useEffect(() => {
    getData(url).then((data) => {
      setData(data);
    });
  }, [url]);

  if (!data) {
    return (
      <Spinner
        className="mx-auto mt-20 flex h-full "
        aria-label="Extra large spinner example"
        size="xl"
      />
    );
  }

  return (
    <>
      <Filters />
      {data.totalCount === 0 ? (
        <EmptyFilter showReset />
      ) : (
        <>
          <div className="grid grid-cols-4 gap-6">
            {data.results.map((auction) => (
              <AuctionCards auction={auction} key={auction.id} />
            ))}
          </div>
          <div className="flex justify-center mt-4">
            <AppPagination
              currentPage={params.pageNumber}
              pageCount={data.pageCount}
              pageChanged={setPageNumber}
            />
          </div>{" "}
        </>
      )}
    </>
  );
}
