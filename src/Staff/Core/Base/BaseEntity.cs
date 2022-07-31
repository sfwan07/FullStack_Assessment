using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staff.Core
{
    public class BaseEntity<TKey>: BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; private set; }
    }
    public class BaseEntity
    {
        public DateTimeOffset CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public Guid? UpdatedBy { get; private set; }
        public short Version { get; private set; }

        public void SetCreate(Guid userId)
        {
            CreatedBy = userId;
            CreatedAt = DateTimeOffset.UtcNow;
            Version = 1;
        }

        public void SetUpdate(Guid userId)
        {
            UpdatedBy = userId;
            UpdatedAt = DateTimeOffset.UtcNow;
            Version += 1;
        }
    }
}
