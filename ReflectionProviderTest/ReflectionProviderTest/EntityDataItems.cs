﻿/**
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

using System.Collections.Generic;
using System.Linq;

using BaseEntityTypes;
using DerivedEntityTypes;

namespace ReflectionProviderTest
{
    public class EntityDataItems
    {
        private static List<DerivedTypeA> _derivedA;
        private static List<DerivedTypeB> _derivedB;

        static EntityDataItems()
        {
            _derivedA = new List<DerivedTypeA>();
            _derivedB = new List<DerivedTypeB>();

            for (int i = 0; i < 5; i++)
            {
                _derivedA.Add(new DerivedTypeA() { Id = i, Name = i.ToString() });
                _derivedB.Add(new DerivedTypeB() { Id = i, Name = i.ToString() });
            }
        }
    
        public IQueryable<BaseEntityTypeA> DerivedA
        {
            get { return _derivedA.AsQueryable<DerivedTypeA>(); }
        }

        public IQueryable<BaseEntityTypeB> DerivedB
        {
            get { return _derivedB.AsQueryable<DerivedTypeB>(); }
        }
    }
}
