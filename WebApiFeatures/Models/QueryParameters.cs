namespace WebApiFeatures.Models
{
    public class QueryParameters
    {
        #region Private Variables
        private const int MaxSize = 20;
        private const int DefaultPageNumber = 1;
        private int _size = 10;
        private string _sortOrder = "asc";
        #endregion

        #region Public Properties
        public int Page { get; set; } = DefaultPageNumber;
        public int Size
        {

            get { return _size; }
            set { _size = Math.Min(MaxSize, value); }
        }

        public string SortBy { get; set; } = "Id";
        public string SortOrder
        {
            get { return _sortOrder; }
            set
            {
                if(value == "asc" || value == "desc")
                    _sortOrder = value;
            }
        }
        #endregion
    }
}
