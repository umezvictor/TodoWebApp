namespace Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string CustGroupCode { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string LGACode { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string MaidenName { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;



        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
