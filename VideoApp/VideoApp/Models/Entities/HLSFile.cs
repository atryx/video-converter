using System.ComponentModel.DataAnnotations;

namespace VideoApp.Web.Models.Entities
{
    public class HLSFile
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public int ParentVideoId { get; set; }
        public HLSType HLSType { get; set; }
        public virtual VideoFile ParentVideoFile { get; set; }
    }

    public enum HLSType
    {
        Playlist,
        PartialVideo
    }
}
