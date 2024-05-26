namespace AcmeCorp.Application.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<ContactInfoDTO> ContactInfos { get; set; }
        public List<OrderDTO> Orders { get; set; }
    }
}
