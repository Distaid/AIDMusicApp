namespace AIDMusicApp.Models
{
    public class Album
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public short Year { get; set; }

        public byte[] Cover { get; set; }
    }
}
