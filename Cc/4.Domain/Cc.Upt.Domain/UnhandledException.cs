using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cc.Upt.Domain
{
    [Table("UnhandledException", Schema = "dbo")]
    public class UnhandledException
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Thread { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Stack { get; set; }
        public string ExceptionType { get; set; }

        public UnhandledException()
        {
            Id = Guid.NewGuid();
        }
    }
}