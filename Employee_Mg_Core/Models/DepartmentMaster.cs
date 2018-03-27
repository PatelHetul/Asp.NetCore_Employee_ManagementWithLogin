using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Employee_Mg_Core.Models
{
    public class DepartmentMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DepartmentMaster()
        {
            this.EmployeeMaster = new HashSet<EmployeeMaster>();
        }
        [Key]
        public int Department_Id { get; set; }
        [Display(Name = "Department Name")]
        [Required(ErrorMessage = "Please Enter Department Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$")]
        [StringLength(40, MinimumLength = 2)]
        public string Department_Name { get; set; }
        public Nullable<int> IsDelete { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
       public virtual ICollection<EmployeeMaster> EmployeeMaster { get; set; }

        //public virtual List<EmployeeMaster> EmployeeMaster { get; set; }

    }
}
