// *********************************************************************************
// Assembly         : Com.MarcusTS.SmartDI.Lib
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 11-26-2018
// Last Modified On : 12-23-2018
//
// <copyright file="ThreadHelper.cs" company="Marcus Technical Services, Inc.">
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

namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;

   /// <summary>
   /// Class ThreadHelper.
   /// </summary>
   public static class ThreadHelper
   {
      #region Public Methods

      /// <summary>
      /// Initializes the specified main thread identifier.
      /// </summary>
      /// <param name="mainThreadId">The main thread identifier.</param>
      public static void Initialize(int mainThreadId)
      {
         MainThreadId = mainThreadId;
      }

      #endregion Public Methods

      #region Public Properties

      /// <summary>
      /// Gets a value indicating whether this instance is on main thread.
      /// </summary>
      /// <value><c>true</c> if this instance is on main thread; otherwise, <c>false</c>.</value>
      public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;

      /// <summary>
      /// Gets the main thread identifier.
      /// </summary>
      /// <value>The main thread identifier.</value>
      public static int MainThreadId { get; private set; }

      #endregion Public Properties
   }
}