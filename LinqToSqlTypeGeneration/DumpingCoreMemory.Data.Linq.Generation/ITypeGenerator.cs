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

namespace DumpingCoreMemory.Data.Linq.Generation
{
    /// <summary>
    /// </summary>
    public interface ITypeGenerator
    {
        /// <summary>
        /// Generate a set of types corresponding to the metadata in the <paramref name="entityDescriptors"/>
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to generate the types into</param>
        /// <param name="entityDescriptors">The metadata that describes the types to be generated</param>
        /// <param name="generatedTypes">The generated Types</param>
        /// <returns>The <see cref="AssemblyBuilder"/> that contains the generated types</returns>
        void GenerateTypes(String assemblyName, IList<EntityDescriptor> entityDescriptors, out IList<Type> generatedTypes);

        /// <summary>
        /// Generate a set of types corresponding to the metadata in the <paramref name="entityDescriptors"/>
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to generate the types into</param>
        /// <param name="assemblyfileName">The file to store the generated assembly too</param>
        /// <param name="entityDescriptors">The metadata that describes the types to be generated</param>
        /// <param name="generatedTypes">The generated Types</param>
        /// <returns>The <see cref="AssemblyBuilder"/> that contains the generated types</returns>
        void GenerateTypes(String assemblyName, String assemblyfileName, IList<EntityDescriptor> entityDescriptors, out IList<Type> generatedTypes);
    }
}
