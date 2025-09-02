namespace E_CommerceSystem.Models.DTOs
{
    public class CategoryDTO
    {
        public int CID { get; set; }   // match your model naming (like UID, PID, OID)

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
