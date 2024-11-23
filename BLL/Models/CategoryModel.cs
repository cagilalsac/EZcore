#nullable disable

using BLL.DAL;
using EZcore.Attributes;
using EZcore.Models;


namespace BLL.Models
{
    public class CategoryModel : Model<Category>
    {
        [DisplayName("Adı", "Name")]
        public string Name => Record.Name;

        [DisplayName("Açıklaması", "Description")]
        public string Description => Record.Description;
    }
}
