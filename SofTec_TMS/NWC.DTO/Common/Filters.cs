namespace NWC.DTO.Common
{
    public class Filters<T>
    {
        public PageFilter PageFilter { get; set; }
        public T SearchKeyword { get; set; }
    }
}
