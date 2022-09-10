﻿namespace Domain.Common
{
   
    public abstract class BaseEntity
    {
        public virtual int Id { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? Deleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
