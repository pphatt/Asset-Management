import { classNames } from "@/libs/classNames";
import { QueryConfig } from "@/pages/Assignment/Assignment";
import { createSearchParams, Link } from "react-router-dom";

interface Props {
  queryConfig: QueryConfig;
  totalPage: number;
  pathName: string;
}
const RANGE = 2;
export default function AssignmnetPagination({
  queryConfig,
  totalPage,
  pathName,
}: Props) {
  const page = Number(queryConfig.pageNumber);
  console.log(page, totalPage);
  const rederPagination = () => {
    let dotAfter = false;
    let dotBefore = false;

    const renderDotBefore = (index: number) => {
      if (!dotBefore) {
        dotBefore = true;
        return (
          <span key={index} className="mx-1 px-2 py-1 text-gray-600 border">
            ...
          </span>
        );
      }
      return null;
    };
    const renderDotAfter = (index: number) => {
      if (!dotAfter) {
        dotAfter = true;
        return (
          <span key={index} className="mx-1 px-2 py-1 text-gray-600 border">
            ...
          </span>
        );
      }
      return null;
    };
    return Array(totalPage)
      .fill(0)
      .map((_, index) => {
        const pageNumber = index + 1;
        if (
          page <= RANGE * 2 + 1 &&
          pageNumber > page + RANGE &&
          pageNumber < totalPage - RANGE + 1
        ) {
          //Trường Hợp ... chỉ xuất hiện duy nhất Ở sau
          //Page nó nằm ở khúc đầu
          return renderDotAfter(index);
        } else if (page > RANGE * 2 + 1 && page < totalPage - RANGE * 2) {
          //Page nó nằm ở khúc giữa
          if (pageNumber < page - RANGE && pageNumber > RANGE) {
            return renderDotBefore(index);
          } else if (
            pageNumber > page + RANGE &&
            pageNumber < totalPage - RANGE + 1
          ) {
            return renderDotAfter(index);
          }
        } else if (
          page >= totalPage - RANGE * 2 &&
          pageNumber > RANGE &&
          pageNumber < page - RANGE
        ) {
          //Trường Hợp ... chỉ xuất hiện duy nhất Ở đầu
          //Page nó nằm ở khúc cuối
          return renderDotBefore(index);
        }
        return (
          <Link
            to={{
              pathname: pathName,
              search: createSearchParams({
                ...queryConfig,
                pageNumber: pageNumber.toString(),
              }).toString(),
            }}
            key={index}
            className={classNames("mx-1 px-3 py-1 text-sm border", {
              "bg-primary text-white font-medium rounded border-primary":
                pageNumber === page,
              "text-gray-600 hover:text-primary rounded": pageNumber !== page,
            })}
          >
            {pageNumber}
          </Link>
        );
      });
  };
  return (
    <div className="flex flex-wrap mt-6 justify-end items-center">
      {page === 1 ? (
        <span className="text-gray-400 cursor-not-allowed mx-2 border px-2 py-1 rounded">
          Previous
        </span>
      ) : (
        <Link
          to={{
            pathname: pathName,
            search: createSearchParams({
              ...queryConfig,
              pageNumber: (page - 1).toString(),
            }).toString(),
          }}
          className="text-gray-600 hover:text-primary mx-2 border px-2 py-1 rounded"
        >
          Previous
        </Link>
      )}
      {rederPagination()}
      {page === totalPage ? (
        <span className="text-gray-400 cursor-not-allowed mx-2 border px-2 py-1 rounded">
          Next
        </span>
      ) : (
        <Link
          to={{
            pathname: pathName,
            search: createSearchParams({
              ...queryConfig,
              pageNumber: (page + 1).toString(),
            }).toString(),
          }}
          className="text-gray-600 hover:text-primary mx-2 border px-2 py-1 rounded"
        >
          Next
        </Link>
      )}
    </div>
  );
}
