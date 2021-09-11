namespace DownloadBoard.Models
{
    public class Webinar
    {
        public Webinar(string id, 
            string webName,
            string status, 
            string dateStart, 
            string dateEnd,
            string link)
        {
            Id = id;
            WebName = webName;
            Status = status;
            DateStart = dateStart;
            DateEnd = dateEnd;
            Link = link;
        }

        public string Id { get; set; }
        public string WebName { get; set; }
        public string Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string Link { get; set; }
    }
}