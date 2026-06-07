using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Models.ViewModels
{
    public class AccountViewModel
    {
        public Personalinfo Profile { get; set; }
        public IList<ClassBookings> Bookings { get; set; } = new List<ClassBookings>();
    }
}
