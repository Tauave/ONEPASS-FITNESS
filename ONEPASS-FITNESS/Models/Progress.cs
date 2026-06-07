namespace ONEPASS_FITNESS.Models
{
    public class Progress
    {
        public int ProgressId { get; set; }

        public int PersonalinfoId { get; set; }

        public decimal Weight { get; set; }

        public DateOnly DateRecorded { get; set; }

        public Personalinfo Personalinfo { get; set; }
    }
}
