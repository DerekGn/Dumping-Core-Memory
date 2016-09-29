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

#pragma once

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::Remoting::Messaging;

ref class EventSource
{
	private:
		EventHandler^ m_asyncEvent;
		
		// Callback that is invoked when the Asynchronus event completes
		void Callback(IAsyncResult^ ar);

	public:
		// An event that can only be raised synchronusly
		event EventHandler^ SyncEvent;

		// Override add and remove to gain access to the underlying event delegate
		event EventHandler^ AsyncEvent
		{
			void add(EventHandler^ eventHandler) 
			{
				m_asyncEvent += eventHandler;
			}

			void remove(EventHandler^ eventHandler) 
			{
				m_asyncEvent -= eventHandler;
			}
		}

		void RaiseSyncEvent();

		void RaiseAsyncEvent();
};


