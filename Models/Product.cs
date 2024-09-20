using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsApiRest.Models
{
    public class Product
    {
        [Key] // Indica que es la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Autoincrementable
        public int Id {get; set;}

        public string Name {get; set;}
        
        public string Description {get; set;}

        public double Price {get; set;}

        public double Stock {get; set;}
    
        public int IdWarehouse { get; set; }// Relación con Bodega

        // Clave foránea de la tabla Bodega
        [ForeignKey("IdWarehouse")]
        public virtual Warehouse? Warehouse { get; set; }
    }
}