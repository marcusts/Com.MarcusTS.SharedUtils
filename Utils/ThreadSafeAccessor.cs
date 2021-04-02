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

namespace Com.MarcusTS.SharedUtils.Utils
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
      int ReadStoredValue();

      /// <summary>
      /// Writes the stored value.
      /// </summary>
      /// <param name="valueToStore">The value to store.</param>
      void WriteStoredValue(int valueToStore);
   }

   /// <summary>
   /// Class ThreadSafeAccessor.
   /// Implements the <see cref="IThreadSafeAccessor" />
   /// Implements the <see cref="IThreadSafeAccessor" />
   /// </summary>
   /// <seealso cref="IThreadSafeAccessor" />
   /// <seealso cref="IThreadSafeAccessor" />
   public class ThreadSafeAccessor : IThreadSafeAccessor
   {
      /// <summary>
      /// The stored value
      /// </summary>
      private volatile int _storedValue;

      /// <summary>
      /// Initializes a new instance of the <see cref="ThreadSafeAccessor" /> class.
      /// </summary>
      /// <param name="storedValue">The stored value.</param>
      public ThreadSafeAccessor(int? storedValue = default)
      {
         if (storedValue.HasValue)
         {
            WriteStoredValue(storedValue.Value);
         }
      }

      /// <summary>
      /// Reads the stored value.
      /// </summary>
      /// <returns>System.Object.</returns>
      public int ReadStoredValue()
      {
         return Interlocked.CompareExchange(ref _storedValue, default(int), default(int));
      }

      /// <summary>
      /// Writes the stored value.
      /// </summary>
      /// <param name="valueToStore">The value to store.</param>
      public void WriteStoredValue(int valueToStore)
      {
         Interlocked.Exchange(ref _storedValue, valueToStore);
      }
   }
   
   public static class ThreadSafeAccessor_Static
   {
      public static bool IsTrue(this IThreadSafeAccessor accessor)
      {
         return accessor?.ReadStoredValue() == 1;
      }

      public static void SetTrue(this IThreadSafeAccessor accessor)
      {
         accessor?.WriteStoredValue(1);
      }
      
      public static void SetFalse(this IThreadSafeAccessor accessor)
      {
         accessor?.WriteStoredValue(0);
      }
   }
}
