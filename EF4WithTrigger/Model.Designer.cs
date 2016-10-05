﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
namespace DumpingCoreMemory.EF4WithTrigger
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class EF4Entities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new EF4Entities object using the connection string found in the 'EF4Entities' section of the application configuration file.
        /// </summary>
        public EF4Entities() : base("name=EF4Entities", "EF4Entities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new EF4Entities object.
        /// </summary>
        public EF4Entities(string connectionString) : base(connectionString, "EF4Entities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new EF4Entities object.
        /// </summary>
        public EF4Entities(EntityConnection connection) : base(connection, "EF4Entities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<TableWithVersion> TableWithVersions
        {
            get
            {
                if ((_TableWithVersions == null))
                {
                    _TableWithVersions = base.CreateObjectSet<TableWithVersion>("TableWithVersions");
                }
                return _TableWithVersions;
            }
        }
        private ObjectSet<TableWithVersion> _TableWithVersions;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the TableWithVersions EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToTableWithVersions(TableWithVersion tableWithVersion)
        {
            base.AddObject("TableWithVersions", tableWithVersion);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="DumpingCoreMemory.EF4Model", Name="TableWithVersion")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class TableWithVersion : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new TableWithVersion object.
        /// </summary>
        /// <param name="identifier">Initial value of the Identifier property.</param>
        /// <param name="type">Initial value of the Type property.</param>
        public static TableWithVersion CreateTableWithVersion(global::System.Guid identifier, global::System.Int32 type)
        {
            TableWithVersion tableWithVersion = new TableWithVersion();
            tableWithVersion.Identifier = identifier;
            tableWithVersion.Type = type;
            return tableWithVersion;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Guid Identifier
        {
            get
            {
                return _Identifier;
            }
            set
            {
                if (_Identifier != value)
                {
                    OnIdentifierChanging(value);
                    ReportPropertyChanging("Identifier");
                    _Identifier = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Identifier");
                    OnIdentifierChanged();
                }
            }
        }
        private global::System.Guid _Identifier;
        partial void OnIdentifierChanging(global::System.Guid value);
        partial void OnIdentifierChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Type
        {
            get
            {
                return _Type;
            }
            set
            {
                OnTypeChanging(value);
                ReportPropertyChanging("Type");
                _Type = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Type");
                OnTypeChanged();
            }
        }
        private global::System.Int32 _Type;
        partial void OnTypeChanging(global::System.Int32 value);
        partial void OnTypeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> Version
        {
            get
            {
                return _Version;
            }
            set
            {
                OnVersionChanging(value);
                ReportPropertyChanging("Version");
                _Version = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Version");
                OnVersionChanged();
            }
        }
        private Nullable<global::System.Int32> _Version;
        partial void OnVersionChanging(Nullable<global::System.Int32> value);
        partial void OnVersionChanged();

        #endregion

    
    }

    #endregion

    
}
