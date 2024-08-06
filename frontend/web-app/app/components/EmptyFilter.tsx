import { Button } from "flowbite-react";
import { useParamsStore } from "../../hooks/useParamsStore";
import Heading from "./Heading";

type Props = {
  title?: string;
  subtitle?: string;
  showReset?: boolean;
};
export default function EmptyFilter({
  title = "no matches for this filter",
  subtitle = "try changing or reseting the filter",
  showReset,
}: Props) {
  const reset = useParamsStore((state) => state.reset);

  return (
    <div className="w-full flex flex-col gap-2 justify-center items-center shadow-lg py-40">
      <Heading title={title} subtitle={subtitle} center />
      <div className="mt-4">
        {showReset && (
          <Button outline onClick={reset}>
            Reset Filters
          </Button>
        )}
      </div>
    </div>
  );
}
