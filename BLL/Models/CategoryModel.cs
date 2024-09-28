#nullable disable

using BLL.DAL;
using EZcore.Models;

namespace BLL.Models
{
    public class CategoryModel : Model<Category>
    {
        public string Name => Record.Name;

        public string Description => Record.Description;
    }
}
