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
    
    public partial class PickPoint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PickPoint()
        {
            this.Order = new HashSet<Order>();
        }
    
        public int Id { get; set; }
        public Nullable<int> IDIndex { get; set; }
        public int IDCity { get; set; }
        public int IdStreet { get; set; }
        public Nullable<int> House { get; set; }
    
        public virtual City City { get; set; }
        public virtual Indexes Indexes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
        public virtual Street Street { get; set; }
    }
}
