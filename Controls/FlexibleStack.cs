#region License

// Copyright (c) 2019  Marcus Technical Services, Inc. <marcus@marcusts.com>
//
// This file, FlexibleStack.cs, is a part of a program called AccountViewMobile.
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
      /// <summary>
      /// The items
      /// </summary>
      private readonly IList<T> _items = new List<T>();

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
      /// Clears this instance.
      /// </summary>
      public void Clear()
      {
         _items.Clear();
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
         return default;
      }

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