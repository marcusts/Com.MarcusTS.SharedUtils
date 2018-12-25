// *********************************************************************************
// Assembly         : Com.MarcusTS.SmartDI.Lib
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 11-26-2018
// Last Modified On : 12-23-2018
//
// <copyright file="ThreadSafeAccessor.cs" company="Marcus Technical Services, Inc.">
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

namespace Com.MarcusTS.SharedUtils.Controls
{
   using System.Threading;

   /// <summary>
   /// Interface IThreadSafeAccessor
   /// </summary>
   public interface IThreadSafeAccessor
   {
      #region Public Methods

      /// <summary>
      /// Reads the stored value.
      /// </summary>
      /// <returns>System.Object.</returns>
      object ReadStoredValue();

      /// <summary>
      /// Writes the stored value.
      /// </summary>
      /// <param name="valueToStore">The value to store.</param>
      void WriteStoredValue(object valueToStore);

      #endregion Public Methods
   }

   /// <summary>
   /// Class ThreadSafeAccessor.
   /// Implements the <see cref="SharedUtils.Controls.IThreadSafeAccessor" />
   /// Implements the <see cref="Com.MarcusTS.SharedUtils.Controls.IThreadSafeAccessor" />
   /// </summary>
   /// <seealso cref="Com.MarcusTS.SharedUtils.Controls.IThreadSafeAccessor" />
   /// <seealso cref="SharedUtils.Controls.IThreadSafeAccessor" />
   public class ThreadSafeAccessor : IThreadSafeAccessor
   {
      #region Private Fields

      /// <summary>
      /// The stored value
      /// </summary>
      private object _storedValue;

      #endregion Private Fields

      #region Public Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="ThreadSafeAccessor" /> class.
      /// </summary>
      /// <param name="storedValue">The stored value.</param>
      public ThreadSafeAccessor(object storedValue = null)
      {
         if (storedValue != null)
         {
            WriteStoredValue(storedValue);
         }
      }

      #endregion Public Constructors

      #region Public Methods

      /// <summary>
      /// Reads the stored value.
      /// </summary>
      /// <returns>System.Object.</returns>
      public object ReadStoredValue()
      {
         return Interlocked.CompareExchange(ref _storedValue, 0, 0);
      }

      /// <summary>
      /// Writes the stored value.
      /// </summary>
      /// <param name="valueToStore">The value to store.</param>
      public void WriteStoredValue(object valueToStore)
      {
         Interlocked.Exchange(ref _storedValue, valueToStore);
      }

      #endregion Public Methods
   }
}