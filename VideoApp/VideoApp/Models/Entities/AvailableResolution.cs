using System.ComponentModel.DataAnnotations;

namespace VideoApp.Web.Models
{
    public class AvailableResolution
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Resolution { get; set; }
        public Category Category { get; set; }
    }

    public enum Category
    {
        EDTV,
        HDTV,
        UHDTV
    }    
}
