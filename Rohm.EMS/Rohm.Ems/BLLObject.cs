using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Rohm.Ems
{
    [Serializable()]
    public abstract class BLLObject
    {
        public enum EntityStateType
        {
            Unchanged,
            Added,
            Deleted,
            Modified
        }

        private EntityStateType m_EntityState;
                
        public EntityStateType EntityState
        {
            get { return m_EntityState; }
            set { m_EntityState = value; }
        }

        [XmlIgnore()]
        public virtual bool IsDirty
        {
            get { return m_EntityState != EntityStateType.Unchanged; }
        }

        protected internal void SetEntityState(EntityStateType newEntityState)
        {
            switch (newEntityState)
            {
                case EntityStateType.Deleted:
                case EntityStateType.Unchanged:
                case EntityStateType.Added:
                    this.m_EntityState = newEntityState;
                    break;
                default:
                    if (this.EntityState == EntityStateType.Unchanged)
                        this.m_EntityState = newEntityState;
                    break;
            }
        }

        public void SetAdded() {
            m_EntityState = EntityStateType.Added;
        }

        public void SetModified() {
            m_EntityState = EntityStateType.Modified;
        }

        public void SetUnchanged()
        {
            m_EntityState = EntityStateType.Unchanged;
        }

        public void SetDeleted() {
            m_EntityState = EntityStateType.Deleted;
        }

    }
}
