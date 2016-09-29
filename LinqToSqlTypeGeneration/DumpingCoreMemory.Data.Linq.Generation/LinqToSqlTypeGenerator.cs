/**
* MIT License
*
* Copyright (c) 2016 Derek Goslin < http://corememorydump.blogspot.ie/ >
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace DumpingCoreMemory.Data.Linq.Generation
{
    /// <summary>
    /// An implementation of the <see cref="ITypeGenerator"/> inteface
    /// </summary>
    public class LinqToSqlTypeGenerator : ITypeGenerator
    {
        /// <summary>
        /// Generate a set of types corresponding to the metadata in the <paramref name="entityDescriptors"/>
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to generate the types into</param>
        /// <param name="entityDescriptors">The metadata that describes the types to be generated</param>
        /// <param name="generatedTypes">The generated Linq To Sql Types</param>
        public void GenerateTypes(String assemblyName, IList<EntityDescriptor> entityDescriptors, out IList<Type> generatedTypes)
        {
            GenerateTypes(assemblyName, String.Empty, entityDescriptors, out generatedTypes);
        }

        /// <summary>
        /// Generate a set of types corresponding to the metadata in the <paramref name="entityDescriptors"/>
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to generate the types into</param>
        /// <param name="assemblyfileName">The file to store the generated assembly too</param>
        /// <param name="entityDescriptors">The metadata that describes the types to be generated</param>
        /// <param name="generatedTypes">The generated Linq To Sql Types</param>
        public void GenerateTypes(String assemblyName, String assemblyfileName, IList<EntityDescriptor> entityDescriptors, out IList<Type> generatedTypes)
        {
            generatedTypes = new List<Type>();

            AssemblyBuilder assemblyBuilder;
            ModuleBuilder moduleBuilder;

            CreateDynamicAssembly(assemblyName, assemblyfileName, out assemblyBuilder, out moduleBuilder);

            foreach (var entity in entityDescriptors)
            {
                generatedTypes.Add(CreateLinqToSqlType(assemblyName, moduleBuilder, entity));
            }

            if(assemblyName != String.Empty)
            {
                assemblyBuilder.Save(assemblyfileName);
            }
        }

        /// <summary>
        /// Creates an assembly on the fly
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <param name="assemblyfileName"></param>
        /// <param name="assemblyBuilder">The assembly builder that module and types will be added too</param>
        /// <param name="moduleBuilder">The module that types will be defined in</param>
        private void CreateDynamicAssembly(String assemblyName, String assemblyfileName, out AssemblyBuilder assemblyBuilder, out ModuleBuilder moduleBuilder)
        {
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            
            if(assemblyfileName == String.Empty)
            {
                moduleBuilder = assemblyBuilder.DefineDynamicModule("module");    
            }
            else
            {
                moduleBuilder = assemblyBuilder.DefineDynamicModule("module", assemblyfileName);
            }
        }

        /// <summary>
        /// Generates a Linq to SQL type based on the <paramref name="entityDescriptor"/>
        /// </summary>
        /// <param name="assemblyName">The assembly name</param>
        /// <param name="moduleBuilder">The module builder into which types will be generated</param>
        /// <param name="entityDescriptor">The description of the type to create</param>
        /// <returns>A concrete type that represents the soecified <paramref name="entityDescriptor"/></returns>
        private Type CreateLinqToSqlType(String assemblyName, ModuleBuilder moduleBuilder, EntityDescriptor entityDescriptor)
        {
            ILGenerator ilgen;

            MethodAttributes getSetAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
            TypeBuilder typeBuilder = moduleBuilder.DefineType(assemblyName + "." + entityDescriptor.EntityName, TypeAttributes.Class | TypeAttributes.Public, null, new Type[] { entityDescriptor.BaseType });
            ConstructorBuilder ctorBuilder = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.HideBySig);

            Type columnAttributeType = typeof(ColumnAttribute);
            PropertyInfo[] columnAttributeProperties = columnAttributeType.GetProperties().Where(x => x.Name == "Name" || x.Name == "IsPrimaryKey").ToArray();
            ConstructorInfo columnAttributeconstructor = columnAttributeType.GetConstructor(Type.EmptyTypes);

            Type tableAttributeType = typeof(TableAttribute);
            PropertyInfo[] tableAttributeProperties = tableAttributeType.GetProperties().Where(x => x.Name == "Name").ToArray();
            ConstructorInfo tableAttributeconstructor = tableAttributeType.GetConstructor(Type.EmptyTypes);
            CustomAttributeBuilder tableAttributeBuilder = new CustomAttributeBuilder(tableAttributeconstructor, new object[] { }, tableAttributeProperties,
                 new object[] { entityDescriptor.TableName });
            typeBuilder.SetCustomAttribute(tableAttributeBuilder);

            foreach (var property in entityDescriptor.EntityProperties)
            {
                if (property.IsIdentity)
                {
                    //the order is important public final hidebysig specialname newslot virtual
                    getSetAttributes = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
                }
                else
                {
                    getSetAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
                }

                FieldBuilder field = typeBuilder.DefineField(String.Format("m_{0}", property.Name), property.Type, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, System.Reflection.PropertyAttributes.HasDefault, property.Type, null);
                MethodBuilder getFieldMethodBuilder = typeBuilder.DefineMethod(String.Format("get_{0}", property.Name), getSetAttributes, property.Type, Type.EmptyTypes);

                ilgen = getFieldMethodBuilder.GetILGenerator();
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldfld, field);
                ilgen.Emit(OpCodes.Ret);

                MethodBuilder setFieldMethodBuilder = typeBuilder.DefineMethod(String.Format("set_{0}", property.Name), getSetAttributes, null, new Type[] { property.Type });
                setFieldMethodBuilder.DefineParameter(1, ParameterAttributes.None, "value");

                ilgen = setFieldMethodBuilder.GetILGenerator();
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldarg_1);
                ilgen.Emit(OpCodes.Stfld, field);
                ilgen.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getFieldMethodBuilder);
                propertyBuilder.SetSetMethod(setFieldMethodBuilder);

                CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(columnAttributeconstructor, new object[] { }, columnAttributeProperties,
                new object[] { property.IsIdentity, property.ColumnName });

                propertyBuilder.SetCustomAttribute(daBuilder);
            }

            Type t = typeBuilder.CreateType();

            return t;
        }        
    }
}
