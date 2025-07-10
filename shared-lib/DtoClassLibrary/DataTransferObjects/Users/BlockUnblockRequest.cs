namespace DataTransferLib.DataTransferObjects.Users
{
    public class BlockUnblockRequest
    {
        public string UserId { get; set; }
        public string PerformedById { get; set; }
        public string Reason { get; set; }
    }
}
