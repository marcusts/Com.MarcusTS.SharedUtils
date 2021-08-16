// *********************************************************************************
// Copyright @2021 Marcus Technical Services, Inc.
// <copyright
// file=ThreadSafeAccessor.cs
// company="Marcus Technical Services, Inc.">
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
   using System.Threading;

   /// <summary>
   /// Interface IThreadSafeAccessor
   /// </summary>
   public interface IThreadSafeAccessor
   {
      /// <summary>
      /// Determines whether this instance is false.
      /// </summary>
      /// <returns><c>true</c> if this instance is false; otherwise, <c>false</c>.</returns>
      bool IsFalse();

      /// <summary>
      /// Determines whether this instance is true.
      /// </summary>
      /// <returns><c>true</c> if this instance is true; otherwise, <c>false</c>.</returns>
      bool IsTrue();

      /// <summary>
      /// Determines whether this instance is unset.
      /// </summary>
      /// <returns><c>true</c> if this instance is unset; otherwise, <c>false</c>.</returns>
      bool IsUnset();

      /// <summary>
      /// Reads the stored value.
      /// </summary>
      /// <returns>System.Object.</returns>
      int ReadStoredValue();

      /// <summary>
      /// Sets the false.
      /// </summary>
      void SetFalse();

      /// <summary>
      /// Sets the true.
      /// </summary>
      void SetTrue();

      /// <summary>
      /// Unsets this instance.
      /// </summary>
      void Unset();

      /// <summary>
      /// Writes the stored value.
      /// </summary>
      /// <param name="valueToStore">The value to store.</param>
      void WriteStoredValue(int valueToStore);
   }

   /// <summary>
   /// Class ThreadSafeAccessor. Implements the <see cref="IThreadSafeAccessor" /> Implements the
   /// <see cref="IThreadSafeAccessor" />
   /// </summary>
   /// <seealso cref="IThreadSafeAccessor" />
   /// <seealso cref="IThreadSafeAccessor" />
   public class ThreadSafeAccessor : IThreadSafeAccessor
   {
      /// <summary>
      /// The unset value
      /// </summary>
      public const int UNSET_VALUE = int.MinValue;

      /// <summary>
      /// The stored value
      /// </summary>
      private volatile int _storedValue;

      /// <summary>
      /// Initializes a new instance of the <see cref="ThreadSafeAccessor" /> class.
      /// </summary>
      /// <param name="storedValue">The stored value.</param>
      public ThreadSafeAccessor(int storedValue = UNSET_VALUE)
      {
         // NOTE Min value is reserved for the "unset" state
         WriteStoredValue(storedValue);
      }

      /// <summary>
      /// Determines whether this instance is false.
      /// </summary>
      /// <returns><c>true</c> if this instance is false; otherwise, <c>false</c>.</returns>
      public bool IsFalse()
      {
         return ReadStoredValue() == 0;
      }

      /// <summary>
      /// Determines whether this instance is true.
      /// </summary>
      /// <returns><c>true</c> if this instance is true; otherwise, <c>false</c>.</returns>
      public bool IsTrue()
      {
         return ReadStoredValue() == 1;
      }

      /// <summary>
      /// Determines whether this instance is unset.
      /// </summary>
      /// <returns><c>true</c> if this instance is unset; otherwise, <c>false</c>.</returns>
      public bool IsUnset()
      {
         return ReadStoredValue() == UNSET_VALUE;
      }

      /// <summary>
      /// Reads the stored value.
      /// </summary>
      /// <returns>System.Object.</returns>
      public int ReadStoredValue()
      {
         return Interlocked.CompareExchange(ref _storedValue, default, default);
      }

      /// <summary>
      /// Sets the false.
      /// </summary>
      public void SetFalse()
      {
         WriteStoredValue(0);
      }

      /// <summary>
      /// Sets the true.
      /// </summary>
      public void SetTrue()
      {
         WriteStoredValue(1);
      }

      /// <summary>
      /// Unsets this instance.
      /// </summary>
      public void Unset()
      {
         WriteStoredValue(UNSET_VALUE);
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
}