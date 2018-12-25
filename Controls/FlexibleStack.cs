// *********************************************************************************
// Assembly         : Com.MarcusTS.SmartDI.Lib
// Author           : Stephen Marcus (Marcus Technical Services, Inc.)
// Created          : 11-26-2018
// Last Modified On : 12-23-2018
//
// <copyright file="FlexibleStack.cs" company="Marcus Technical Services, Inc.">
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
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using Utils;

   /// <summary>
   /// Class FlexibleStack.
   /// Implements the <see cref="System.Collections.Generic.IEnumerable{T}" />
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
   public class FlexibleStack<T> : IEnumerable<T>
   {
      #region Private Fields

      /// <summary>
      /// The items
      /// </summary>
      private readonly IList<T> _items = new List<T>();

      #endregion Private Fields

      #region Private Methods

      /// <summary>
      /// Pops the or peek.
      /// </summary>
      /// <param name="removeIt">if set to <c>true</c> [remove it].</param>
      /// <returns>T.</returns>
      private T PopOrPeek(bool removeIt)
      {
         if (_items.Count > 0)
         {
            var temp = _items[_items.Count - 1];

            if (removeIt)
            {
               _items.RemoveAt(_items.Count - 1);
            }

            return temp;
         }

         // FAIL CASE
         return default(T);
      }

      #endregion Private Methods

      #region Public Methods

      /// <summary>
      /// Clears this instance.
      /// </summary>
      public void Clear()
      {
         _items.Clear();
      }

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection.</returns>
      public IEnumerator<T> GetEnumerator()
      {
         return _items.GetEnumerator();
      }

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the
      /// collection.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      /// <summary>
      /// Peeks this instance.
      /// </summary>
      /// <returns>T.</returns>
      public T Peek()
      {
         return PopOrPeek(false);
      }

      /// <summary>
      /// Pops this instance.
      /// </summary>
      /// <returns>T.</returns>
      public T Pop()
      {
         return PopOrPeek(true);
      }

      /// <summary>
      /// Pushes the specified item.
      /// </summary>
      /// <param name="item">The item.</param>
      public void Push(T item)
      {
         _items.Add(item);
      }

      /// <summary>
      /// Removes if present.
      /// </summary>
      /// <param name="item">The item.</param>
      /// <param name="dupTest">The dup test.</param>
      public void RemoveIfPresent(T            item,
                                  Predicate<T> dupTest)
      {
         if (_items.IsEmpty() || dupTest == null)
         {
            return;
         }

         var itemIdx = 0;

         do
         {
            var currItem = _items[itemIdx];

            if (dupTest(currItem))
            {
               _items.Remove(item);

               // Do *not* increment item idx
            }
            else
            {
               itemIdx++;
            }
         } while (itemIdx < _items.Count);
      }

      #endregion Public Methods

      //public void RemoveIfPresent(T item)
      //{
      //   // Can remove more than one.
      //   while (_items.Contains(item))
      //   {
      //     _items.Remove(item);
      //   }
      //}
   }
}