using System;

namespace OpenNet.Orm
{
    public class StoreAlreadyExistsException : Exception
    {
        public StoreAlreadyExistsException()
            : base("Selected store already exists")
        {
        }
    }

    public class ReservedWordException : Exception
    {
        public ReservedWordException(string word)
            : base(string.Format("'{0}' is a reserved word.  It cannot be used for an Entity or Field name. Rename the entity/field or adjust its attributes.", word))
        {
        }
    }

    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Type type)
            : base(string.Format("Entity Type '{0}' not found. Is your Store up to date?", type.Name))
        {
        }

        public EntityNotFoundException(string entityName)
            : base(string.Format("Entity Type '{0}' not found. Is your Store up to date?", entityName))
        {
        }
    }

    public class PrimaryKeyRequiredException : Exception
    {
        public PrimaryKeyRequiredException(string message)
            : base(message)
        {
        }
    }

    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException(string message)
            : base(message)
        {
        }
    }

    public class DefinitionException : Exception
    {
        public DefinitionException(string message)
            : base(message) { }
    }

    public class EntityDefinitionException : Exception
    {
        public EntityDefinitionException(string message)
            : base(message)
        {
        }
    }
}
