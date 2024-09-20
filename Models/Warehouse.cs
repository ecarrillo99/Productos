using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsApiRest.Models
{
    public class Warehouse
    {
        [Key] // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Autoincrementable
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}