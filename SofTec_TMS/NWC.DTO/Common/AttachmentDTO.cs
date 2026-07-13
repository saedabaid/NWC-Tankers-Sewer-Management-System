namespace NWC.DTO.Common
{
    public class AttachmentDTO
    {
        public long ID { get; set; }
        public string DocumentName { get; set; }
        public string RelativePath { get; set; }
        public bool IsDeleted { get; set; }
    }
}
