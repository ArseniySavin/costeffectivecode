﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CostEffectiveCode.Domain.Ddd.Entities
{
    [PublicAPI]
    public class EntityEqualityComparer : IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
                return false;

            var entity = x as IEntity;
            if (entity != null && y is IEntity)
                return entity.GetId() == ((IEntity)y).GetId();

            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            return ((IEntity)obj).GetId().GetHashCode();
        }
    }

    [PublicAPI]
    public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity>
        where TEntity : class, IEntity
    {
        public bool Equals(TEntity x, TEntity y)
        {
            if (x == null || y == null)
                return false;

            return x.GetId() == y.GetId();
        }

        public int GetHashCode(TEntity obj)
        {
            return obj.GetId().GetHashCode();
        }
    }
}
