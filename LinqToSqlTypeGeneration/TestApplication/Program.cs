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
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using DumpingCoreMemory.Data.Linq.Generation;

namespace TestApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var entityDescriptors = GenerateEntityDescriptors();
                var generator = new LinqToSqlTypeGenerator();
                IList<Type> generatedTypes;

#warning Ensure that the process that invokes this method has rights to write to the disk
                // Passing the assemblyFileName parameter is not necessary. 
                // The generator will write the assembly to the specified filename.
                // This allows us to reuse the generated types or just take a look at them with ILSpy
                generator.GenerateTypes("LinqToSqlGeneratedTypes", "LinqToSqlGeneratedTypesAssembly.dll",
                                        entityDescriptors,
                                        out generatedTypes);

                object instance = Activator.CreateInstance(generatedTypes[0]);

                var instanceAsIEntity = (IEntity) instance;
                dynamic o = instance;

                Console.WriteLine("Accessed as interface: {0}", instanceAsIEntity.Identity);
                Console.WriteLine("Accessed as dynamic: {0}", o.Identity);
                Console.WriteLine("Accessed by reflection: {0}", GetPropertyValueByReflection(instance, "Identity"));

                o.Identity = 1;
                o.PropertyA = "A test";
                o.PropertyB = Int32.MaxValue;
                              
                PersistEntity(instance);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Occured: {0}", e);
            }
        }

        private static void PersistEntity(object instance)
        {
            using (var sqlConnection = new SqlConnection("Server=localhost;Database=Test;Trusted_Connection=True;"))
            {
                using (var context = new DataContext(sqlConnection))
                {
                    ITable table = context.GetTable(instance.GetType());

                    table.InsertOnSubmit(instance);

                    context.SubmitChanges();
                }    
            }
        }

        private static object GetPropertyValueByReflection(Object instance, String propertyName)
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName);

            return propertyInfo.GetValue(instance, null);
        }

        private static List<EntityDescriptor> GenerateEntityDescriptors()
        {
            var entityDescriptors = new List<EntityDescriptor>
                                        {
                                            new EntityDescriptor
                                                {
                                                    BaseType = typeof (IEntity),
                                                    EntityName = "EntityA",
                                                    TableName = "TableA",
                                                    EntityProperties = new List<EntityPropertyDescriptor>
                                                                           {
                                                                               new EntityPropertyDescriptor
                                                                                   {
                                                                                       ColumnName = "Id",
                                                                                       Name = "Identity",
                                                                                       Type = typeof (Int32),
                                                                                       IsIdentity = true
                                                                                   },
                                                                               new EntityPropertyDescriptor
                                                                                   {
                                                                                       ColumnName = "ColumnA",
                                                                                       Name = "PropertyA",
                                                                                       Type = typeof (String)
                                                                                   },
                                                                               new EntityPropertyDescriptor
                                                                                   {
                                                                                       ColumnName = "ColumnB",
                                                                                       Name = "PropertyB",
                                                                                       Type = typeof (Int32)
                                                                                   }
                                                                           },
                                                }
                                        };
            return entityDescriptors;
        }
    }
}