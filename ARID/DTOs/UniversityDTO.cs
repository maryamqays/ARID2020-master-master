namespace ARID.DTOs
{
    public class UniversityDTO
    {
        public int Id { get; set; }
        public string ArUniversityName { get; set; }
        public string EnUniversityName { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string LogoHD { get; set; }
        public int YearofEstablishment { get; set; }
        public bool Governmental { get; set; }
        public bool Private { get; set; }
        public bool SemiPrivate { get; set; }
    }
}