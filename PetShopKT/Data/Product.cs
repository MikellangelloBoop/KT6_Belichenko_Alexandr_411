//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PetShopKT.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderProduct = new HashSet<OrderProduct>();
        }
    
        public int Id { get; set; }
        public Nullable<int> Idunits { get; set; }
        public int IdProductName { get; set; }
        public int IdProducterName { get; set; }
        public Nullable<int> IdProductManufacturer { get; set; }
        public int IdProductCategory { get; set; }
        public string Article { get; set; }
        public decimal ProductCost { get; set; }
        public byte ProductDiscountAmount { get; set; }
        public int ProductQuantityInStock { get; set; }
        public string Count { get; set; }
        public string ProductDescription { get; set; }
        public string PhotoName { get; set; }
        public byte[] ProductPhoto { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
        public virtual Producter Producter { get; set; }
        public virtual ProductName ProductName { get; set; }
        public virtual Units Units { get; set; }
    }
}