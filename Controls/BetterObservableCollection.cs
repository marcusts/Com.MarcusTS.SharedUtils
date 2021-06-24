// *********************************************************************************
// Copyright @2020 Marcus Technical Services, Inc.
// <copyright
// file=BetterObservableCollection.cs
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

namespace Com.MarcusTS.SharedUtils.Controls
{
   using System.Collections.ObjectModel;
   using System.Collections.Specialized;
   using System.Linq;
   using Utils;

   /// <summary>
   ///    Interface IDecideWhichIsLess
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public interface IDecideWhichIsLess<T>
   {
      /// <summary>
      ///    Determines whether [is less than] [the specified main item].
      /// </summary>
      /// <param name="mainItem">The main item.</param>
      /// <param name="compareItem">The compare item.</param>
      /// <returns><c>true</c> if [is less than] [the specified main item]; otherwise, <c>false</c>.</returns>
      bool IsLessThan(T mainItem, T compareItem);
   }

   /// <summary>
   ///    Class BetterObservableCollection.
   ///    Implements the <see cref="System.Collections.ObjectModel.ObservableCollection{T}" />
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <seealso cref="System.Collections.ObjectModel.ObservableCollection{T}" />
   public class BetterObservableCollection<T> : ObservableCollection<T>
   {
      /// <summary>
      ///    The suppress notification
      /// </summary>
      private bool _suppressNotification;

      /// <summary>
      ///    Initializes a new instance of the <see cref="BetterObservableCollection{T}" /> class.
      /// </summary>
      /// <param name="items">The items.</param>
      public BetterObservableCollection(T[] items)
      {
         AddRangeSortedAndWithoutNotification(items);
         NotifyOfAdditions(items);
      }

      /// <summary>
      ///    Initializes a new instance of the <see cref="BetterObservableCollection{T}" /> class.
      /// </summary>
      public BetterObservableCollection()
      {
      }

      /// <summary>
      ///    Adds the range sorted and without notification.
      /// </summary>
      /// <param name="list">The list.</param>
      /// <param name="comparer">The comparer.</param>
      public void AddRangeSortedAndWithoutNotification(T[] list, IDecideWhichIsLess<T> comparer = null)
      {
         ErrorUtils.IssueArgumentErrorIfTrue
         (list.IsAnEmptyList(),
            nameof(BetterObservableCollection<T>) + ": " +
            nameof(AddRangeSortedAndWithoutNotification) + " requires a valid list");

         _suppressNotification = true;

         foreach (var item in list)
         {
            AddSorted(item, comparer);
         }

         _suppressNotification = false;
      }

      /// <summary>
      ///    Adds the sorted.
      /// </summary>
      /// <param name="item">The item.</param>
      /// <param name="comparer">The comparer.</param>
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

      /// <summary>
      ///    Notifies the of additions.
      /// </summary>
      /// <param name="list">The list.</param>
      public void NotifyOfAdditions(T[] list)
      {
         OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
      }

      /// <summary>
      ///    Sorts the specified sorted.
      /// </summary>
      /// <param name="sorted">The sorted.</param>
      public void Sort(T[] sorted)
      {
         _suppressNotification = true;

         for (var i = 0; i < sorted.Count(); i++)
         {
            Move(IndexOf(sorted[i]), i);
         }

         _suppressNotification = false;
      }

      /// <summary>
      ///    Handles the <see cref="E:CollectionChanged" /> event.
      /// </summary>
      /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
         if (_suppressNotification)
         {
            return;
         }

         base.OnCollectionChanged(e);
      }
   }
}
