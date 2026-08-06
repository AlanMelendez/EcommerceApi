using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Common.Models
{
    // We need a standart object to return paginated data.
    public sealed class PagedResult<T>
    {
        private PagedResult(
            IReadOnlyList<T> items,
            int pageNumber,
            int pageSize,
            int totalCount,
            int totalPages)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }

        public IReadOnlyList<T> Items { get; }

        public int PageNumber { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public int TotalPages { get; }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public static PagedResult<T> Create(
            IReadOnlyList<T> items,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            //Total count is the total number of items in the database, pageSize is the number of items per page, and pageNumber is the current page number.
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResult<T>(
                items,
                pageNumber,
                pageSize,
                totalCount,
                totalPages);
        }
    }
}
