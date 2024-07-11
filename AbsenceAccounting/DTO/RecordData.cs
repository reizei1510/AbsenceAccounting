namespace AbsenceAccounting.DTO
{
    // модель для передачи данных о записи
    public class RecordData
    {
        public int Id { get; set; }
        public string Employee { get; set; }
        public string Reason { get; set; }
        public string Start { get; set; }
        public int Duration { get; set; }
        public bool Taken { get; set; }
        public string Comment { get; set; }
    }
}
