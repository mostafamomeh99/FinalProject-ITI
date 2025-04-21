namespace finalProject.Helper
{
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 6;

        private const int MaxPageSize = 50;
        public int ValidatedPageSize => (PageSize > MaxPageSize) ? MaxPageSize : PageSize;
    }
}
