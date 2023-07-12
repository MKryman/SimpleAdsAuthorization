using Homework_04_19.Data;

namespace Homework_04_19.Web.Models
{
    public class HomePageviewModel
    {
        public List<Ad> Ads { get; set; }
        public UserRepository Repo { get; set; }
    }
}
