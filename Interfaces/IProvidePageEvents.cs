// *********************************************************************************
// Assembly         : Com.MarcusTS.SmartDI.Lib
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 11-26-2018
// Last Modified On : 12-23-2018
//
// <copyright file="IProvidePageEvents.cs" company="Marcus Technical Services, Inc.">
//     @2018 Marcus Technical Services, Inc.
// </copyright>
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

namespace Com.MarcusTS.SharedUtils.Interfaces
{
   using System;

   /// <summary>
   /// Interface IProvidePageEvents
   /// </summary>
   public interface IProvidePageEvents
   {
      /// <summary>
      /// Regrettable use of object; we could type-cast, but than makes it difficult to pass
      /// IProvidePageEvents at lower levels without omniscient knowledge off the parent page type.
      /// </summary>
      /// <value>The get event broadcaster.</value>
      /// <remarks>The function is better than a property when there is a chance of nesting to view inside
      /// view, etc. Whenever this property is assigned, it will always seek a legal value.
      /// Otherwise, an assignment might begin with null and then never change. The root of these
      /// events is a known, valid page that should be seekable by any deriver or nested deriver.</remarks>
      Func<object> GetEventBroadcaster { get; }
   }
}
