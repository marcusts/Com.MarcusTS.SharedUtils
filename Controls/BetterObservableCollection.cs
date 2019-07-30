// *********************************************************************************
// <copyright
//    file=BetterObservableCollection.cs
//    company="Marcus Technical Services, Inc.">
//    Copyright 2019 Marcus Technical Services, Inc.
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
//  *********************************************************************************

namespace Com.MarcusTS.SharedUtils.Controls
{
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Collections.Specialized;
   using System.Linq;
   using Utils;

   public class BetterObservableCollection<T> : ObservableCollection<T>
   {
      private bool _suppressNotification = false;

      public BetterObservableCollection(T[] items)
      {
         AddRangeSortedAndWithoutNotification(items);
         NotifyOfAdditions(items);
      }

      public BetterObservableCollection()
      {
      }

      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
         if (_suppressNotification)
         {
            return;
         }

         base.OnCollectionChanged(e);
      }

      public void NotifyOfAdditions(T[] list)
      {
         OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
      }

      public void AddRangeSortedAndWithoutNotification(T[] list, IDecideWhichIsLess<T> comparer = null)
      {
         ErrorUtils.ConsiderArgumentError(list.IsEmpty(), nameof(BetterObservableCollection<T>) + ": " + nameof(AddRangeSortedAndWithoutNotification) + " requires a valid list");

         _suppressNotification = true;

         foreach (var item in list)
         {
            AddSorted(item, comparer);
         }

         _suppressNotification = false;
      }

      public void Sort(T[] sorted)
      {
         _suppressNotification = true;

         for (int i = 0; i < sorted.Count(); i++)
         {
            Move(IndexOf(sorted[i]), i);
         }

         _suppressNotification = false;
      }

      public void AddSorted(T item, IDecideWhichIsLess<T> comparer = null)
      {
         if (comparer == null)
         {
            Add(item);
            return;
         }

         var i = 0;

         while (i < Count && comparer.IsLessThan(item, this[i]))
         {
            i++;
         }

         Insert(i, item);
      }
   }

   public interface IDecideWhichIsLess<T>
   {
      bool IsLessThan(T mainItem, T compareItem);
   }
}