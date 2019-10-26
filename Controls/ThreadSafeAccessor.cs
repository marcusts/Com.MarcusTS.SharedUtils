#region License

// Copyright (c) 2019  Marcus Technical Services, Inc. <marcus@marcusts.com>
//
// This file, ThreadSafeAccessor.cs, is a part of a program called AccountViewMobile.
//
// AccountViewMobile is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Permission to use, copy, modify, and/or distribute this software
// for any purpose with or without fee is hereby granted, provided
// that the above copyright notice and this permission notice appear
// in all copies.
//
// AccountViewMobile is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// For the complete GNU General Public License,
// see <http://www.gnu.org/licenses/>.

#endregion

namespace Com.MarcusTS.SharedUtils.Controls
{
   using System.Threading;

   /// <summary>
   /// Interface IThreadSafeAccessor
   /// </summary>
   public interface IThreadSafeAccessor
   {
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
      /// <summary>
      /// The stored value
      /// </summary>
      private object _storedValue;

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
   }
}