﻿namespace Com.MarcusTS.SharedUtils.Controls
{
   using Com.MarcusTS.SharedUtils.Utils;
   using System;
   using System.Collections;
   using System.Collections.Generic;

   /// <summary>
   ///    Class FlexibleStack.
   ///    Implements the <see cref="System.Collections.Generic.IEnumerable{T}" />
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
   public class FlexibleStack<T> : IEnumerable<T>
   {
      /// <summary>
      ///    The items
      /// </summary>
      private readonly IList<T> _items = new List<T>();

      /// <summary>
      ///    Clears this instance.
      /// </summary>
      public void Clear()
      {
         _items.Clear();
      }

      /// <summary>
      ///    Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection.</returns>
      public IEnumerator<T> GetEnumerator()
      {
         return _items.GetEnumerator();
      }

      /// <summary>
      ///    Peeks this instance.
      /// </summary>
      /// <returns>T.</returns>
      public T Peek()
      {
         return PopOrPeek(false);
      }

      /// <summary>
      ///    Pops this instance.
      /// </summary>
      /// <returns>T.</returns>
      public T Pop()
      {
         return PopOrPeek(true);
      }

      /// <summary>
      ///    Pushes the specified item.
      /// </summary>
      /// <param name="item">The item.</param>
      public void Push(T item)
      {
         _items.Add(item);
      }

      /// <summary>
      ///    Removes if present.
      /// </summary>
      /// <param name="item">The item.</param>
      /// <param name="dupTest">The dup test.</param>
      public void RemoveIfPresent(T item,
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
      ///    Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>
      ///    An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the
      ///    collection.
      /// </returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      /// <summary>
      ///    Pops the or peek.
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

      //public Task RemoveIfPresent(T item)
      //{
      //   // Can remove more than one.
      //   while (_items.Contains(item))
      //   {
      //     _items.Remove(item);
      //   }
      //}
   }
}
