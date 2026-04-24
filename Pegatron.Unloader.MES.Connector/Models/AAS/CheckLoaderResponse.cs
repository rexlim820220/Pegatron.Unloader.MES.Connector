namespace Models.AAS
{
    public class CheckLoaderResponse : AasBaseResponse
    {
        public string Lot { get; set; }
        public string Part { get; set; }
        public string Detail { get; set; }
    }
}