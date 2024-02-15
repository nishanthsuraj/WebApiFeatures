namespace WebApiFeatures.Models
{
    public class ProductQueryParameters : QueryParameters
    {
        #region Public Properties
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string ProductName { get; set; } = string.Empty;
        #endregion
    }
}
