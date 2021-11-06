namespace AIDMusicApp.Models
{
    public class Member
    {
        public int Id { get; set; }

        public int MusicianId { get; set; }

        public int GroupId { get; set; }

        public bool IsFormer { get; set; }
    }
}
