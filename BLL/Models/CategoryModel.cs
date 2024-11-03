#nullable disable

using BLL.DAL;
using EZcore.Attributes;
using EZcore.Models;


namespace BLL.Models
{
    public class CategoryModel : Model<Category>
    {
        [DisplayName("Name", "Adı")]
        public string Name => Record.Name;

        [DisplayName("Description", "Açıklaması")]
        public string Description => Record.Description;
    }
}
