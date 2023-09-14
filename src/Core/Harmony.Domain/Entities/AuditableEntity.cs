﻿using Harmony.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Domain.Entities
{
    /// <summary>
    /// Auditable entities are automatically update their dates
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AuditableEntity<T> : IAuditableEntity, IEntity<T>
    {
        public T Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    public interface IAuditableEntity
    {
        DateTime DateCreated { get; set; }
        DateTime? DateUpdated { get; set; }
    }
}
